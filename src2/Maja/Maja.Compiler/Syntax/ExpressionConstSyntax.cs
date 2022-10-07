using System.Linq;

namespace Maja.Compiler.Syntax;

public abstract record ExpressionConstSyntax : ExpressionSyntax
{
    protected ExpressionConstSyntax(string text, bool precedence = false)
        : base(text, precedence)
    { }
}

public record ExpressionLiteralSyntax : ExpressionConstSyntax
{
    public ExpressionLiteralSyntax(string text)
        : base(text, false)
    { }

    public LiteralNumberSyntax? LiteralNumber
        => Children.OfType<LiteralNumberSyntax>().SingleOrDefault();

    public LiteralStringSyntax? LiteralString
        => Children.OfType<LiteralStringSyntax>().SingleOrDefault();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionLiteral(this);
}

public record ExpressionLiteralBoolSyntax : ExpressionConstSyntax
{
    public ExpressionLiteralBoolSyntax(string text)
        : base(text, false)
    {
        Value = text.ToUpper() == "TRUE";
    }

    public bool Value { get; }

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionLiteralBool(this);
}