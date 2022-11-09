using System.Collections.Generic;
using System.Collections.Immutable;

namespace Maja.Compiler.Symbol;

public sealed record NamespaceSymbol : Symbol
{
    public NamespaceSymbol(IEnumerable<string> nameParts)
        : base(SymbolName.Join(SymbolName.ToCanonical(nameParts)))
    {
        NameParts = SymbolName.ToCanonical(nameParts).ToImmutableArray();
        OriginalName = SymbolName.Join(nameParts);
    }

    public override SymbolKind Kind
        => SymbolKind.Namespace;

    public ImmutableArray<string> NameParts { get; }
    public string OriginalName { get; }
}
