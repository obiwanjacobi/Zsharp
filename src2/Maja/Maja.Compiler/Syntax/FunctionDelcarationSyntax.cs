using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a function declaration.
/// </summary>
public sealed record FunctionDelcarationSyntax : MemberDeclarationSyntax
{
    public FunctionDelcarationSyntax(string text)
        : base(text)
    { }

    /// <summary>
    /// The name of the declared function.
    /// </summary>
    public NameSyntax Identifier
        => Children.OfType<NameSyntax>().Single();

    /// <summary>
    /// A collection of parameters, if any.
    /// </summary>
    public IEnumerable<ParameterSyntax> Parameters
        => Children.OfType<ParameterSyntax>();

    /// <summary>
    /// The return type of the function, if specified.
    /// </summary>
    public TypeSyntax? ReturnType
        => Children.OfType<TypeSyntax>().SingleOrDefault();

    /// <summary>
    /// The function body.
    /// </summary>
    public CodeBlockSyntax CodeBlock
        => Children.OfType<CodeBlockSyntax>().Single();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnFunctionDeclaration(this);
}
