using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Symbol;

[DebuggerDisplay("{DebuggerDisplay()}")]
public sealed record SymbolName
{
    public SymbolName(string name)
        : this(Enumerable.Empty<string>(), name)
    { }

    public SymbolName(string ns, string name)
    {
        Namespace = new NamespaceSymbol(ns);
        Value = ToCanonical(name);
        OriginalName = name;
    }

    public SymbolName(IEnumerable<string> nsParts, string name)
    {
        Namespace = new NamespaceSymbol(nsParts);
        Value = ToCanonical(name);
        OriginalName = name;
    }

    public NamespaceSymbol Namespace { get; }
    public string Value { get; }
    public string OriginalName { get; }

    public string FullName
        => String.IsNullOrEmpty(Namespace.Value)
                ? Value
                : $"{Namespace.Value}.{Value}";

    public int MatchesWith(SymbolName fullName)
    {
        // fullName: namespace.module.name
        // -match
        // namespace.module.name
        // module.name
        // name

        if (fullName.Value == Value)
        {
            if (Namespace.NameParts.Count > 0 &&
                Namespace.NameParts.Count <= fullName.Namespace.NameParts.Count)
            {
                var i = Namespace.NameParts.Count - 1;
                for (; i >= 0; i--)
                {
                    if (Namespace.NameParts[i] != fullName.Namespace.NameParts[i])
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
