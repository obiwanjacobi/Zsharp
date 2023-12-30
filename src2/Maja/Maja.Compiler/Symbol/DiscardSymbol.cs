using Maja.Compiler.Syntax;

namespace Maja.Compiler.Symbol;

public sealed record DiscardSymbol : VariableSymbol
{
    public DiscardSymbol()
        : base(new SymbolName(SyntaxToken.Discard), TypeSymbol.Unknown)
    { }

    public static bool IsDiscard(string variableName)
        => variableName.Length == 1 &&
            variableName == SyntaxToken.Discard;

    public override SymbolKind Kind
        => SymbolKind.Discard;
}
