using System.Collections;
using System.Collections.Generic;

namespace Maja.Runtime;

internal sealed class SingleValueEnumerator<T> : IEnumerator<T>
{
    private bool _done;
    private readonly T _value;

    public T Current => _value;
    object? IEnumerator.Current => _value;

    public bool MoveNext()
    {
        if (!_done)
        {
            _done = true;
            return true;
        }

        return false;
    }

    public SingleValueEnumerator(T value) => _value = value;
    public void Reset() => _done = false;
    public void Dispose() { /* no resources to dispose*/ }
}
