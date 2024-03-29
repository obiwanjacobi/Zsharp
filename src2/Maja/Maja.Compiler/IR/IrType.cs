﻿using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrType : IrNode
{
    public IrType(TypeSyntax syntax, TypeSymbol symbol)
        : base(syntax)
    {
        Symbol = symbol;
    }

    public TypeSymbol Symbol { get; }

    public new TypeSyntax Syntax
        => (TypeSyntax)base.Syntax;

    private static readonly IrType _void = new(new TypeSyntax("void"), TypeSymbol.Void);
    public static IrType Void
        => _void;
}