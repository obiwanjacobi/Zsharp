using System.Runtime.CompilerServices;

namespace Maja.Runtime;

public static class Types
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Array<T>(uint capacity)
        => new T[capacity];
}
