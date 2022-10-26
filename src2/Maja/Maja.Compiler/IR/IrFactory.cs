using System;
using System.Collections.Generic;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal class IrFactory
{
    private readonly DiagnosticList _diagnostics = new();
    public IEnumerable<DiagnosticMessage> Diagnostics
        => _diagnostics;

    private IrFactory() { }

    public static IrScope GlobalScope(SyntaxTree syntaxTree)
    {
        // TODO: check for syntax diagnostics - exit if any
        // syntaxTree.Diagnostics;

        var factory = new IrFactory();

        var scope = factory.Module(syntaxTree.Root);

        return scope;
    }

    private IrModuleScope Module(CompilationUnitSyntax root)
    {
        var exports = PublicExports(root.PublicExports);
        var imports = UseImports(root.UseImports);

        var members = Declarations(root.Members);
        var statements = Statements(root.Statements);

        return new IrModuleScope(root, exports, imports, statements, members);
    }

    private IEnumerable<IrDeclaration> Declarations(IEnumerable<MemberDeclarationSyntax> syntax)
    {
        var declarations = new List<IrDeclaration>();

        foreach (var mbr in syntax)
        {
            //var decl = mbr switch
            //{
            //    FunctionDeclarationSyntax fd => FunctionDeclaration(fd),
            //    VariableDeclarationTypedSyntax vtd => VariableTypedDeclaration(vtd),
            //    VariableDeclarationInferredSyntax vid => VariableInferredDeclaration(vid),
            //    TypeDeclarationSyntax td => TypeDeclaration(td),
            //    _ => throw new NotSupportedException($"IR: No support for Declaration {mbr.SyntaxKind}")
            //};

            IrDeclaration decl;

            switch (mbr.SyntaxKind)
            {
                case SyntaxKind.FunctionDeclaration:
                    decl = FunctionDeclaration((FunctionDeclarationSyntax)mbr);
                    break;
                case SyntaxKind.TypedVariableDeclaration:
                    decl = VariableTypedDeclaration((VariableDeclarationTypedSyntax)mbr);
                    break;
                case SyntaxKind.InferredVariableDeclaration:
                    decl = VariableInferredDeclaration((VariableDeclarationInferredSyntax)mbr);
                    break;
                case SyntaxKind.TypeDeclaration:
                    decl = TypeDeclaration((TypeDeclarationSyntax)mbr);
                    break;
                default:
                    throw new NotSupportedException($"IR: No support for Declaration {mbr.SyntaxKind}");
            }

            declarations.Add(decl);
        }

        return declarations;
    }

    private IrTypeDeclaration TypeDeclaration(TypeDeclarationSyntax syntax)
    {
        throw new NotImplementedException();
    }

    private IrVariableDeclaration VariableInferredDeclaration(VariableDeclarationInferredSyntax syntax)
    {
        var varSymbol = new VariableSymbol(syntax.Name.Text);
        var initializer = Expression(syntax.Expression!);

        return new IrVariableDeclaration(syntax, varSymbol, initializer.Type, initializer);
    }

    private IrVariableDeclaration VariableTypedDeclaration(VariableDeclarationTypedSyntax syntax)
    {
        var varSymbol = new VariableSymbol(syntax.Name.Text);
        IrExpression? initializer = null;
        if (syntax.Expression is ExpressionSyntax synExpr)
            initializer = Expression(synExpr);
        var type = new TypeSymbol(syntax.Type.Name.Text);

        return new IrVariableDeclaration(syntax, varSymbol, type, initializer);
    }

    private IrFunctionDeclaration FunctionDeclaration(FunctionDeclarationSyntax syntax)
    {
        var block = CodeBlock(syntax.CodeBlock);
        var prms = Parameters(syntax.Parameters);
        IrType? type = null;
        if (syntax.ReturnType is TypeSyntax retType)
            type = Type(retType);
        //functionDeclaration.TypeParameters;

        return new IrFunctionDeclaration(syntax, prms, type, block);
    }

    private IrScope CodeBlock(CodeBlockSyntax syntax)
    {
        var declarations = Declarations(syntax.Members);
        var statements = Statements(syntax.Statements);

        return new IrScope(syntax, statements, declarations);
    }

    private IEnumerable<IrStatement> Statements(IEnumerable<StatementSyntax> statements)
    {
        var irStats = new List<IrStatement>();

        foreach (var stat in statements)
        {
            var irStat = stat switch
            {
                StatementIfSyntax ifs => StatementIf(ifs),
                _ => throw new NotSupportedException($"IR: No support for statement {stat.SyntaxKind}.")
            };

            irStats.Add(irStat);
        }

        return irStats;
    }

    private IrStatementIf StatementIf(StatementIfSyntax syntax)
    {
        var codeBlock = CodeBlock(syntax.CodeBlock);
        var condition = Expression(syntax.Expression);

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
            elifStat = new IrElseIfClause(elifSyntax, elifCondition, elifBlock);
        }

        return new IrStatementIf(syntax, condition, codeBlock, elseStat, elifStat);
    }

    private IrExpression Expression(ExpressionSyntax syntax)
    {
        return syntax switch
        {
            ExpressionBinarySyntax be => BinaryExpression(be),
            ExpressionLiteralSyntax le => LiteralExpression(le),
            ExpressionLiteralBoolSyntax lbe => LiteralBoolExpression(lbe),
            ExpressionInvocationSyntax ie => InvocationExpression(ie),
            _ => throw new NotSupportedException($"IR: No support for Expression {syntax.SyntaxKind}.")
        };
    }

    private IrExpressionInvocation InvocationExpression(ExpressionInvocationSyntax syntax)
    {
        // TODO: implement
        return new IrExpressionInvocation(syntax, TypeSymbol.I64);
    }

    private IrExpressionLiteral LiteralBoolExpression(ExpressionLiteralBoolSyntax syntax)
    {
        return new IrExpressionLiteral(syntax, TypeSymbol.Bool, syntax.Value);
    }

    private IrExpressionLiteral LiteralExpression(ExpressionLiteralSyntax syntax)
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
        // TODO: remove hardcoded type I64
        var op = new IrBinaryOperator(syntax.Operator, TypeSymbol.I64);

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

    private IrParameter Parameter(ParameterSyntax param)
    {
        var name = new ParameterSymbol(param.Name.Text);
        var type = Type(param.Type);
        return new IrParameter(param, name, type);
    }

    private IrType Type(TypeSyntax type)
    {
        var name = new TypeSymbol(type.Name.Text);
        return new IrType(type, name);
    }

    private IEnumerable<IrImport> UseImports(IEnumerable<UseImportSyntax> syntax)
    {
        var imports = new List<IrImport>();

        foreach (var useImp in syntax)
        {
            var import = new IrImport(useImp);
            imports.Add(import);
        }

        return imports;
    }

    private IEnumerable<IrExport> PublicExports(IEnumerable<PublicExportSyntax> syntax)
    {
        var exports = new List<IrExport>();

        foreach (var pubExp in syntax)
        {
            var names = PublicExport(pubExp);
            exports.AddRange(names);
        }

        return exports;
    }

    private IEnumerable<IrExport> PublicExport(PublicExportSyntax syntax)
    {
        var exports = new List<IrExport>();
        foreach (var qn in syntax.QualifiedNames)
            exports.Add(new IrExport(qn));

        return exports;
    }
}
