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
    /// The assignment operator(s) used.
    /// </summary>
    public OperatorAssignmentSyntax? AssignmentOperator
        => ChildNodes.OfType<OperatorAssignmentSyntax>().SingleOrDefault();

    /// <summary>
    /// The initialization expression, if specified.
    /// </summary>
    public ExpressionSyntax? Expression
        => ChildNodes.OfType<ExpressionSyntax>().SingleOrDefault();
}

/// <summary>
/// Represents a variable declaration with an explicit type.
/// </summary>
public sealed class DeclarationVariableTypedSyntax : DeclarationVariableSyntax, ICreateSyntaxNode<DeclarationVariableTypedSyntax>
{
    private DeclarationVariableTypedSyntax(string text)
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

    public static DeclarationVariableTypedSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
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
public sealed class DeclarationVariableInferredSyntax : DeclarationVariableSyntax, ICreateSyntaxNode<DeclarationVariableInferredSyntax>
{
    private DeclarationVariableInferredSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.InferredVariableDeclaration;

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnVariableDeclarationInferred(this);

    public static DeclarationVariableInferredSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}