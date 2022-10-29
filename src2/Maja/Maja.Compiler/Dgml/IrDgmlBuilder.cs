using System;
using Maja.Compiler.IR;
using Maja.Dgml;

namespace Maja.Compiler.Dgml;

internal sealed class IrDgmlBuilder
{
    private readonly DgmlBuilder _builder = new("IR");

    public IrDgmlBuilder()
    { }

    public static void Save(IrNode node, string filePath = "interrep.dgml")
    {
        var builder = new IrDgmlBuilder();
        builder.WriteNode(node);
        builder._builder.SaveAs(filePath);
    }

    public Node WriteNode(IrNode irNode)
    {
        return irNode switch
        {
            IrDeclaration decl => WriteDeclaration(decl),
            IrExpression expr => WriteExpression(expr),
            IrStatement stat => WriteStatement(stat),
            _ => throw new NotSupportedException($"Dgml: No support for Node {irNode.Syntax.Text}.")
        };
    }

    private Node WriteStatement(IrStatement stat)
    {
        return stat switch
        {
            IrStatementIf si => WriteIf(si),
            IrStatementLoop lp => WriteLoop(lp),
            _ => throw new NotSupportedException($"Dgml: No support for Statement {stat.Syntax.Text}.")
        };
    }

    private Node WriteLoop(IrStatementLoop loop)
    {
        var typeName = loop.GetType().Name;
        var label = loop.Syntax.Text;
        var node = _builder.CreateNode(typeName, label, typeName);
        return node;
    }

    private Node WriteIf(IrStatementIf statIf)
    {
        var typeName = statIf.GetType().Name;
        var label = "if"; //statIf.Condition.Syntax.Text;
        var nodeIf = _builder.CreateNode(typeName, label, typeName);

        var nodeExpr = WriteExpression(statIf.Condition);
        _builder.CreateLink(nodeIf.Id, nodeExpr.Id);
        return nodeIf;
    }

    private Node WriteExpression(IrExpression expr)
    {
        return expr switch
        {
            IrExpressionBinary be => WriteBinaryExpression(be),
            IrExpressionLiteral le => WriteLiteralExpression(le),
            IrExpressionInvocation ie => WriteInvocationExpression(ie),
            _ => throw new NotSupportedException($"Dgml: No support for Expression {expr.Syntax.Text}.")
        };
    }

    private Node WriteBinaryExpression(IrExpressionBinary binExpr)
    {
        var nodeL = WriteExpression(binExpr.Left);
        var nodeR = WriteExpression(binExpr.Right);
        var nodeOp = WriteBinaryOp(binExpr.Op);

        var typeName = binExpr.GetType().Name;
        var nodeBin = _builder.CreateNode(typeName, binExpr.Op.Syntax.Text, typeName);
        _builder.CreateLink(nodeBin.Id, nodeL.Id);
        _builder.CreateLink(nodeBin.Id, nodeOp.Id);
        _builder.CreateLink(nodeBin.Id, nodeR.Id);

        return nodeBin;
    }

    private Node WriteBinaryOp(IrBinaryOperator op)
    {
        var typeName = op.GetType().Name;
        return _builder.CreateNode(typeName, op.Syntax.Text, typeName);
    }

    private Node WriteLiteralExpression(IrExpressionLiteral litExpr)
    {
        var typeName = litExpr.GetType().Name;
        return _builder.CreateNode(typeName, litExpr.Syntax.Text, typeName);
    }

    private Node WriteInvocationExpression(IrExpressionInvocation invocExpr)
    {
        var typeName = invocExpr.GetType().Name;
        return _builder.CreateNode(typeName, invocExpr.Syntax.Text, typeName);
    }

    private Node WriteDeclaration(IrDeclaration decl)
    {
        return decl switch
        {
            IrFunctionDeclaration fd => WriteFunctionDeclaration(fd),
            IrVariableDeclaration vd => WriteVariableDeclaration(vd),
            IrTypeDeclaration td => WriteTypeDeclaration(td),
            _ => throw new NotSupportedException($"Dgml: No support for Declaration {decl.Syntax.Text}.")
        };
    }

    private Node WriteFunctionDeclaration(IrFunctionDeclaration funcDecl)
    {
        var typeName = funcDecl.GetType().Name;
        var nodeFun = _builder.CreateNode(typeName, funcDecl.Syntax.Identifier.Text, typeName);

        // TODO: params etc.

        return nodeFun;
    }

    private Node WriteVariableDeclaration(IrVariableDeclaration varDecl)
    {
        throw new NotImplementedException();
    }

    private Node WriteTypeDeclaration(IrTypeDeclaration typeDecl)
    {
        throw new NotImplementedException();
    }
}
