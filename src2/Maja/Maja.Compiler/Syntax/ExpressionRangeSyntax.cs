using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed class ExpressionRangeSyntax : ExpressionSyntax, ICreateSyntaxNode<ExpressionRangeSyntax>
{
    public ExpressionRangeSyntax(string text)
        : base(text)
    { }

    // expects children to be:
    // [-token, startNode?, range-token, endNode?, ]-token

    public ExpressionSyntax? Start
        => Children[1].Node as ExpressionSyntax;

    public ExpressionSyntax? End
        => Children[2].Token is PunctuationToken
            ? Children[3].Node as ExpressionSyntax
            : Children[2].Node as ExpressionSyntax;

    public override SyntaxKind SyntaxKind
        => SyntaxKind.RangeExpression;

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionRange(this);
    public static ExpressionRangeSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}
