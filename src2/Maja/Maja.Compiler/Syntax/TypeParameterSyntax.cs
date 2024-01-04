using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Common base class for type parameters.
/// </summary>
public abstract class TypeParameterSyntax : SyntaxNode
{
    protected TypeParameterSyntax(string text)
        : base(text)
    { }

    /// <summary>
    /// The name of the type parameter.
    /// </summary>
    public TypeSyntax Type
        => ChildNodes.OfType<TypeSyntax>().First();

    /// <summary>
    /// The default type of the parameter, if specified.
    /// </summary>
    public TypeSyntax? DefaultType
        => ChildNodes.OfType<TypeSyntax>().Skip(1).SingleOrDefault();
}

/// <summary>
/// A generic type parameter (.NET).
/// </summary>
public sealed class TypeParameterGenericSyntax : TypeParameterSyntax, ICreateSyntaxNode<TypeParameterGenericSyntax>
{
    private TypeParameterGenericSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.GenericTypeParameter;

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnTypeParameterGeneric(this);

    public static TypeParameterGenericSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}

/// <summary>
/// A template type parameter (#)
/// </summary>
public sealed class TypeParameterTemplateSyntax : TypeParameterSyntax, ICreateSyntaxNode<TypeParameterTemplateSyntax>
{
    private TypeParameterTemplateSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.TemplateTypeParameter;

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnTypeParameterTemplate(this);

    public static TypeParameterTemplateSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}

/// <summary>
/// A scalar value type parameter.
/// </summary>
public sealed class TypeParameterValueSyntax : TypeParameterSyntax, ICreateSyntaxNode<TypeParameterValueSyntax>
{
    private TypeParameterValueSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.ValueTypeParameter;

    public ExpressionSyntax? Expression
        => ChildNodes.OfType<ExpressionSyntax>().SingleOrDefault();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnTypeParameterValue(this);

    public static TypeParameterValueSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}
