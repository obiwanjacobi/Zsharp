using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Symbol;

public record SymbolName
{
    public SymbolName(IEnumerable<string> nsParts, string name)
    {
        Namespace = new NamespaceSymbol(nsParts);
        Name = ToCanonical(name);
        OriginalName = name;
    }

    public NamespaceSymbol Namespace { get; }
    public string Name { get; }
    public string OriginalName { get; }

    public string FullName
        => String.IsNullOrEmpty(Namespace.Name)
                ? Name
                : $"{Namespace.Name}.{Name}";


    internal static string ToCanonical(string text)
    {
        Debug.Assert(!text.Contains(SyntaxToken.Separator));

        // remove discards '_'
        var canonical = text.Replace(SyntaxToken.Discard, String.Empty);
        // preserve casing of first letter
        return canonical[0] + canonical[1..].ToLowerInvariant();
    }

    internal static IEnumerable<string> ToCanonical(IEnumerable<string> parts)
        => parts.Select(p => ToCanonical(p));

    internal static string Join(IEnumerable<string> parts)
        => String.Join(SyntaxToken.Separator, parts);
}
