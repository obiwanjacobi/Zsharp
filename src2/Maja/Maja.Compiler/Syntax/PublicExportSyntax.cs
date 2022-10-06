﻿using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record PublicExportSyntax : SyntaxNode
{
    public IEnumerable<QualifiedNameSyntax> QualifiedNames
        => Children.OfType<QualifiedNameSyntax>();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnPublicExport(this);
}