using System.Collections.Generic;

namespace Maja.Compiler.IR;

internal interface IrContainer
{
    IEnumerable<T> GetDescendantsOfType<T>() where T : IrNode;
}

internal static class IrContainerExtensions
{
    public static IEnumerable<T> GetDescendantsOfType<T>(this IEnumerable<IrNode> nodes)
        where T : IrNode
    {
        foreach (var node in nodes)
        {
            if (node is T nodeType)
                yield return nodeType;

            if (node is IrContainer container)
                foreach (var t in container.GetDescendantsOfType<T>())
                    yield return t;
        }
    }

    public static IEnumerable<T> GetDescendantsOfType<T>(this IrNode? node)
        where T : IrNode
    {
        if (node is null) yield break;

        if (node is T nodeType)
            yield return nodeType;

        if (node is IrContainer container)
            foreach (var t in container.GetDescendantsOfType<T>())
                yield return t;
    }
}