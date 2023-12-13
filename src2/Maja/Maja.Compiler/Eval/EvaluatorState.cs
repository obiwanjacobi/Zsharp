using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.IR;

namespace Maja.Compiler.Eval;

public sealed class EvaluatorState
{
    private readonly DiagnosticList _diagnostics = new();
    private readonly Dictionary<string, object> _variables = new();
    private readonly EvaluatorState? _parent;
    private readonly IrScope? _scope;

    internal EvaluatorState()
    { }

    internal EvaluatorState(EvaluatorState parent, IrScope scope)
    {
        _parent = parent;
        _scope = scope;
    }

    internal IrScope? Scope => _scope;

    internal DiagnosticList Diagnostics
        => _diagnostics;

    public bool TryLookupVariable(string fullName, [NotNullWhen(true)] out object? value)
    {
        if (_variables.TryGetValue(fullName, out value))
            return true;

        if (_parent is not null)
            return _parent.TryLookupVariable(fullName, out value);

        value = null;
        return false;
    }

    public bool TrySetVariable(string name, object value)
    {
        if (_variables.ContainsKey(name))
        {
            _variables[name] = value;
            return true;
        }

        if (_parent?.TrySetVariable(name, value) == true)
            return true;

        _variables[name] = value;
        return true;
    }

    public void Reset()
    {
        _variables.Clear();
        _parent?.Reset();
    }
}
