using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Symbol;

[DebuggerDisplay("{DebuggerDisplay()}")]
public sealed record SymbolName
{
    private SymbolName(SymbolNamespace ns, string name, string originalName)
    {
        Namespace = ns;
        Value = name;
        OriginalName = originalName;
    }

    public SymbolName(string fullName, bool isType = false)
    {
        var parts = fullName.Split(SyntaxToken.Separator);

        var name = parts[^1];
        var nsParts = parts[..^1];

        Namespace = nsParts.Any() ? new SymbolNamespace(nsParts) : SymbolNamespace.Empty;
        Value = isType ? name : ToCanonical(name);
        OriginalName = name;
    }

    public SymbolName(string ns, string name, bool isType = false)
    {
        Namespace = String.IsNullOrEmpty(ns) ? SymbolNamespace.Empty : new SymbolNamespace(ns);
        Value = isType ? name : ToCanonical(name);
        OriginalName = name;
    }

    public SymbolName(IEnumerable<string> nsParts, string name, bool isType = false)
    {
        Namespace = nsParts.Any() ? new SymbolNamespace(nsParts) : SymbolNamespace.Empty;
        Value = isType ? name : ToCanonical(name);
        OriginalName = name;
    }

    internal static SymbolName InternalName(string name)
        => new(SymbolNamespace.Empty, name, name);

    public SymbolNamespace Namespace { get; }
    public string Value { get; }
    public string OriginalName { get; }

    public string FullName
        => String.IsNullOrEmpty(Namespace.Value)
                ? Value
                : $"{Namespace.Value}.{Value}";

    public string FullOriginalName
        => String.IsNullOrEmpty(Namespace.OriginalName)
                ? OriginalName
                : $"{Namespace.OriginalName}.{OriginalName}";

    // returns negative if no match. Zero and positive is a match
    // The 'this' instance contains the full-name
    public int MatchesWith(SymbolName partialName)
    {
        var fullName = this;
        // fullName: namespace.module.name
        // -match partialName:
        // namespace.module.name
        // module.name
        // name

        if (fullName.Value == partialName.Value)
        {
            if (partialName.Namespace.NameParts.Count > 0 &&
                partialName.Namespace.NameParts.Count <= fullName.Namespace.NameParts.Count)
            {
                var i = partialName.Namespace.NameParts.Count - 1;
                for (; i >= 0; i--)
                {
                    if (partialName.Namespace.NameParts[i] != fullName.Namespace.NameParts[i])
                        return -1;
                }
                return i + 1;
            }
            return 0;
        }
        return -1;
    }

    internal string DebuggerDisplay()
        => FullName;

    public override string ToString()
        => FullName;

    public static SymbolName Empty
        => new(String.Empty);

    public SymbolNamespace ToNamespace()
        => new(FullOriginalName);

    internal static string ToCanonical(string text)
    {
        Debug.Assert(!text.Contains(SyntaxToken.Separator));

        if (String.IsNullOrEmpty(text))
            return text;

        // remove discards '_'
        var canonical = text.Replace(SyntaxToken.Discard, String.Empty);
        if (!String.IsNullOrEmpty(canonical))
        {
            // preserve casing of first letter
            return canonical[0] + canonical[1..].ToLowerInvariant();
        }

        return String.Empty;
    }

    internal static IEnumerable<string> ToCanonical(IEnumerable<string> parts)
        => parts.Select(p => ToCanonical(p));

    internal static string Join(IEnumerable<string> parts)
        => String.Join(SyntaxToken.Separator, parts);
}
