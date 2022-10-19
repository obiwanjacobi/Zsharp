using System.Linq;

namespace Maja.Compiler.Syntax;

public abstract record ExpressionConstantSyntax : ExpressionSyntax
{
    protected ExpressionConstantSyntax(string text)
        : base(text)
    { }
}

public record ExpressionLiteralSyntax : ExpressionConstantSyntax
{
    public ExpressionLiteralSyntax(string text)
        : base(text)
    { }

    public LiteralNumberSyntax? LiteralNumber
        => Children.OfType<LiteralNumberSyntax>().SingleOrDefault();

    public LiteralStringSyntax? LiteralString
        => Children.OfType<LiteralStringSyntax>().SingleOrDefault();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionLiteral(this);
}

public record ExpressionLiteralBoolSyntax : ExpressionConstantSyntax
{
    public ExpressionLiteralBoolSyntax(string text)
        : base(text)
    {
        Value = text == "true";
    }

    public bool Value { get; }

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionLiteralBool(this);
}