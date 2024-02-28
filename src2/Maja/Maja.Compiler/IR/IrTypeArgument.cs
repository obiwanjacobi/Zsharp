using System.Collections.Generic;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal class IrTypeArgument : IrNode, IrContainer
{
    public IrTypeArgument(TypeArgumentSyntax syntax, IrType type)
        : base(syntax)
    {
        Type = type;
    }

    public IrType Type { get; }

    public new TypeArgumentSyntax Syntax
        => (TypeArgumentSyntax)base.Syntax;

    public IEnumerable<T> GetDescendentsOfType<T>() where T : IrNode
        => Type.GetDescendentsOfType<T>();
}