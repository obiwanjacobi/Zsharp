﻿using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record CodeBlockSyntax : SyntaxNode
{
    public CodeBlockSyntax(string text)
        : base(text)
    { }

    public IEnumerable<MemberDeclarationSyntax> Members
        => Children.OfType<MemberDeclarationSyntax>();

    public IEnumerable<StatementSyntax> Statements
        => Children.OfType<StatementSyntax>();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnCodeBlock(this);
}
