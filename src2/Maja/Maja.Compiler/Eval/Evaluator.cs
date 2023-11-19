using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.IR;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Eval;

public sealed class Evaluator
{
    private readonly EvaluatorState _state;

    public Evaluator()
        : this(new())
    { }

    public Evaluator(EvaluatorState state)
    {
        _state = state;
    }

    public EvaluatorResult Eval(string inputText)
    {
        object? result = null;
        var diagnostics = new List<string>();

        var syntaxTree = SyntaxTree.Parse(inputText, nameof(Evaluator));
        if (syntaxTree.Diagnostics.HasDiagnaostics)
        {
            diagnostics.AddRange(syntaxTree.Diagnostics.Select(d => d.ToString()));
        }
        else
        {
            var compilation = Compilation.Compilation.Create(syntaxTree);
            var model = compilation.GetModel(syntaxTree);
            if (model.Program.Diagnostics.Length > 0)
                diagnostics.AddRange(syntaxTree.Diagnostics.Select(d => d.ToString()));
            else
                result = Eval(model.Program);
        }

        return new EvaluatorResult(syntaxTree, diagnostics, result);
    }

    public void Reset()
        => _state.Reset();

    private object? Eval(IrProgram program)
    {
        var walker = new EvalWalker(_state);
        var result = walker.OnProgram(program);
        return result;
    }
}
