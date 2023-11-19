using System.Collections.Generic;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Eval;

public class EvaluatorResult
{
    internal EvaluatorResult(SyntaxTree syntaxTree, IEnumerable<string> diagnostics, object? value)
    {
        SyntaxTree = syntaxTree;
        Diagnostics = diagnostics;
        Value = value;
    }

    public SyntaxTree SyntaxTree { get; }
    
    public IEnumerable<string> Diagnostics { get; }
    
    public object? Value { get; }
}