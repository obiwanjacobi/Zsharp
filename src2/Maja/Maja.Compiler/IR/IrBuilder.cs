using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.External;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

/// <summary>
/// Builds the intermediate representation tree from the syntax tree(s).
/// </summary>
/// <remarks>Single threaded. Do not call the same instance from multiple threads.</remarks>
internal sealed class IrBuilder
{
    private readonly DiagnosticList _diagnostics = new();
    private readonly Stack<IrScope> _scopes = new();
    private readonly IExternalModuleLoader _moduleLoader;
    private IrLocality _locallity = IrLocality.None;

    public const string DefaultModuleName = "DefMod";

    private IrBuilder(IExternalModuleLoader moduleLoader, IrScope? parentScope)
    {
        _scopes.Push(parentScope ?? new IrGlobalScope());
        _moduleLoader = moduleLoader;
    }

    public IEnumerable<DiagnosticMessage> Diagnostics
        => _diagnostics;

    private IrScope PushScope(IrScope scope)
    {
        var parentScope = _scopes.Peek();
        _scopes.Push(scope);
        return parentScope;
    }
    private IrScope PopScope() => _scopes.Pop();
    private IrScope CurrentScope => _scopes.Peek();

    private T GetScopeOf<T>() where T : IrScope
        => _scopes.OfType<T>().Last();

    public static IrProgram Program(SyntaxTree syntaxTree, IExternalModuleLoader moduleLoader, IrScope? parentScope = null)
    {
        if (syntaxTree.Diagnostics.Any())
            throw new InvalidOperationException("Cannot Compile when there are syntax errors.");

        var builder = new IrBuilder(moduleLoader, parentScope);
        var module = builder.Module(syntaxTree.Root);
        _ = builder.PushScope(new IrModuleScope(module.Symbol.Name.FullOriginalName, builder.CurrentScope));

        var compilation = builder.Compilation(syntaxTree.Root);
        var scope = (IrModuleScope)builder.PopScope();

        return new IrProgram(syntaxTree.Root, scope, module, compilation, builder.Diagnostics);
    }

    private IrModule Module(CompilationUnitSyntax syntax)
    {
        SyntaxNode syn;
        SymbolName name;

        if (syntax.Module is not null)
        {
            syn = syntax.Module;
            name = syntax.Module.Identifier.ToSymbolName();
        }
        else
        {
            syn = syntax;
            name = new SymbolName(DefaultModuleName);
        }

        var symbol = new ModuleSymbol(name);

        if (CurrentScope is IrGlobalScope globalScope &&
            !globalScope.TryDeclareModule(symbol) &&
            globalScope.TryLookupSymbol<ModuleSymbol>(name, out var existingSymbol))
        {
            symbol = existingSymbol;
        }

        // TODO: would like to return the existing IrModule as well...

        return new IrModule(syn, symbol);
    }

    private IrCompilation Compilation(CompilationUnitSyntax root)
    {
        var exports = PublicExports(root.PublicExports);
        var imports = UseImports(root.UseImports);

        GetScopeOf<IrModuleScope>().SetExports(exports);
        ProcessImports(imports);

        var members = Declarations(root.Members);

        _locallity = IrLocality.Module;
        var statements = Statements(root.Statements);
        _locallity = IrLocality.None;

        return new IrCompilation(root, imports, exports, statements, members);
    }

    private void ProcessImports(IEnumerable<IrImport> imports)
    {
        foreach (var import in imports)
        {
            if (_moduleLoader.TryLookupModule(import.SymbolName, out var extMod))
            {
                if (!((IrModuleScope)CurrentScope).TryDeclareModule(extMod))
                {
                    // TODO: duplicate?
                }
            }
            else
            {
                var modules = _moduleLoader.LookupNamespace(import.SymbolName.ToNamespace());

                if (modules.Count > 0)
                {
                    foreach (var module in modules)
                    {
                        ((IrModuleScope)CurrentScope).TryDeclareModule(module);
                    }
                }
                else
                {
                    _diagnostics.ImportNotFound(import.Syntax.Location, import.SymbolName.FullName);
                }
            }
        }
    }

    private List<IrDeclaration> Declarations(IEnumerable<MemberDeclarationSyntax> syntax)
    {
        var declarations = new List<IrDeclaration>();

        foreach (var mbr in syntax)
        {
            IrDeclaration decl = mbr switch
            {
                FunctionDeclarationSyntax fds => DeclarationFunction(fds),
                VariableDeclarationTypedSyntax vdt => DeclarationVariableTyped(vdt),
                VariableDeclarationInferredSyntax vdi => DeclarationVariableInferred(vdi),
                TypeDeclarationSyntax tds => DeclarationType(tds),
                _ => throw new NotSupportedException($"IR: No support for Declaration '{mbr.SyntaxKind}'")
            };

            declarations.Add(decl);
        }

        return declarations;
    }

    private IrDeclarationType DeclarationType(TypeDeclarationSyntax syntax)
    {
        var enums = TypeMemberEnums(syntax.Enums);
        var fields = TypeMemberFields(syntax.Fields);
        var rules = TypeMemberRules(syntax.Rules);

        var name = new SymbolName(CurrentScope.FullName, syntax.Name.Text);
        var symbol = new DeclaredTypeSymbol(name,
            enums.Select(e => e.Symbol),
            fields.Select(f => f.Symbol),
            rules.Select(r => r.Symbol));

        if (!CurrentScope.TryDeclareType(symbol))
        {
            _diagnostics.TypeAlreadyDeclared(syntax.Location, syntax.Name.Text);
        }

        // TODO: inline declared types
        var locality = CurrentScope.IsExport(name) || syntax.IsPublic
            ? IrLocality.Public
            : IrLocality.None;

        return new IrDeclarationType(syntax, symbol, enums, fields, rules, locality);
    }

    private IEnumerable<IrTypeMemberEnum> TypeMemberEnums(TypeMemberListSyntax<MemberEnumSyntax>? syntax)
    {
        if (syntax is null)
            return Enumerable.Empty<IrTypeMemberEnum>();

        var enums = new List<IrTypeMemberEnum>();

        var id = 0;
        foreach (var synEnum in syntax.Items)
        {
            IrExpression? expr = null;
            if (synEnum.Expression is ExpressionSyntax synExpr)
            {
                expr = Expression(synExpr);

                if (expr.ConstantValue is null)
                    _diagnostics.EnumValueNotConstant(synExpr.Location, synExpr.Text);
            }

            var val = expr?.ConstantValue?.Value ?? id;
            var typeSymbol = expr?.TypeSymbol ?? TypeSymbol.I64;
            var name = new SymbolName(synEnum.Name.Text);
            var symbol = new EnumSymbol(name, typeSymbol, val);
            var enm = new IrTypeMemberEnum(synEnum, symbol, expr, val);
            enums.Add(enm);
            id++;
        }

        return enums;
    }

    private IEnumerable<IrTypeMemberField> TypeMemberFields(TypeMemberListSyntax<MemberFieldSyntax>? syntax)
    {
        if (syntax is null)
            return Enumerable.Empty<IrTypeMemberField>();

        var fields = new List<IrTypeMemberField>();

        foreach (var synFld in syntax.Items)
        {
            var type = Type(synFld.Type)!;
            IrExpression? defExpr = null;
            if (synFld.Expression is ExpressionSyntax synExpr)
                defExpr = Expression(synExpr);

            var name = new SymbolName(synFld.Name.Text);
            var symbol = new FieldSymbol(name, type.Symbol);
            var fld = new IrTypeMemberField(synFld, symbol, type, defExpr);
            fields.Add(fld);
        }

        return fields;
    }

    private IEnumerable<IrTypeMemberRule> TypeMemberRules(TypeMemberListSyntax<MemberRuleSyntax>? syntax)
    {
        if (syntax is null)
            return Enumerable.Empty<IrTypeMemberRule>();

        var rules = new List<IrTypeMemberRule>();

        foreach (var synRule in syntax.Items)
        {
            var name = new SymbolName(synRule.Name.Text);
            var symbol = new RuleSymbol(name);
            var expr = Expression(synRule.Expression);

            var fld = new IrTypeMemberRule(synRule, symbol, expr);
            rules.Add(fld);
        }

        return rules;
    }

    private List<IrTypeParameter> TypeParameters(IEnumerable<TypeParameterSyntax> syntax)
    {
        var typeParams = new List<IrTypeParameter>();

        foreach (var typeParamSyntax in syntax)
        {
            IrTypeParameter typeParam = typeParamSyntax switch
            {
                TypeParameterGenericSyntax tpg => TypeParameterGeneric(tpg),
                _ => throw new NotSupportedException($"IR: No support for TypeParameter '{typeParamSyntax.SyntaxKind}'")
            };

            typeParams.Add(typeParam);
        }

        return typeParams;
    }

    private IrTypeParameterGeneric TypeParameterGeneric(TypeParameterGenericSyntax syntax)
    {
        var symbol = new TypeParameterSymbol(syntax.Type.Text);
        var type = Type(syntax.DefaultType);

        return new(syntax, type, symbol);
    }

    private IrDeclarationVariable DeclarationVariableInferred(VariableDeclarationInferredSyntax syntax)
    {
        var initializer = Expression(syntax.Expression!);

        if (initializer.TypeSymbol == TypeSymbol.Void)
        {
            _diagnostics.CannotAssignVariableWithVoid(syntax.Expression!.Location, syntax.Name.Text);
        }

        if (initializer.TypeInferredSymbol?.TryGetPreferredType(out var type) != true)
            type = initializer.TypeSymbol;

        var name = new SymbolName(CurrentScope.FullName, syntax.Name.Text);
        var symbol = new VariableSymbol(name, type!);

        if (!CurrentScope.TryDeclareVariable(symbol))
        {
            _diagnostics.VariableAlreadyDeclared(syntax.Location, syntax.Name.Text);
        }

        return new IrDeclarationVariable(syntax, symbol, type!, initializer);
    }

    private IrDeclarationVariable DeclarationVariableTyped(VariableDeclarationTypedSyntax syntax)
    {
        IrExpression? initializer = null;
        if (syntax.Expression is ExpressionSyntax synExpr)
            initializer = Expression(synExpr);

        var typeName = new SymbolName(syntax.Type.Name.Text);
        if (!CurrentScope.TryLookupSymbol<TypeSymbol>(typeName, out var typeSymbol))
        {
            _diagnostics.TypeNotFound(syntax.Location, syntax.Type.Name.Text);
            typeSymbol = new TypeSymbol(typeName);
        }

        var name = new SymbolName(CurrentScope.FullName, syntax.Name.Text);
        var variableSymbol = new VariableSymbol(name, typeSymbol);

        if (!CurrentScope.TryDeclareVariable(variableSymbol))
        {
            _diagnostics.VariableAlreadyDeclared(syntax.Location, syntax.Name.Text);
        }

        if (initializer is not null)
        {
            var rewriter = new IrExpressionTypeRewriter(typeSymbol);
            initializer = rewriter.RewriteExpression(initializer);
        }

        return new IrDeclarationVariable(syntax, variableSymbol, typeSymbol, initializer);
    }

    private IrDeclarationFunction DeclarationFunction(FunctionDeclarationSyntax syntax)
    {
        var functionScope = new IrFunctionScope(syntax.Identifier.Text, CurrentScope);
        var parentScope = PushScope(functionScope);

        var typeParameters = TypeParameters(syntax.TypeParameters);
        var typeParamSymbols = typeParameters.Select(p => p.Symbol).ToArray();
        var index = functionScope.TryDeclareTypes(typeParamSymbols);
        if (index >= 0)
        {
            _diagnostics.TypeAlreadyDeclared(
                typeParameters[index].Syntax.Location, typeParameters[index].Symbol.Name.FullName);
        }

        var parameters = Parameters(syntax.Parameters);
        var paramSymbols = parameters.Select(p => p.Symbol).ToArray();

        var returnType = Type(syntax.ReturnType) ?? IrType.Void;

        var name = new SymbolName(parentScope.FullName, syntax.Identifier.Text);
        var symbol = new FunctionSymbol(name, typeParamSymbols, paramSymbols, returnType.Symbol);

        if (!parentScope.TryDeclareFunction(symbol))
        {
            _diagnostics.FunctionAlreadyDelcared(syntax.Location, symbol.Name.FullName);
        }

        index = functionScope.TryDeclareVariables(paramSymbols);
        if (index >= 0)
        {
            _diagnostics.ParameterNameAlreadyDeclared(
                parameters[index].Syntax.Location, parameters[index].Symbol.Name.FullName);
        }

        var block = CodeBlock(syntax.CodeBlock);
        _ = PopScope();

        if (returnType == IrType.Void)
        {
            // TODO: this does not catch return statements inside if's and loops etc.
            var invalidReturns = block.Statements
                .OfType<IrStatementReturn>()
                .Where(r => r.Expression is not null);
            foreach (var ret in invalidReturns)
            {
                _diagnostics.VoidFunctionCannotReturnValue(ret.Expression!.Syntax.Location, name.FullName);
            }
        }

        // TODO: Local function
        var locality = CurrentScope.IsExport(name) || syntax.IsPublic
            ? IrLocality.Public
            : IrLocality.None;

        return new IrDeclarationFunction(syntax, symbol, typeParameters, parameters, returnType, functionScope, block, locality);
    }

    private IrCodeBlock CodeBlock(CodeBlockSyntax syntax)
    {
        var declarations = Declarations(syntax.Members);
        var statements = Statements(syntax.Statements);

        return new IrCodeBlock(syntax, statements, declarations);
    }

    private List<IrStatement> Statements(IEnumerable<StatementSyntax> statements)
    {
        var irStats = new List<IrStatement>();

        foreach (var stat in statements)
        {
            IrStatement irStat = stat switch
            {
                StatementAssignmentSyntax sa => StatementAssignment(sa),
                StatementIfSyntax ifs => StatementIf(ifs),
                StatementReturnSyntax ret => StatementReturn(ret),
                StatementExpressionSyntax ses => StatementExpression(ses),
                StatementLoopSyntax sls => StatementLoop(sls),
                _ => throw new NotSupportedException($"IR: No support for Statement '{stat.SyntaxKind}'.")
            };

            irStats.Add(irStat);
        }

        return irStats;
    }

    private IrStatementLoop StatementLoop(StatementLoopSyntax syntax)
    {
        IrExpression? expr = null;

        // TODO:
        // - register any indentifiers as variables in codeblock scope

        if (syntax.Expression is not null)
        {
            expr = Expression(syntax.Expression);

            var isValid = expr.TypeSymbol.MatchesWith(TypeSymbol.I32) ||
                expr.TypeSymbol.MatchesWith(TypeSymbol.Bool);
            if (!isValid)
                _diagnostics.InvalidLoopExpression(syntax.Location);
        }

        var codeBlock = CodeBlock(syntax.CodeBlock);

        return new IrStatementLoop(syntax, expr, codeBlock);
    }

    private IrStatementAssignment StatementAssignment(StatementAssignmentSyntax syntax)
    {
        var expr = Expression(syntax.Expression);

        VariableSymbol? variableSymbol;
        if (DiscardSymbol.IsDiscard(syntax.Name.Text))
        {
            variableSymbol = new DiscardSymbol();

            if (expr is not IrExpressionInvocation)
                _diagnostics.DiscardOnlyInvocation(expr.Syntax.Location);
        }
        else
        {
            var name = new SymbolName(syntax.Name.Text);
            if (!CurrentScope.TryLookupSymbol<VariableSymbol>(name, out variableSymbol))
            {
                _diagnostics.VariableNotFound(syntax.Location, syntax.Name.Text);
                variableSymbol = new VariableSymbol(name, expr.TypeSymbol);
            }
        }

        // TODO: check expr.TypeSymbol against variableSymbol.Type
        return new IrStatementAssignment(syntax, variableSymbol, expr, _locallity);
    }

    private IrStatementExpression StatementExpression(StatementExpressionSyntax syntax)
    {
        var expr = Expression(syntax.Expression);

        if (expr is not IrExpressionInvocation)
        {
            _diagnostics.InvalidStatementExpression(syntax.Location);
        }
        return new IrStatementExpression(syntax, expr, _locallity);
    }

    private IrStatementReturn StatementReturn(StatementReturnSyntax syntax)
    {
        IrExpression? expr = null;
        if (syntax.Expression is ExpressionSyntax synExpr)
            expr = Expression(synExpr);

        return new IrStatementReturn(syntax, expr);
    }

    private IrStatementIf StatementIf(StatementIfSyntax syntax)
    {
        var condition = Expression(syntax.Expression);
        var codeBlock = CodeBlock(syntax.CodeBlock);

        IrElseClause? elseStat = null;
        IrElseIfClause? elifStat = null;
        if (syntax.Else is StatementElseSyntax elseSyntax)
        {
            var elseBlock = CodeBlock(elseSyntax.CodeBlock);
            elseStat = new IrElseClause(elseSyntax, elseBlock);
        }
        else if (syntax.ElseIf is StatementElseIfSyntax elifSyntax)
        {
            var elifBlock = CodeBlock(elifSyntax.CodeBlock);
            var elifCondition = Expression(elifSyntax.Expression);
            var nested = ElseIfClause(elifSyntax);
            elifStat = new IrElseIfClause(elifSyntax, elifCondition, elifBlock, nested.elseStat, nested.elifStat);
        }

        return new IrStatementIf(syntax, condition, codeBlock, elseStat, elifStat);
    }

    private (IrElseClause? elseStat, IrElseIfClause? elifStat) ElseIfClause(StatementElseIfSyntax syntax)
    {
        IrElseClause? elseStat = null;
        IrElseIfClause? elifStat = null;
        if (syntax.Else is StatementElseSyntax elseSyntax)
        {
            var elseBlock = CodeBlock(elseSyntax.CodeBlock);
            elseStat = new IrElseClause(elseSyntax, elseBlock);
        }
        else if (syntax.ElseIf is StatementElseIfSyntax elifSyntax)
        {
            var elifBlock = CodeBlock(elifSyntax.CodeBlock);
            var elifCondition = Expression(elifSyntax.Expression);
            var nested = ElseIfClause(elifSyntax);
            elifStat = new IrElseIfClause(elifSyntax, elifCondition, elifBlock, nested.elseStat, nested.elifStat);
        }

        return (elseStat, elifStat);
    }

    private IrExpression Expression(ExpressionSyntax syntax)
    {
        return syntax switch
        {
            ExpressionBinarySyntax be => BinaryExpression(be),
            ExpressionLiteralSyntax le => LiteralExpression(le),
            ExpressionLiteralBoolSyntax lbe => LiteralBoolExpression(lbe),
            ExpressionInvocationSyntax ie => InvocationExpression(ie),
            ExpressionTypeInitializerSyntax eti => TypeInitializerExpression(eti),
            ExpressionMemberAccessSyntax ema => MemberAccessExpression(ema),
            ExpressionIdentifierSyntax ide => IdentifierExpression(ide),
            ExpressionRangeSyntax ers => RangeExpression(ers),
            _ => throw new NotSupportedException($"IR: No support for Expression '{syntax.SyntaxKind}'.")
        };
    }

    private IrExpressionRange RangeExpression(ExpressionRangeSyntax syntax)
    {
        var start = syntax.Start is null
            ? null
            : Expression(syntax.Start);

        var end = syntax.End is null
            ? null
            : Expression(syntax.End);

        return new IrExpressionRange(syntax, start, end);
    }

    private IrExpressionIdentifier IdentifierExpression(ExpressionIdentifierSyntax syntax)
    {
        var name = new SymbolName(syntax.Name.Text);
        if (!CurrentScope.TryLookupSymbol<VariableSymbol>(name, out var symbol))
        {
            _diagnostics.VariableNotFound(syntax.Location, syntax.Name.Text);
            symbol = new VariableSymbol(name, TypeSymbol.Unknown);
        }

        var type = symbol.Type ?? TypeSymbol.Unknown;
        return new IrExpressionIdentifier(syntax, symbol, type);
    }

    private IrExpressionMemberAccess MemberAccessExpression(ExpressionMemberAccessSyntax syntax)
    {
        var left = Expression(syntax.Left);
        var name = new SymbolName(syntax.Name.Text);

        // variable (root)
        if (left is IrExpressionIdentifier identifier)
        {
            if (!CurrentScope.TryLookupMemberType(identifier.TypeSymbol.Name, name, out var memberType))
            {
                _diagnostics.FieldNotFoundOnType(syntax.Location, identifier.TypeSymbol.Name.Value, name.Value);
                memberType = TypeSymbol.Unknown;
            }
            var symbol = new FieldSymbol(name, memberType);
            return new IrExpressionMemberAccess(syntax, memberType, identifier, [symbol]);
        }

        // function (root)
        if (left is IrExpressionInvocation invocation)
        {
            if (!CurrentScope.TryLookupMemberType(invocation.TypeSymbol.Name, name, out var memberType))
            {
                _diagnostics.FieldNotFoundOnType(syntax.Location, invocation.TypeSymbol.Name.Value, name.Value);
                memberType = TypeSymbol.Unknown;
            }
            var symbol = new FieldSymbol(name, memberType);
            return new IrExpressionMemberAccess(syntax, memberType, invocation, [symbol]);
        }

        // aggregate
        if (left is IrExpressionMemberAccess memberAccess)
        {
            var typeName = memberAccess.Members.Last().Type.Name;
            if (!CurrentScope.TryLookupMemberType(typeName, name, out var memberType))
            {
                _diagnostics.FieldNotFoundOnType(syntax.Location, typeName.Value, name.Value);
                memberType = TypeSymbol.Unknown;
            }
            var symbol = new FieldSymbol(name, memberType);
            return new IrExpressionMemberAccess(syntax, memberType, memberAccess.Expression, [.. memberAccess.Members, symbol]);
        }

        throw new MajaException($"Unexpected member access left expression type: {left.GetType().FullName}.");
    }

    private IrExpressionInvocation InvocationExpression(ExpressionInvocationSyntax syntax)
    {
        var typeArgs = TypeArguments(syntax.TypeArguments);
        var args = Arguments(syntax.Arguments);
        var argSymbols = args.Select(a => a.Expression.TypeSymbol);

        var name = new SymbolName(syntax.Identifier.Text);
        if (!CurrentScope.TryLookupFunctionSymbol(name, argSymbols, out var functionSymbol))
        {
            _diagnostics.FunctionNotFound(syntax.Location, syntax.Identifier.Text);
            // TODO: should have as many (type) params as the invocation uses.
            functionSymbol = new FunctionSymbol(name, Enumerable.Empty<TypeParameterSymbol>(),
                Enumerable.Empty<ParameterSymbol>(), TypeSymbol.Unknown);
        }

        if (functionSymbol.TypeParameters.Count() != typeArgs.Count)
        {
            _diagnostics.MismatchTypeArgumentCount(syntax.Location, functionSymbol.Name.Value, typeArgs.Count);
        }
        if (functionSymbol.Parameters.Count() != args.Count)
        {
            _diagnostics.MismatchArgumentCount(syntax.Location, functionSymbol.Name.Value, args.Count);
        }

        var matcher = new IrArgumentMatcher(
            functionSymbol.TypeParameters, typeArgs,
            functionSymbol.Parameters, args);

        if (!matcher.TryMapSymbol(functionSymbol.ReturnType, out var retType))
            retType = functionSymbol.ReturnType ?? TypeSymbol.Unknown;

        var newArgs = matcher.RewriteArgumentTypes();

        if (matcher.Diagnostics.HasDiagnostics)
            _diagnostics.AddRange(matcher.Diagnostics);

        return new IrExpressionInvocation(syntax, functionSymbol, typeArgs, newArgs, retType);
    }

    private List<IrArgument> Arguments(IEnumerable<ArgumentSyntax> arguments)
    {
        var args = new List<IrArgument>();
        foreach (var synArg in arguments)
        {
            var arg = Argument(synArg);
            args.Add(arg);
        }
        return args;
    }

    private IrArgument Argument(ArgumentSyntax syntax)
    {
        var expr = Expression(syntax.Expression);
        VariableSymbol? argSymbol = null;
        if (syntax.Name is NameSyntax paramName)
        {
            var name = new SymbolName(paramName.Text);
            argSymbol = new VariableSymbol(name, TypeSymbol.Unknown);
        }

        return new IrArgument(syntax, expr, argSymbol);
    }

    private List<IrTypeArgument> TypeArguments(IEnumerable<TypeArgumentSyntax> arguments)
    {
        var args = new List<IrTypeArgument>();
        foreach (var synArg in arguments)
        {
            var arg = TypeArgument(synArg);
            args.Add(arg);
        }
        return args;
    }

    private IrTypeArgument TypeArgument(TypeArgumentSyntax syntax)
    {
        var type = Type(syntax.Type)
            ?? throw new MajaException("IR: TypeArgumentSyntax does not specify a Type.");

        return new IrTypeArgument(syntax, type);
    }

    private IrExpressionTypeInitializer TypeInitializerExpression(ExpressionTypeInitializerSyntax syntax)
    {
        var name = new SymbolName(syntax.Identifier.Name.Text);
        if (!CurrentScope.TryLookupSymbol<TypeSymbol>(name, out var type))
        {
            _diagnostics.TypeNotFound(syntax.Location, syntax.Identifier.Name.Text);
            type = new TypeSymbol(name);
        }

        var initializers = new List<IrTypeInitializerField>();
        foreach (var fldInitSyntax in syntax.FieldInitializers)
        {
            var fldName = new SymbolName(fldInitSyntax.Name.Text);
            var field = new FieldSymbol(fldName, type);
            var expression = Expression(fldInitSyntax.Expression);
            var init = new IrTypeInitializerField(fldInitSyntax, field, expression);
            initializers.Add(init);
        }

        return new(syntax, type, initializers);
    }

    private static IrExpressionLiteral LiteralBoolExpression(ExpressionLiteralBoolSyntax syntax)
        => new(syntax, TypeSymbol.Bool, syntax.Value);

    private static IrExpressionLiteral LiteralExpression(ExpressionLiteralSyntax syntax)
    {
        object? value;
        TypeSymbol type;

        if (syntax.LiteralNumber?.Text is not null)
        {
            var types = IrNumber.ParseNumber(syntax.LiteralNumber.Text, out value);
            type = new TypeInferredSymbol(types);
        }
        else if (syntax.LiteralString?.Text is not null)
        {
            value = syntax.LiteralString.Text;
            type = TypeSymbol.Str;
        }
        else
        {
            throw new NotSupportedException("ExpressionLiteralSyntax could not be parsed. Empty?");
        }

        return new IrExpressionLiteral(syntax, type, value!);
    }

    private IrExpressionBinary BinaryExpression(ExpressionBinarySyntax syntax)
    {
        var left = Expression(syntax.Left);
        var right = Expression(syntax.Right);

        if (!IrTypeConversion.TryDecideType(left.TypeSymbol, right.TypeSymbol, out var opType))
            opType = left.TypeSymbol;

        var op = new IrBinaryOperator(syntax.Operator, opType);

        return new IrExpressionBinary(syntax, left, op, right);
    }

    private List<IrParameter> Parameters(IEnumerable<ParameterSyntax> syntax)
    {
        var prms = new List<IrParameter>();

        foreach (var param in syntax)
        {
            var prm = Parameter(param);
            prms.Add(prm);
        }

        return prms;
    }

    private IrParameter Parameter(ParameterSyntax syntax)
    {
        var type = Type(syntax.Type);
        Debug.Assert(type is not null);
        var symbol = new ParameterSymbol(syntax.Name.Text, type!.Symbol);

        return new IrParameter(syntax, symbol, type);
    }

    private IrType? Type(TypeSyntax? syntax)
    {
        if (syntax is null) return null;

        var name = new SymbolName(syntax.Name.Text);
        if (!CurrentScope.TryLookupSymbol<TypeSymbol>(name, out var symbol))
        {
            _diagnostics.TypeNotFound(syntax.Location, syntax.Name.Text);
            symbol = new TypeSymbol(name);
        }

        return new IrType(syntax, symbol);
    }

    private static List<IrImport> UseImports(IEnumerable<UseImportSyntax> syntax)
    {
        var imports = new List<IrImport>();

        foreach (var useImp in syntax)
        {
            var uses = UseImport(useImp);
            imports.AddRange(uses);
        }

        return imports;
    }

    private static List<IrImport> UseImport(UseImportSyntax syntax)
    {
        var imports = new List<IrImport>();
        foreach (var qn in syntax.QualifiedNames)
            imports.Add(new IrImport(qn));

        return imports;
    }

    private static List<IrExport> PublicExports(IEnumerable<PublicExportSyntax> syntax)
    {
        var exports = new List<IrExport>();

        foreach (var pubExp in syntax)
        {
            var names = PublicExport(pubExp);
            exports.AddRange(names);
        }

        return exports;
    }

    private static List<IrExport> PublicExport(PublicExportSyntax syntax)
    {
        var exports = new List<IrExport>();
        foreach (var qn in syntax.QualifiedNames)
        {
            var name = new SymbolName(qn.Text);
            exports.Add(new IrExport(qn, name));
        }
        return exports;
    }
}
