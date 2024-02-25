using System.Collections;

namespace Maja.Compiler;

internal static class Extensions
{
    public static T[] Append<T>(this T[] first, T[] last)
    {
        if (last is null || last.Length == 0) return first;

        var arr = new T[first.Length + last.Length];
        first.CopyTo(arr, 0);
        last.CopyTo(arr, first.Length);
        return arr;
    }
}