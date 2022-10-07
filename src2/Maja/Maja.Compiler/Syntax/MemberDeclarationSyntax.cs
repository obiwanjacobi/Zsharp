namespace Maja.Compiler.Syntax;

public abstract record MemberDeclarationSyntax : SyntaxNode
{
    protected MemberDeclarationSyntax(string text)
        : base(text)
    { }
}