using System;
using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Symbol;

public record SymbolName
{
    public SymbolName(IEnumerable<string> nsParts, string name)
    {
        Namespace = new NamespaceSymbol(nsParts);
        Name = name;
        CanonicalName = ToCanonical(name);
    }

    public NamespaceSymbol Namespace { get; }
    public string Name { get; }
    public string CanonicalName { get; }

    public string FullName
        => String.IsNullOrEmpty(Namespace.Name)
                ? Name
                : $"{Namespace.Name}.{Name}";

    internal static string ToCanonical(string text)
    {
        // remove discards '_'
        var canonical = text.Replace("_", String.Empty);
        // preserve casing of first letter
        return canonical[0] + canonical[1..].ToLowerInvariant();
    }

    internal static IEnumerable<string> ToCanonical(IEnumerable<string> parts)
        => parts.Select(p => ToCanonical(p));

    internal static string Join(IEnumerable<string> parts)
        => String.Join(".", parts);
}
