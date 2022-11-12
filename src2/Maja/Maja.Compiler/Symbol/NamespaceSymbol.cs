using System.Collections.Generic;
using System.Collections.Immutable;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Symbol;

public sealed record NamespaceSymbol //: Symbol
{
    public NamespaceSymbol(IEnumerable<string> nameParts)
    {
        var canonicalParts = SymbolName.ToCanonical(nameParts);
        Value = SymbolName.Join(canonicalParts);
        NameParts = canonicalParts.ToImmutableList().WithValueSemantics();
        OriginalName = SymbolName.Join(nameParts);
    }

    public NamespaceSymbol(string ns)
        : this(ns.Split(SyntaxToken.Separator))
    { }

    public SymbolKind Kind
        => SymbolKind.Namespace;

    public string Value { get; }
    public IImmutableList<string> NameParts { get; }
    public string OriginalName { get; }
}
