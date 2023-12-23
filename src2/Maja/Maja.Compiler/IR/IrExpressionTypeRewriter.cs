using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.IR;

// Not based on the IrRewriter because type propagation is not bottum up.
internal class IrExpressionTypeRewriter
{
    private readonly Stack<TypeSymbol> _typeStack = new();

    public IrExpressionTypeRewriter(TypeSymbol targetType)
        => _typeStack.Push(targetType);

    public IrExpression RewriteExpression(IrExpression expression)
    {
        var expr = expression switch
        {
            IrExpressionBinary exprBin => RewriteExpressionBinary(exprBin),
            //IrExpressionIdentifier exprId => RewriteExpressionIdentifier(exprId),
            IrExpressionInvocation exprInv => RewriteExpressionInvocation(exprInv),
            IrExpressionLiteral exprLit => RewriteExpressionLiteral(exprLit),
            _ => expression
        };

        return expr;
    }

    private IrExpressionBinary RewriteExpressionBinary(IrExpressionBinary expression)
    {
        // TODO: implement type propegation through binary operators.
        return expression;
    }

    private IrExpressionInvocation RewriteExpressionInvocation(IrExpressionInvocation expression)
    {
        var newArgs = expression.Arguments.Select(a => RewriteArgument(a));
        
        _typeStack.Push(expression.TypeSymbol);
        
        return new IrExpressionInvocation(expression.Syntax, expression.Symbol,
            expression.TypeArguments, newArgs, expression.TypeSymbol);
    }

    private IrArgument RewriteArgument(IrArgument argument)
    {
        var expr = RewriteExpression(argument.Expression);

        if (expr == argument.Expression)
            return argument;

        return new IrArgument(argument.Syntax, expr, argument.Symbol);
    }

    private IrExpressionLiteral RewriteExpressionLiteral(IrExpressionLiteral expression)
    {
        if (expression.TypeInferredSymbol is not null)
        {
            if (!expression.TypeInferredSymbol.TrySelectInferredType(_typeStack.Peek(), out var newType))
                throw new MajaException($"Could not determine Type for literal expression '{expression.Syntax.Text}'");

            return new IrExpressionLiteral(expression.Syntax, newType, expression.ConstantValue!);
        }

        return expression;
    }
}