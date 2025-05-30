using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Symbol;

[DebuggerDisplay("{DebuggerDisplay()}")]
public sealed record SymbolNamespace
{
    private SymbolNamespace()
    {
        Value = String.Empty;
        NameParts = Array.Empty<string>().ToImmutableList();
        OriginalName = String.Empty;
    }

    public SymbolNamespace(IEnumerable<string> nameParts)
    {
        var canonicalParts = SymbolName.ToCanonical(nameParts);
        Value = SymbolName.Join(canonicalParts);
        NameParts = canonicalParts.ToImmutableList().WithValueSemantics();
        OriginalName = SymbolName.Join(nameParts);
    }

    public SymbolNamespace(string ns)
        : this(ns.Split(SyntaxToken.Separator))
    { }

    public SymbolKind Kind
        => SymbolKind.Namespace;

    public string Value { get; }
    public IImmutableList<string> NameParts { get; }
    public string OriginalName { get; }

    internal string DebuggerDisplay()
        => ToString();
    public override string ToString()
        => String.IsNullOrEmpty(Value) ? String.Empty : $"{Value} ({OriginalName})";

    public static SymbolNamespace Empty
        => new();
}
