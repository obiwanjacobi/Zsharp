using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record FunctionDelcarationSyntax : MemberDeclarationSyntax
{
    public FunctionDelcarationSyntax(string text)
        : base(text)
    { }

    public NameSyntax Identifier
        => Children.OfType<NameSyntax>().Single();

    public IEnumerable<ParameterSyntax> Parameters
        => Children.OfType<ParameterSyntax>();

    public TypeSyntax? ReturnType
        => Children.OfType<TypeSyntax>().SingleOrDefault();

    public CodeBlockSyntax CodeBlock
        => Children.OfType<CodeBlockSyntax>().Single();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnFunctionDeclaration(this);
}
