using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record FunctionDelcarationSyntax: MemberDeclarationSyntax
{
    public NameSyntax Identifier
        => Children.OfType<NameSyntax>().Single();

    public CodeBlockSyntax CodeBlock
        => Children.OfType<CodeBlockSyntax>().Single();
}
