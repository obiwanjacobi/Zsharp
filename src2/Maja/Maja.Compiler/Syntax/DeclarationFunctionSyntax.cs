﻿using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a function declaration.
/// </summary>
public sealed class DeclarationFunctionSyntax : DeclarationMemberSyntax, ICreateSyntaxNode<DeclarationFunctionSyntax>
{
    private DeclarationFunctionSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.FunctionDeclaration;

    /// <summary>
    /// The name of the declared function.
    /// </summary>
    public NameSyntax Name
        => ChildNodes.OfType<NameSyntax>().Single();

    /// <summary>
    /// A collection of type parameters, if any.
    /// </summary>
    public IEnumerable<TypeParameterSyntax> TypeParameters
        => ChildNodes.OfType<TypeParameterSyntax>();

    /// <summary>
    /// A collection of parameters, if any.
    /// </summary>
    public IEnumerable<ParameterSyntax> Parameters
        => ChildNodes.OfType<ParameterSyntax>();

    /// <summary>
    /// The return type of the function, if specified.
    /// </summary>
    public TypeSyntax? ReturnType
        => ChildNodes.OfType<TypeSyntax>().SingleOrDefault();

    /// <summary>
    /// The function body.
    /// </summary>
    public CodeBlockSyntax CodeBlock
        => ChildNodes.OfType<CodeBlockSyntax>().Single();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnFunctionDeclaration(this);
    public static DeclarationFunctionSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}
