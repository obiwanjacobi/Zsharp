using System.Collections.Generic;
using System.Collections.Immutable;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal abstract class IrDeclaration : IrNode
{
    public IrDeclaration(SyntaxNode syntax)
        : base(syntax)
    { }
}

internal sealed class IrFunctionDeclaration : IrDeclaration
{
    public IrFunctionDeclaration(SyntaxNode syntax, IEnumerable<IrParameter> prms, IrType? type, IrScope scope)
        : base(syntax)
    {
        Parameters = prms.ToImmutableArray();
        Type = type;
        Body = scope;
    }

    public ImmutableArray<IrParameter> Parameters { get; }
    public IrType? Type { get; }
    public IrScope Body { get; }
}

internal sealed class IrVariableDeclaration : IrDeclaration
{
    public IrVariableDeclaration(SyntaxNode syntax, VariableSymbol symbol, TypeSymbol type, IrExpression? initializer)
        : base(syntax)
    {
        Symbol = symbol;
        Type = type;
        Initializer = initializer;
    }

    public VariableSymbol Symbol { get; }
    public TypeSymbol Type { get; }
    public IrExpression? Initializer { get; }
}

internal sealed class IrTypeDeclaration : IrDeclaration
{
    public IrTypeDeclaration(SyntaxNode syntax)
        : base(syntax)
    { }
}