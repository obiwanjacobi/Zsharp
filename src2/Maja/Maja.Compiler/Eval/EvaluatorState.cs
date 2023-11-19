using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Maja.Compiler.Eval;

public sealed class EvaluatorState
{
    private readonly Dictionary<string, object> _variables = new();
    private readonly EvaluatorState? _parent;

    public EvaluatorState()
    { }

    public EvaluatorState(EvaluatorState parent)
    {
        _parent = parent;
    }

    public bool TryLookupValue(string fullName, [NotNullWhen(true)] out object? value)
    {
        if (_variables.TryGetValue(fullName, out value))
            return true;

        if (_parent is not null)
            return _parent.TryLookupValue(fullName, out value);

        value = null;
        return false;
    }

    public bool TrySetValue(string name, object value)
    {
        if (_variables.ContainsKey(name))
        {
            _variables[name] = value;
            return true;
        }

        if (_parent?.TrySetValue(name, value) == true)
            return true;

        _variables[name] = value;
        return true;
    }

    public bool TryAdd(string name, object value)
    {
        return _variables.TryAdd(name, value);
    }

    public void Reset()
    {
        _variables.Clear();
        _parent?.Reset();
    }
}
