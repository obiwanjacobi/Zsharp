namespace Maja.Compiler.Syntax;

/// <summary>
/// Common base class for member (function, type and variable) declarations.
/// </summary>
public abstract class MemberDeclarationSyntax : SyntaxNode
{
    protected MemberDeclarationSyntax(string text)
        : base(text)
    { }
}