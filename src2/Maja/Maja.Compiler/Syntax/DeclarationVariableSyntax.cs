using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Common base class for a variable declaration.
/// </summary>
public abstract class DeclarationVariableSyntax : DeclarationMemberSyntax
{
    protected DeclarationVariableSyntax(string text)
        : base(text)
    { }

    /// <summary>
    /// The name or identifier of the variable.
    /// </summary>
    public NameSyntax Name
        => ChildNodes.OfType<NameSyntax>().Single();

    /// <summary>
    /// The initialization expression, if specified.
    /// </summary>
    public ExpressionSyntax? Expression
        => ChildNodes.OfType<ExpressionSyntax>().SingleOrDefault();
}

/// <summary>
/// Represents a variable declaration with an explicit type.
/// </summary>
public sealed class VariableDeclarationTypedSyntax : DeclarationVariableSyntax, ICreateSyntaxNode<VariableDeclarationTypedSyntax>
{
    private VariableDeclarationTypedSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.TypedVariableDeclaration;

    /// <summary>
    /// The type specified for the variable.
    /// </summary>
    public TypeSyntax Type
        => ChildNodes.OfType<TypeSyntax>().Single();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnVariableDeclarationTyped(this);

    public static VariableDeclarationTypedSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}

/// <summary>
/// Represents a variable declaration where it's type is to be inferred.
/// Expression is set.
/// </summary>
public sealed class VariableDeclarationInferredSyntax : DeclarationVariableSyntax, ICreateSyntaxNode<VariableDeclarationInferredSyntax>
{
    private VariableDeclarationInferredSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.InferredVariableDeclaration;

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnVariableDeclarationInferred(this);

    public static VariableDeclarationInferredSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}