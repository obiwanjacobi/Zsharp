using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Common base class for a variable declaration.
/// </summary>
public abstract record VariableDeclarationSyntax : MemberDeclarationSyntax
{
    public VariableDeclarationSyntax(string text)
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
public sealed record VariableDeclarationTypedSyntax : VariableDeclarationSyntax
{
    public VariableDeclarationTypedSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.TypedVariableDeclaration;

    /// <summary>
    /// The type specified for the variable.
    /// </summary>
    public TypeSyntax Type
        => ChildNodes.OfType<TypeSyntax>().Single();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnVariableDeclarationTyped(this);
}

/// <summary>
/// Represents a variable declaration where it's type is to be inferred.
/// Expression is set.
/// </summary>
public sealed record VariableDeclarationInferredSyntax : VariableDeclarationSyntax
{
    public VariableDeclarationInferredSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.InferredVariableDeclaration;

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnVariableDeclarationInferred(this);
}