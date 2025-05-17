using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// An expression that initializes a structure and its fields
/// </summary>
public sealed class ExpressionTypeInitializerSyntax : ExpressionSyntax, ICreateSyntaxNode<ExpressionTypeInitializerSyntax>
{
    private ExpressionTypeInitializerSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.TypeInitializerExpression;

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionTypeInitializer(this);

    public TypeSyntax Type
        => ChildNodes.OfType<TypeSyntax>().Single();

    public IEnumerable<TypeInitializerFieldSyntax> FieldInitializers
        => ChildNodes.OfType<TypeInitializerFieldSyntax>();

    public static ExpressionTypeInitializerSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}
