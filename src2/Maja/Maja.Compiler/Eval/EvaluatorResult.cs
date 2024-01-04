using System.Collections.Generic;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Eval;

public class EvaluatorResult
{
    internal EvaluatorResult(EvaluatorState state, SyntaxTree syntaxTree, IEnumerable<string> diagnostics, object? value)
    {
        State = state;
        SyntaxTree = syntaxTree;
        Diagnostics = diagnostics;
        Value = value;
    }

    internal EvaluatorState State { get; }

    public SyntaxTree SyntaxTree { get; }

    public IEnumerable<string> Diagnostics { get; }

    public object? Value { get; }

    public bool TryLookupVariable(string variable, out object? value)
        => State.TryLookupVariable(variable, out value);
}