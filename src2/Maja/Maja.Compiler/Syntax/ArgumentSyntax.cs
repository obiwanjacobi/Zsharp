using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// An argument for a function parameter.
/// </summary>
public sealed class ArgumentSyntax : SyntaxNode, ICreateSyntaxNode<ArgumentSyntax>
{
    private ArgumentSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.FunctionArgument;

    /// <summary>
    /// Optional parameter name the argument specifies a value for.
    /// </summary>
    public NameSyntax? Name
        => ChildNodes.OfType<NameSyntax>().SingleOrDefault();

    /// <summary>
    /// The expression that represent the argument.
    /// </summary>
    public ExpressionSyntax Expression
        => ChildNodes.OfType<ExpressionSyntax>().Single();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnArgument(this);
    
    public static ArgumentSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location, 
            Children = children, 
            ChildNodes = childNodes, 
            TrailingTokens = trailingTokens
        };
}