using Maja.Compiler.Syntax;

namespace Maja.Compiler.Symbol;

public sealed record DiscardSymbol : Symbol
{
    public DiscardSymbol()
        : base(SyntaxToken.Discard)
    { }

    public override SymbolKind Kind
        => SymbolKind.Discard;
}
