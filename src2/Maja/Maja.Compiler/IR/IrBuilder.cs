using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

/// <summary>
/// Builds the intermediate representation tree from the syntax tree(s).
/// </summary>
internal sealed class IrBuilder
{
    private readonly DiagnosticList _diagnostics = new();
    private readonly Stack<IrScope> _scopes = new();

    public IEnumerable<DiagnosticMessage> Diagnostics
        => _diagnostics;

    private IrBuilder()
    {
        _scopes.Push(new IrGlobalScope());
    }

    private void PushScope(IrScope scope) => _scopes.Push(scope);
    private IrScope PopScope() => _scopes.Pop();
    private IrScope CurrentScope => _scopes.Peek();

    public static IrProgram Program(SyntaxTree syntaxTree)
    {
        // TODO: check for syntax diagnostics - exit if any
        // syntaxTree.Diagnostics;

        var builder = new IrBuilder();

        var module = builder.Module(syntaxTree.Root);
        builder.PushScope(new IrModuleScope(module.Symbol.Name, builder.CurrentScope));

        var compilation = builder.Compilation(syntaxTree.Root);

        var scope = (IrModuleScope)builder.PopScope();
        return new IrProgram(syntaxTree.Root, scope, module, compilation, builder.Diagnostics);
    }

    private IrModule Module(CompilationUnitSyntax syntax)
    {
        SyntaxNode syn = syntax;
        var name = "default";

        if (syntax.Module is not null)
        {
            syn = syntax.Module;
            name = syntax.Module.Identifier.Text;
        }

        var symbol = new ModuleSymbol(name);
        if (!((IrGlobalScope)CurrentScope).TryDeclareModule(symbol))
            _ = CurrentScope.TryLookupSymbol(name, out symbol);

        // TODO: would like to return the existing IrModule as well...

        return new IrModule(syn, symbol!);
    }

    private IrCompilation Compilation(CompilationUnitSyntax root)
    {
        var exports = PublicExports(root.PublicExports);
        var imports = UseImports(root.UseImports);

        var members = Declarations(root.Members);
        var statements = Statements(root.Statements);

        return new IrCompilation(root, imports, exports, statements, members);
    }

    private IEnumerable<IrDeclaration> Declarations(IEnumerable<MemberDeclarationSyntax> syntax)
    {
        var declarations = new List<IrDeclaration>();

        foreach (var mbr in syntax)
        {
            IrDeclaration decl = mbr switch
            {
                FunctionDeclarationSyntax fds => FunctionDeclaration(fds),
                VariableDeclarationTypedSyntax vdt => VariableTypedDeclaration(vdt),
                VariableDeclarationInferredSyntax vdi => VariableInferredDeclaration(vdi),
                TypeDeclarationSyntax tds => TypeDeclaration(tds),
                _ => throw new NotSupportedException($"IR: No support for Declaration '{mbr.SyntaxKind}'")
            };

            declarations.Add(decl);
        }

        return declarations;
    }

    private IrTypeDeclaration TypeDeclaration(TypeDeclarationSyntax syntax)
    {
        var enums = TypeMemberEnums(syntax.Enums);
        var fields = TypeMemberFields(syntax.Fields);
        var rules = TypeMemberRules(syntax.Rules);

        // TODO: add enums, fields and rules
        var symbol = new TypeSymbol(syntax.Name.Text);

        if (!CurrentScope.TryDeclareType(symbol))
        {
            _ = _diagnostics.TypeAlreadyDelcared(syntax.Location, syntax.Name.Text);
        }

        return new IrTypeDeclaration(syntax, symbol, enums, fields, rules);
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
                    _ = _diagnostics.EnumValueNotConstant(synExpr.Location, synExpr.Text);
            }

            var val = expr?.ConstantValue?.Value ?? id;
            var typeSymbol = expr?.TypeSymbol ?? TypeSymbol.I64;
            var symbol = new EnumSymbol(synEnum.Name.Text, typeSymbol);
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

            var symbol = new FieldSymbol(synFld.Name.Text, type.Symbol);
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
            var symbol = new RuleSymbol(synRule.Name.Text);
            var expr = Expression(synRule.Expression);

            var fld = new IrTypeMemberRule(synRule, symbol, expr);
            rules.Add(fld);
        }

        return rules;
    }

    private IrVariableDeclaration VariableInferredDeclaration(VariableDeclarationInferredSyntax syntax)
    {
        var initializer = Expression(syntax.Expression!);
        if (initializer.TypeSymbol == TypeSymbol.Void)
        {
            _ = _diagnostics.CannotAssignVariableWithVoid(syntax.Expression!.Location, syntax.Name.Text);
        }
        var symbol = new VariableSymbol(syntax.Name.Text, initializer.TypeSymbol);

        if (!CurrentScope.TryDeclareVariable(symbol))
        {
            _ = _diagnostics.VariableAlreadyDeclared(syntax.Location, syntax.Name.Text);
        }

        return new IrVariableDeclaration(syntax, symbol, initializer.TypeSymbol, initializer);
    }

    private IrVariableDeclaration VariableTypedDeclaration(VariableDeclarationTypedSyntax syntax)
    {
        IrExpression? initializer = null;
        if (syntax.Expression is ExpressionSyntax synExpr)
            initializer = Expression(synExpr);

        if (!CurrentScope.TryLookupSymbol<TypeSymbol>(syntax.Type.Name.Text, out var typeSymbol))
        {
            _ = _diagnostics.TypeNotFound(syntax.Location, syntax.Type.Name.Text);
            typeSymbol = new TypeSymbol(syntax.Type.Name.Text);
        }
        var symbol = new VariableSymbol(syntax.Name.Text, typeSymbol);

        if (!CurrentScope.TryDeclareVariable(symbol))
        {
            _ = _diagnostics.VariableAlreadyDeclared(syntax.Location, syntax.Name.Text);
        }

        return new IrVariableDeclaration(syntax, symbol, typeSymbol, initializer);
    }

    private IrFunctionDeclaration FunctionDeclaration(FunctionDeclarationSyntax syntax)
    {
        var parameters = Parameters(syntax.Parameters);
        var returnType = Type(syntax.ReturnType);
        //syntax.TypeParameters;

        var paramSymbols = parameters.Select(p => p.Symbol).ToArray();
        var symbol = new FunctionSymbol(syntax.Identifier.Text,
            paramSymbols, returnType?.Symbol);
        if (!CurrentScope.TryDeclareFunction(symbol))
        {
            _ = _diagnostics.FunctionAlreadyDelcared(syntax.Location, symbol.Name);
        }

        var scope = new IrScope(syntax.Identifier.Text, CurrentScope);
        PushScope(scope);
        var index = scope.TryDeclareVariables(paramSymbols);
        if (index >= 0)
        {
            var arrParams = parameters.ToArray();
            _ = _diagnostics.ParameterNameAlreadyDeclared(
                arrParams[index].Syntax.Location, arrParams[index].Symbol.Name);
        }

        var block = CodeBlock(syntax.CodeBlock);
        _ = PopScope();

        return new IrFunctionDeclaration(syntax, symbol, parameters, returnType, scope, block);
    }

    private IrCodeBlock CodeBlock(CodeBlockSyntax syntax)
    {
        var declarations = Declarations(syntax.Members);
        var statements = Statements(syntax.Statements);

        return new IrCodeBlock(syntax, statements, declarations);
    }

    private IEnumerable<IrStatement> Statements(IEnumerable<StatementSyntax> statements)
    {
        var irStats = new List<IrStatement>();

        foreach (var stat in statements)
        {
            IrStatement irStat = stat switch
            {
                StatementIfSyntax ifs => StatementIf(ifs),
                StatementReturnSyntax ret => StatementReturn(ret),
                StatementExpressionSyntax ses => StatementExpression(ses),
                _ => throw new NotSupportedException($"IR: No support for Statement '{stat.SyntaxKind}'.")
            };

            irStats.Add(irStat);
        }

        return irStats;
    }

    private IrStatementExpression StatementExpression(StatementExpressionSyntax syntax)
    {
        var expr = Expression(syntax.Expression);
        return new IrStatementExpression(syntax, expr);
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
            ExpressionIdentifierSyntax ide => IdentifierExpression(ide),
            _ => throw new NotSupportedException($"IR: No support for Expression '{syntax.SyntaxKind}'.")
        };
    }

    private IrExpressionIdentifier IdentifierExpression(ExpressionIdentifierSyntax syntax)
    {
        if (!CurrentScope.TryLookupSymbol<VariableSymbol>(syntax.Name.Text, out var symbol))
        {
            _ = _diagnostics.VariableNotFound(syntax.Location, syntax.Name.Text);
            symbol = new VariableSymbol(syntax.Name.Text, TypeSymbol.Unknown);
        }
        var type = symbol!.Type ?? TypeSymbol.Unknown;

        return new IrExpressionIdentifier(syntax, symbol!, type);
    }

    private IrExpressionInvocation InvocationExpression(ExpressionInvocationSyntax syntax)
    {
        var args = Arguments(syntax.Arguments);
        if (!CurrentScope.TryLookupSymbol<FunctionSymbol>(syntax.Identifier.Text, out var symbol))
        {
            _ = _diagnostics.FunctionNotFound(syntax.Location, syntax.Identifier.Text);
            symbol = new FunctionSymbol(syntax.Identifier.Text, Enumerable.Empty<ParameterSymbol>(), TypeSymbol.Unknown);
        }

        var type = symbol!.ReturnType ?? TypeSymbol.Unknown;

        return new IrExpressionInvocation(syntax, symbol, args, type);
    }

    private IEnumerable<IrArgument> Arguments(IEnumerable<ArgumentSyntax> arguments)
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
            argSymbol = new VariableSymbol(paramName.Text, null);

        return new IrArgument(syntax, expr, argSymbol);
    }

    private static IrExpressionLiteral LiteralBoolExpression(ExpressionLiteralBoolSyntax syntax)
        => new(syntax, TypeSymbol.Bool, syntax.Value);

    private static IrExpressionLiteral LiteralExpression(ExpressionLiteralSyntax syntax)
    {
        object value = syntax.LiteralString?.Text
            ?? IrNumber.ParseNumber(syntax.LiteralNumber?.Text!);

        var type = syntax.LiteralNumber != null
            ? TypeSymbol.I64
            : TypeSymbol.Str;

        return new IrExpressionLiteral(syntax, type, value);
    }

    private IrExpressionBinary BinaryExpression(ExpressionBinarySyntax syntax)
    {
        var left = Expression(syntax.Left);
        var right = Expression(syntax.Right);
        var op = new IrBinaryOperator(syntax.Operator, left.TypeSymbol);

        return new IrExpressionBinary(syntax, left, op, right);
    }

    private IEnumerable<IrParameter> Parameters(IEnumerable<ParameterSyntax> syntax)
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

        if (!CurrentScope.TryLookupSymbol<TypeSymbol>(syntax.Name.Text, out var symbol))
        {
            _ = _diagnostics.TypeNotFound(syntax.Location, syntax.Name.Text);
            symbol = new TypeSymbol(syntax.Name.Text);
        }

        return new IrType(syntax, symbol);
    }

    private static IEnumerable<IrImport> UseImports(IEnumerable<UseImportSyntax> syntax)
    {
        var imports = new List<IrImport>();

        foreach (var useImp in syntax)
        {
            var import = new IrImport(useImp);
            imports.Add(import);
        }

        return imports;
    }

    private static IEnumerable<IrExport> PublicExports(IEnumerable<PublicExportSyntax> syntax)
    {
        var exports = new List<IrExport>();

        foreach (var pubExp in syntax)
        {
            var names = PublicExport(pubExp);
            exports.AddRange(names);
        }

        return exports;
    }

    private static IEnumerable<IrExport> PublicExport(PublicExportSyntax syntax)
    {
        var exports = new List<IrExport>();
        foreach (var qn in syntax.QualifiedNames)
            exports.Add(new IrExport(qn));

        return exports;
    }
}
