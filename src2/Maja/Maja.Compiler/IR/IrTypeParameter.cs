using System.Collections.Generic;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal abstract class IrTypeParameter : IrNode
{
    protected IrTypeParameter(TypeParameterSyntax syntax, TypeParameterSymbol symbol)
        : base(syntax)
    {
        Symbol = symbol;
    }

    public TypeParameterSymbol Symbol { get; }

    public new TypeParameterSyntax Syntax
        => (TypeParameterSyntax)base.Syntax;
}

internal sealed class IrTypeParameterGeneric : IrTypeParameter, IrContainer
{
    public IrTypeParameterGeneric(TypeParameterGenericSyntax syntax, IrType? type, TypeParameterSymbol symbol)
        : base(syntax, symbol)
    {
        Type = type;
    }

    public IrType? Type { get; }

    public new TypeParameterGenericSyntax Syntax
        => (TypeParameterGenericSyntax)base.Syntax;

    public IEnumerable<T> GetDescendentsOfType<T>() where T : IrNode
        => Type.GetDescendentsOfType<T>();
}

internal sealed class IrTypeParameterTemplate : IrTypeParameter, IrContainer
{
    public IrTypeParameterTemplate(TypeParameterTemplateSyntax syntax, IrType? type, TypeParameterSymbol symbol)
        : base(syntax, symbol)
    {
        Type = type;
    }

    public IrType? Type { get; }

    public new TypeParameterTemplateSyntax Syntax
        => (TypeParameterTemplateSyntax)base.Syntax;

    public IEnumerable<T> GetDescendentsOfType<T>() where T : IrNode
        => Type.GetDescendentsOfType<T>();
}