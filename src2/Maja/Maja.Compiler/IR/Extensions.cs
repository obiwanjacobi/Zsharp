using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal static class Extensions
{
    public static SymbolName ToSymbolName(this QualifiedNameSyntax syntax)
    {
        var nameNode = syntax.ChildNodes[^1];
        var nsNodes = syntax.ChildNodes[..^1];
        return new SymbolName(nsNodes.Select(n => n.Text), nameNode.Text);
    }

    public static bool IsExactly<T>(this IEnumerable<T> objects, T theOne)
        where T : class
    {
        bool success = false;

        foreach (var obj in objects)
            success = obj == theOne;

        return success;
    }
}
