using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal abstract class IrTypeParameter : IrNode
{
    protected IrTypeParameter(TypeParameterSyntax syntax)
        : base(syntax)
    { }
}

internal sealed class IrTypeParameterGeneric : IrTypeParameter
{
    public IrTypeParameterGeneric(TypeParameterGenericSyntax syntax, IrType type)
        : base(syntax)
    {
        Type = type;
    }

    public IrType Type { get; }
}