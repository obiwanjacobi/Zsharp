using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// The module directive that names the code file
/// </summary>
public sealed class ModuleSyntax : SyntaxNode, ICreateSyntaxNode<ModuleSyntax>
{
    private ModuleSyntax(string text)
        : base(text)
    { }

    public QualifiedNameSyntax Identifier
        => ChildNodes.OfType<QualifiedNameSyntax>().Single();

    public override SyntaxKind SyntaxKind
        => SyntaxKind.ModuleDirective;

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnModule(this);

    public static ModuleSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}