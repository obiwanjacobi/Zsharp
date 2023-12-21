using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal class IrTypeArgument : IrNode
{
    public IrTypeArgument(TypeArgumentSyntax syntax, IrType type)
        : base(syntax)
    {
        Type = type;
    }

    public IrType Type { get; }

    public new TypeArgumentSyntax Syntax
        => (TypeArgumentSyntax)base.Syntax;
}