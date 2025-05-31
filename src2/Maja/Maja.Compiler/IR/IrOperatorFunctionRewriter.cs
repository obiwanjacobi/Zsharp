using Maja.Compiler.External;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrOperatorFunctionRewriter : IrRewriter
{
    private readonly IExternalModuleLoader _moduleLoader;

    public IrOperatorFunctionRewriter(IExternalModuleLoader moduleLoader)
    {
        _moduleLoader = moduleLoader;
    }

    public IrModule Resolve(IrModule module)
    {
        return RewriteModule(module);
    }

    protected override IrExpression RewriteExpressionBinary(IrExpressionBinary expression)
    {
        if (_moduleLoader.TryLookupOperator(
            expression.Operator.Syntax.Text,
            expression.Operator.TargetType,
            [expression.Operator.OperandType, expression.Operator.OperandType],
            out var functionSymbol))
        {
            // change the binary expression into an invocation expression (fake syntax)
            var argSyntax1 = ArgumentSyntax.Create(expression.Left.Syntax.Text, expression.Left.Syntax.Location,
                SyntaxNodeOrTokenList.Empty, SyntaxNodeList.Empty, SyntaxTokenList.Empty);
            var argSyntax2 = ArgumentSyntax.Create(expression.Right.Syntax.Text, expression.Right.Syntax.Location,
                SyntaxNodeOrTokenList.Empty, SyntaxNodeList.Empty, SyntaxTokenList.Empty);
            var nameSyntax = NameSyntax.Create(functionSymbol.Name.FullName, expression.Syntax.Location,
                SyntaxNodeOrTokenList.Empty, SyntaxNodeList.Empty, SyntaxTokenList.Empty);
            var funcNameSyntax = ExpressionIdentifierSyntax.Create(functionSymbol.Name.FullName, expression.Syntax.Location,
                SyntaxNodeOrTokenList.New([new SyntaxNodeOrToken(nameSyntax)]), SyntaxNodeList.New([nameSyntax]), SyntaxTokenList.Empty);
            var invokeSyntax = ExpressionInvocationSyntax.Create(expression.Syntax.Text, expression.Syntax.Location,
                SyntaxNodeOrTokenList.New([new SyntaxNodeOrToken(funcNameSyntax), new SyntaxNodeOrToken(argSyntax1), new SyntaxNodeOrToken(argSyntax2)]),
                SyntaxNodeList.New([funcNameSyntax, argSyntax1, argSyntax2]), SyntaxTokenList.Empty);

            var arguments = new[] {
                new IrArgument(argSyntax1, expression.Left, null),
                new IrArgument(argSyntax2, expression.Right, null),
            };
            return new IrExpressionInvocation(invokeSyntax, functionSymbol, [], arguments, expression.TypeSymbol);
        }

        return base.RewriteExpressionBinary(expression);
    }
}
