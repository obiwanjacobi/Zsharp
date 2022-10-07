using System.Linq;

namespace Maja.Compiler.Syntax;

public abstract record ExpressionConstSyntax : ExpressionSyntax
{ }

public record ExpressionLiteralSyntax : ExpressionConstSyntax
{
    public LiteralNumberSyntax? LiteralNumber
        => Children.OfType<LiteralNumberSyntax>().SingleOrDefault();

    public LiteralStringSyntax? LiteralString
        => Children.OfType<LiteralStringSyntax>().SingleOrDefault();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionLiteral(this);
}

public record ExpressionLiteralBoolSyntax : ExpressionConstSyntax
{
    public ExpressionLiteralBoolSyntax(string value)
    {
        Text = value;
        Value = value.ToUpper() == "TRUE";
    }

    public string Text { get; }

    public bool Value { get; }

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionLiteralBool(this);
}