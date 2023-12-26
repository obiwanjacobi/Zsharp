using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed class TypeInitializerFieldSyntax : SyntaxNode, ICreateSyntaxNode<TypeInitializerFieldSyntax>
{
    private TypeInitializerFieldSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.TypeInitializerField;

    public NameSyntax Name
        => ChildNodes.OfType<NameSyntax>().Single();

    public ExpressionSyntax Expression
        => ChildNodes.OfType<ExpressionSyntax>().Single();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnTypeInitializerField(this);

    public static TypeInitializerFieldSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}
