﻿using System.Collections.Generic;
using System.Collections.Immutable;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrCodeBlock : IrNode, IrContainer
{
    public IrCodeBlock(CodeBlockSyntax syntax, IEnumerable<IrNode> nodes
        //IEnumerable<IrStatement> statements, IEnumerable<IrDeclaration> declarations
        )
        : base(syntax)
    {
        Nodes = nodes.ToImmutableArray();
        //Statements = statements.ToImmutableArray();
        //Declarations = declarations.ToImmutableArray();
    }

    public ImmutableArray<IrNode> Nodes { get; }

    //public ImmutableArray<IrStatement> Statements { get; }
    //public ImmutableArray<IrDeclaration> Declarations { get; }

    public new CodeBlockSyntax Syntax
        => (CodeBlockSyntax)base.Syntax;

    //public IEnumerable<T> GetDescendantsOfType<T>() where T : IrNode
    //    => Statements.GetDescendantsOfType<T>()
    //        .Concat(Declarations.GetDescendantsOfType<T>());
    public IEnumerable<T> GetDescendantsOfType<T>() where T : IrNode
        => Nodes.GetDescendantsOfType<T>();
}