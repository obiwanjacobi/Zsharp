﻿using System;
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
    private IrLocality _locality = IrLocality.None;

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
    private T PopScope<T>()
        where T : IrScope
    {
        var scope = (T)_scopes.Pop();
        scope.Freeze();
        return scope;
    }
    private IrScope CurrentScope
        => _scopes.Peek();

    private T GetScopeOf<T>() where T : IrScope
        => _scopes.OfType<T>().First();

    public static IrProgram Program(SyntaxTree syntaxTree, IExternalModuleLoader moduleLoader, IrScope? parentScope = null)
    {
        if (syntaxTree.Diagnostics.Any())
            throw new InvalidOperationException("Cannot Compile when there are syntax errors.");

        var builder = new IrBuilder(moduleLoader, parentScope);
        var module = builder.Module(syntaxTree.Root);
        module = AddTemplateInstantiations(module);

        var resolver = new IrResolveSymbolsRewriter(module.Scope);
        module = resolver.FixUnresolvedSymbols(module);
        builder._diagnostics.AddRange(resolver.GetDiagnostics());

        var operatorFunctions = new IrOperatorFunctionRewriter(builder._moduleLoader);
        module = operatorFunctions.Resolve(module);

        var validator = new IrValidationRewriter();
        if (validator.Validate(module))
            builder._diagnostics.AddRange(validator.GetDiagnostics());


        return new IrProgram(syntaxTree.Root, module, builder.Diagnostics);
    }

    private static IrModule AddTemplateInstantiations(IrModule module)
    {
        var concreteDecls = module.Scope.TemplateInstantiations;
        if (!concreteDecls.Any()) return module;

        return new IrModule(module.Syntax, module.Symbol, module.Scope,
            module.Imports, module.Exports,
            module.Nodes.Concat(concreteDecls));
    }

    private static ModuleSymbol ModuleSymbol(CompilationUnitSyntax syntax)
    {
        return new(syntax.Module?.Identifier.ToSymbolName()
            ?? new SymbolName(DefaultModuleName));
    }

    private IrModule Module(CompilationUnitSyntax syntax)
    {
        var symbol = ModuleSymbol(syntax);
        SyntaxNode syn = syntax.Module is not null
            ? syntax.Module
            : syntax;

        _ = PushScope(new IrModuleScope(symbol.Name.FullOriginalName, CurrentScope));
        _locality = IrLocality.Module;

        var exports = PublicExports(syntax.PublicExports);
        var imports = UseImports(syntax.UseImports);

        GetScopeOf<IrModuleScope>().SetExports(exports);
        ProcessImports(imports);

        // by first processing declarations before statements
        // we prevent some complexity of finding forward refs of symbols.
        var nodes = Nodes(syntax.ChildNodes);

        _locality = IrLocality.None;
        var moduleScope = PopScope<IrModuleScope>();

        return new IrModule(syn, symbol, moduleScope,
            imports, exports, nodes);
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
                    _diagnostics.ImportNotFound(import.Syntax.Location, import.SymbolName.FullOriginalName);
                }
            }
        }
    }

    private List<IrDeclaration> Declarations(IEnumerable<DeclarationMemberSyntax> syntax)
    {
        var declarations = new List<IrDeclaration>();

        foreach (var mbr in syntax)
        {
            var decl = Declaration(mbr);
            declarations.Add(decl);
        }

        return declarations;
    }

    private IrDeclaration Declaration(DeclarationMemberSyntax declaration)
    {
        return declaration switch
        {
            DeclarationFunctionSyntax fds => DeclarationFunction(fds),
            DeclarationVariableTypedSyntax vdt => DeclarationVariableTyped(vdt),
            DeclarationVariableInferredSyntax vdi => DeclarationVariableInferred(vdi),
            DeclarationTypeSyntax tds => DeclarationType(tds),
            _ => throw new NotSupportedException($"IR: No support for Declaration '{declaration.SyntaxKind}'")
        };
    }

    private IrDeclarationType DeclarationType(DeclarationTypeSyntax syntax)
    {
        var typeScope = new IrTypeScope(syntax.Name.Text, CurrentScope);
        var typeParameters = TypeParameters(syntax.TypeParameters);
        var typeParamSymbols = typeParameters.Select(p => p.Symbol).ToArray();
        var index = typeScope.TryDeclareTypes(typeParamSymbols);
        if (index >= 0)
        {
            _diagnostics.TypeAlreadyDeclared(
                typeParameters[index].Syntax.Location, typeParameters[index].Symbol.Name.FullOriginalName);
        }

        var parentScope = PushScope(typeScope);
        var enums = TypeMemberEnums(syntax.Enums);
        var fields = TypeMemberFields(syntax.Fields);
        var rules = TypeMemberRules(syntax.Rules);
        typeScope = PopScope<IrTypeScope>();

        var baseType = Type(syntax.BaseType);

        var name = new SymbolName(parentScope.FullName, syntax.Name.Text);
        var typeSymbol = new DeclaredTypeSymbol(name,
            typeParamSymbols,
            enums.Select(e => e.Symbol),
            fields.Select(f => f.Symbol),
            rules.Select(r => r.Symbol),
            baseType?.Symbol);

        if (!CurrentScope.TryDeclareType(typeSymbol))
        {
            _diagnostics.TypeAlreadyDeclared(syntax.Location, typeSymbol.Name.FullOriginalName);
        }

        // TODO: inline declared types
        var locality = CurrentScope.IsExport(name) || syntax.IsPublic
            ? IrLocality.Public
            : IrLocality.None;

        var typeDecl = new IrDeclarationType(syntax, typeSymbol, typeParameters, enums, fields, rules, baseType, typeScope, locality);

        // templates are registered for later instantiation processing
        if (typeDecl.IsTemplate &&
            !CurrentScope.TryRegisterTemplateType(typeDecl))
        {
            _diagnostics.TypeTemplateAlreadyDeclared(syntax.Location, typeSymbol.Name.FullOriginalName);
        }

        return typeDecl;
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
                TypeParameterTemplateSyntax tpt => TypeParameterTemplate(tpt),
                _ => throw new NotSupportedException($"IR: No support for TypeParameter '{typeParamSyntax.SyntaxKind}'")
            };

            typeParams.Add(typeParam);
        }

        return typeParams;
    }

    private IrTypeParameterGeneric TypeParameterGeneric(TypeParameterGenericSyntax syntax)
    {
        var symbol = new TypeParameterGenericSymbol(syntax.Type.Text);
        var type = Type(syntax.DefaultType);

        return new(syntax, type, symbol);
    }

    private IrTypeParameterTemplate TypeParameterTemplate(TypeParameterTemplateSyntax syntax)
    {
        var symbol = new TypeParameterTemplateSymbol(syntax.Type.Text);
        var type = Type(syntax.DefaultType);

        return new(syntax, type, symbol);
    }

    private IrDeclarationVariable DeclarationVariableInferred(DeclarationVariableInferredSyntax syntax)
    {
        var initializer = Expression(syntax.Expression!);

        if (initializer.TypeSymbol == TypeSymbol.Void)
        {
            _diagnostics.CannotAssignVariableWithVoid(syntax.Expression!.Location, syntax.Name.Text);
        }

        if (initializer.TypeInferredSymbol?.TryGetPreferredType(out var type) != true)
            type = initializer.TypeSymbol;

        var rewriter = new IrExpressionTypeRewriter(type!);
        initializer = rewriter.RewriteExpression(initializer);

        var name = new SymbolName(CurrentScope.FullName, syntax.Name.Text);
        var symbol = new DeclaredVariableSymbol(name, type!);

        if (!CurrentScope.TryDeclareVariable(symbol))
        {
            _diagnostics.VariableAlreadyDeclared(syntax.Location, symbol.Name.FullOriginalName);
        }

        IrOperatorAssignment? assignment =
            syntax.AssignmentOperator is null
            ? null
            : new IrOperatorAssignment(syntax.AssignmentOperator, initializer.TypeSymbol);

        return new IrDeclarationVariable(syntax, symbol, type!, assignment, initializer);
    }

    private IrDeclarationVariable DeclarationVariableTyped(DeclarationVariableTypedSyntax syntax)
    {
        IrExpression? initializer = null;
        if (syntax.Expression is ExpressionSyntax synExpr)
            initializer = Expression(synExpr);

        var typeName = new SymbolName(syntax.Type.Name.Text);
        if (!CurrentScope.TryLookupSymbol<TypeSymbol>(typeName, out var typeSymbol))
        {
            _diagnostics.TypeNotFound(syntax.Location, typeName.FullOriginalName);
            typeSymbol = new TypeSymbol(typeName);
        }

        var name = new SymbolName(CurrentScope.FullName, syntax.Name.Text);
        var variableSymbol = new DeclaredVariableSymbol(name, typeSymbol);

        if (!CurrentScope.TryDeclareVariable(variableSymbol))
        {
            _diagnostics.VariableAlreadyDeclared(syntax.Location, variableSymbol.Name.FullOriginalName);
        }

        if (initializer is not null)
        {
            var rewriter = new IrExpressionTypeRewriter(typeSymbol);
            initializer = rewriter.RewriteExpression(initializer);
        }

        TypeSymbol? wrapperType = null;
        IrOperatorAssignment? assignment =
            syntax.AssignmentOperator is null
            ? null
            : new IrOperatorAssignment(syntax.AssignmentOperator, wrapperType);

        return new IrDeclarationVariable(syntax, variableSymbol, typeSymbol, assignment, initializer);
    }

    private IrDeclarationFunction DeclarationFunction(DeclarationFunctionSyntax syntax)
    {
        var functionScope = new IrFunctionScope(syntax.Name.Text, CurrentScope);
        var parentScope = PushScope(functionScope);

        var typeParameters = TypeParameters(syntax.TypeParameters);
        var typeParamSymbols = typeParameters.Select(p => p.Symbol).ToArray();
        var index = functionScope.TryDeclareTypes(typeParamSymbols);
        if (index >= 0)
        {
            _diagnostics.TypeAlreadyDeclared(
                typeParameters[index].Syntax.Location, typeParameters[index].Symbol.Name.FullOriginalName);
        }

        var parameters = Parameters(syntax.Parameters);
        var paramSymbols = parameters.Select(p => p.Symbol).ToArray();

        var returnType = Type(syntax.ReturnType) ?? IrType.Void;

        var name = new SymbolName(parentScope.FullName, syntax.Name.Text);
        var symbol = new DeclaredFunctionSymbol(name, typeParamSymbols, paramSymbols, returnType.Symbol);

        if (!parentScope.TryDeclareFunction(symbol))
        {
            _diagnostics.FunctionAlreadyDeclared(syntax.Location, symbol.Name.FullOriginalName);
        }

        index = functionScope.TryDeclareVariables(paramSymbols);
        if (index >= 0)
        {
            _diagnostics.ParameterNameAlreadyDeclared(
                parameters[index].Syntax.Location, parameters[index].Symbol.Name.FullOriginalName);
        }

        var block = CodeBlock(syntax.CodeBlock);
        functionScope = PopScope<IrFunctionScope>();

        if (returnType == IrType.Void)
        {
            var invalidReturns = block.Nodes
                .GetDescendantsOfType<IrStatementReturn>()
                .Where(r => r.Expression is not null);
            foreach (var ret in invalidReturns)
            {
                _diagnostics.VoidFunctionCannotReturnValue(ret.Expression!.Syntax.Location, name.FullOriginalName);
            }
        }

        // TODO: Local function
        var locality = CurrentScope.IsExport(name) || syntax.IsPublic
            ? IrLocality.Public
            : IrLocality.None;

        var functionDecl = new IrDeclarationFunction(syntax, symbol, typeParameters, parameters, returnType, functionScope, block, locality);

        // templates are registered for later instantiation processing
        if (functionDecl.IsTemplate &&
            !CurrentScope.TryRegisterTemplateFunction(functionDecl))
        {
            _diagnostics.FunctionTemplateAlreadyDeclared(syntax.Location, symbol.Name.FullOriginalName);
        }

        return functionDecl;
    }

    private IrCodeBlock CodeBlock(CodeBlockSyntax syntax)
    {
        var nodes = Nodes(syntax.ChildNodes);
        return new IrCodeBlock(syntax, nodes);
    }
    private List<IrNode> Nodes(SyntaxNodeList syntax)
    {
        // exclude module directives etc.
        return Nodes(syntax.Where(s => (int)s.SyntaxKind >= 1000));
    }
    private List<IrNode> Nodes(IEnumerable<SyntaxNode> syntax)
    {
        var nodes = new List<IrNode>();
        foreach (var synNode in syntax)
        {
            var node = Node(synNode);
            nodes.Add(node);
        }

        return nodes;
    }

    private IrNode Node(SyntaxNode synNode)
    {
        return synNode switch
        {
            StatementSyntax statement => Statement(statement),
            DeclarationMemberSyntax declaration => Declaration(declaration),
            _ => throw new MajaException($"IR: Invalid code block root syntax object: {synNode.GetType().FullName}.")
        };
    }

    private List<IrStatement> Statements(IEnumerable<StatementSyntax> statements)
    {
        var irStats = new List<IrStatement>();

        foreach (var stat in statements)
        {
            var irStat = Statement(stat);
            irStats.Add(irStat);
        }

        return irStats;
    }

    private IrStatement Statement(StatementSyntax statement)
    {
        return statement switch
        {
            StatementAssignmentSyntax sa => StatementAssignment(sa),
            StatementIfSyntax ifs => StatementIf(ifs),
            StatementReturnSyntax ret => StatementReturn(ret),
            StatementExpressionSyntax ses => StatementExpression(ses),
            StatementLoopSyntax sls => StatementLoop(sls),
            _ => throw new NotSupportedException($"IR: No support for Statement '{statement.SyntaxKind}'.")
        };
    }

    private IrStatementLoop StatementLoop(StatementLoopSyntax syntax)
    {
        IrExpression? expr = null;

        // TODO:
        // - register any identifiers as variables in codeblock scope

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

        DeclaredVariableSymbol? variableSymbol;
        if (DiscardSymbol.IsDiscard(syntax.Name.Text))
        {
            variableSymbol = new DiscardSymbol();

            if (expr is not IrExpressionInvocation)
                _diagnostics.DiscardOnlyInvocation(expr.Syntax.Location);
        }
        else
        {
            var name = new SymbolName(syntax.Name.Text);
            if (!CurrentScope.TryLookupSymbol<DeclaredVariableSymbol>(name, out variableSymbol))
            {
                _diagnostics.VariableNotFound(syntax.Location, name.FullOriginalName);
                variableSymbol = new DeclaredVariableSymbol(name, expr.TypeSymbol);
            }
        }

        TypeSymbol? wrapperType = null;     // TODO
        var assignment = new IrOperatorAssignment(syntax.AssignmentOperator, wrapperType);

        // TODO: check expr.TypeSymbol against variableSymbol.Type
        return new IrStatementAssignment(syntax, variableSymbol, assignment, expr, _locality);
    }

    private IrStatementExpression StatementExpression(StatementExpressionSyntax syntax)
    {
        var expr = Expression(syntax.Expression);

        if (expr is not IrExpressionInvocation)
        {
            _diagnostics.InvalidStatementExpression(syntax.Location);
        }
        else if (!expr.TypeSymbol.IsUnresolved &&
            expr.TypeSymbol != TypeSymbol.Void)
        {
            _diagnostics.FunctionReturnValueUnhandled(syntax.Location, expr.Syntax.Text);
        }

        return new IrStatementExpression(syntax, expr, _locality);
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
        if (!CurrentScope.TryLookupSymbol<DeclaredVariableSymbol>(name, out var symbol))
        {
            _diagnostics.VariableNotFound(syntax.Location, name.FullOriginalName);
            symbol = new DeclaredVariableSymbol(name, TypeSymbol.Unknown);
        }

        return new IrExpressionIdentifier(syntax, symbol, symbol.Type);
    }

    private IrExpressionMemberAccess MemberAccessExpression(ExpressionMemberAccessSyntax syntax)
    {
        var left = Expression(syntax.Left);
        var name = new SymbolName(syntax.Name.Text);

        // variable (root)
        if (left is IrExpressionIdentifier identifier)
        {
            if (!CurrentScope.TryLookupMemberType(identifier.TypeSymbol, name, out var memberType))
            {
                _diagnostics.FieldNotFoundOnType(syntax.Location, identifier.TypeSymbol.Name.FullOriginalName, name.FullOriginalName);
                memberType = TypeSymbol.Unknown;
            }
            var symbol = new FieldSymbol(name, memberType);
            return new IrExpressionMemberAccess(syntax, memberType, identifier, [symbol]);
        }

        // function (root)
        if (left is IrExpressionInvocation invocation)
        {
            if (!CurrentScope.TryLookupMemberType(invocation.TypeSymbol, name, out var memberType))
            {
                _diagnostics.FieldNotFoundOnType(syntax.Location, invocation.TypeSymbol.Name.FullOriginalName, name.FullOriginalName);
                memberType = TypeSymbol.Unknown;
            }
            var symbol = new FieldSymbol(name, memberType);
            return new IrExpressionMemberAccess(syntax, memberType, invocation, [symbol]);
        }

        // aggregate
        if (left is IrExpressionMemberAccess memberAccess)
        {
            var memberType = memberAccess.Members.Last().Type;
            if (!CurrentScope.TryLookupMemberType(memberType, name, out var memberTypeDecl))
            {
                _diagnostics.FieldNotFoundOnType(syntax.Location, memberType.Name.FullOriginalName, name.FullOriginalName);
                memberTypeDecl = TypeSymbol.Unknown;
            }
            var symbol = new FieldSymbol(name, memberType);
            return new IrExpressionMemberAccess(syntax, memberTypeDecl, memberAccess.Expression, [.. memberAccess.Members, symbol]);
        }

        _diagnostics.InvalidExpressionLeftValueMemberAccess(left.Syntax.Location, left.Syntax.Text);

        var unknownSymbol = new UnresolvedTypeSymbol(TypeSymbol.Unknown.Name);
        return new IrExpressionMemberAccess(syntax, unknownSymbol, left, Enumerable.Empty<FieldSymbol>());
    }

    private IrExpressionInvocation InvocationExpression(ExpressionInvocationSyntax syntax)
    {
        var args = Arguments(syntax.Arguments);
        var argSymbols = args.Select(a => a.Expression.TypeSymbol);

        var name = new SymbolName(syntax.Identifier.Text);
        if (!CurrentScope.TryLookupFunctionSymbol(name, argSymbols, out var functionSymbol))
        {
            // TODO: should have as many (type) params as the invocation uses.
            functionSymbol = new UnresolvedDeclaredFunctionSymbol(name);
        }

        var typeArgs = TypeArguments(syntax.TypeArguments);
        // TODO: Handle default type parameter values (missing invocation arguments)
        if (functionSymbol.TypeParameters.Count() != typeArgs.Count)
        {
            _diagnostics.MismatchTypeArgumentCount(syntax.Location, functionSymbol.Name.FullOriginalName, typeArgs.Count);
        }
        // TODO: Handle default parameter values (missing invocation arguments)
        if (functionSymbol.Parameters.Count() != args.Count)
        {
            _diagnostics.MismatchArgumentCount(syntax.Location, functionSymbol.Name.FullOriginalName, args.Count);
        }

        // TODO: is this part of the argumentMatcher / FunctionOverloadResolver?
        if (functionSymbol.IsTemplate &&
            CurrentScope.TryLookupTemplateFunction(functionSymbol.Name.FullName, out var templateFunction))
        {
            var instantiator = new IrTemplateInstantiator();
            if (instantiator.TryManifest(templateFunction, typeArgs, out var instantiatedTemplate))
            {
                // Register for code generation
                if (!CurrentScope.TryRegisterTemplateFunctionInstantiation(functionSymbol.Name.FullName, instantiatedTemplate))
                    throw new MajaException($"Registering a function template instantiation for {functionSymbol.Name.FullName} failed.");

                functionSymbol = instantiatedTemplate.Symbol;
            }
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
        ParameterSymbol? paramSymbol = null;
        if (syntax.Name is NameSyntax paramName)
        {
            var name = new SymbolName(paramName.Text);
            // parameter type to be resolved later.
            paramSymbol = new ParameterSymbol(name, TypeSymbol.Unknown);
        }

        return new IrArgument(syntax, expr, paramSymbol);
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
        var name = new SymbolName(syntax.Type.Name.Text);
        if (!CurrentScope.TryLookupSymbol<DeclaredTypeSymbol>(name, out var typeSymbol))
        {
            // TODO: Add fields etc. to satisfy expressed syntax?
            typeSymbol = new UnresolvedDeclaredTypeSymbol(name);
        }

        var typeArgs = TypeArguments(syntax.Type.TypeArguments);
        // Type template processing
        if (typeSymbol.IsTemplate &&
            CurrentScope.TryLookupTemplateType(typeSymbol.Name.FullName, out var templateType))
        {
            var instantiator = new IrTemplateInstantiator();
            if (instantiator.TryManifest(templateType, typeArgs, out var instantiatedTemplate))
            {
                // Register for code generation
                if (!CurrentScope.TryRegisterTemplateTypeInstantiation(typeSymbol.Name.FullName, instantiatedTemplate))
                    throw new MajaException($"Registering a type template instantiation for {typeSymbol.Name.FullName} failed.");

                typeSymbol = instantiatedTemplate.Symbol;
            }
        }

        var initializers = new List<IrTypeInitializerField>();
        var fields = new List<FieldSymbol>();
        foreach (var fldInitSyntax in syntax.FieldInitializers)
        {
            // TODO: Use the instantiatedTemplate(.Symbol) to resolve the fields
            if (!CurrentScope.TryLookupField(typeSymbol, fldInitSyntax.Name.Text, out var field))
            {
                field = new FieldSymbol(new SymbolName(fldInitSyntax.Name.Text), TypeSymbol.Unknown);
            }
            // TODO: can the expression contain type-params from the template?
            var expression = Expression(fldInitSyntax.Expression);
            var init = new IrTypeInitializerField(fldInitSyntax, field, expression);
            initializers.Add(init);
            fields.Add(field);
        }

        if (typeSymbol.IsGeneric)
        {
            var matcher = new IrFieldTypeMatcher(typeSymbol.TypeParameters, typeArgs, fields, initializers);
            return new IrExpressionTypeInitializer(syntax, typeSymbol, typeArgs, matcher.RewriteFieldInitializers());
        }

        return new IrExpressionTypeInitializer(syntax, typeSymbol, typeArgs, initializers);
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

        var op = new IrOperatorBinary(syntax.Operator, opType);

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
        var symbol = new ParameterSymbol(syntax.Name.Text, type.Symbol);

        return new IrParameter(syntax, symbol, type);
    }

    private IrType? Type(TypeSyntax? syntax)
    {
        if (syntax is null) return null;

        var name = new SymbolName(syntax.Name.Text);
        if (!CurrentScope.TryLookupSymbol<TypeSymbol>(name, out var symbol))
        {
            symbol = new UnresolvedTypeSymbol(name);
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
