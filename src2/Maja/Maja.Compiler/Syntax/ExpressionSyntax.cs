namespace Maja.Compiler.Syntax;

public record ExpressionSyntax : SyntaxNode
{
}

public record ExpressionConstSyntax : ExpressionSyntax
{

}

public record ExpressionLiteralSyntax : ExpressionConstSyntax
{
    public ExpressionLiteralSyntax(string value)
    {
        Value = value;
    }

    public string Value { get; }
}

public record ExpressionLiteralBoolSyntax : ExpressionConstSyntax
{

}