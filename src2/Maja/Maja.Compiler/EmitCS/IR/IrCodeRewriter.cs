using Maja.Compiler.IR;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.EmitCS.IR;

internal sealed class IrCodeRewriter : IrRewriter
{
    public IrProgram CodeRewrite(IrProgram program)
        => RewriteProgram(program);

    protected override IrStatement RewriteStatementLoop(IrStatementLoop statement)
    {
        if (statement.Expression is IrExpressionRange rangeExpr)
        {
            var type = rangeExpr.TypeSymbol;
            var symbol = new VariableSymbol(SymbolName.InternalName("__i"), type);
            var initExpr = rangeExpr.Start;
            var initializer = new IrDeclarationVariable(symbol, type, initExpr);

            var condition = new IrExpressionBinary(
                new IrExpressionIdentifier(symbol, type),
                new IrBinaryOperator(IrBinaryOperatorKind.Lesser, type),
                rangeExpr.End!
                );
            
            var addOne = new IrExpressionBinary(
                new IrExpressionIdentifier(symbol, type),
                new IrBinaryOperator(IrBinaryOperatorKind.Add, type),
                new IrExpressionLiteral(type, 1)
                );
            var step = new IrStatementAssignment(symbol, addOne, IrLocality.None);
            
            return new IrCodeStatementForLoop(statement.Syntax, initializer, condition, step, statement.CodeBlock);
        }

        var expression = statement.Expression
            ?? new IrExpressionLiteral(TypeSymbol.Bool, true);

        return new IrCodeStatementWhileLoop(statement.Syntax, expression, statement.CodeBlock);
    }
}
