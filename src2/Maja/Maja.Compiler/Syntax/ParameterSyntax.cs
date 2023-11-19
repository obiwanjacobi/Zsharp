using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a parameter in a function declaration.
/// </summary>
public sealed class ParameterSyntax : SyntaxNode, ICreateSyntaxNode<ParameterSyntax>
{
    private ParameterSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.FunctionParameter;

    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public NameSyntax Name
        => ChildNodes.OfType<NameSyntax>().Single();

    /// <summary>
    /// The type of the parameter.
    /// </summary>
    public TypeSyntax Type
        => ChildNodes.OfType<TypeSyntax>().Single();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnParameter(this);
    
    public static ParameterSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location, 
            Children = children, 
            ChildNodes = childNodes, 
            TrailingTokens = trailingTokens
        };
}
