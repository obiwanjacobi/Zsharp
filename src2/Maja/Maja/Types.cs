using System.Runtime.CompilerServices;

namespace Maja;

public static class Types
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Array<T>(uint capacity)
        => new T[capacity];
}
