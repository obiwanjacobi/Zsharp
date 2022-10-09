using System.Linq;

namespace Maja.Compiler.Syntax;

public record ExpressionSyntax : SyntaxNode
{
    public ExpressionSyntax(string text)
        : base(text)
    { }

    public bool Precedence { get; internal set; }

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpression(this);
}
