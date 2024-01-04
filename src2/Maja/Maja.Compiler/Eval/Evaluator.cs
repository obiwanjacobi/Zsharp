using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.External;
using Maja.Compiler.IR;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Eval;

public sealed class Evaluator
{
    private readonly IExternalModuleLoader _moduleLoader;
    private EvaluatorState _state;

    public Evaluator()
        : this(new())
    { }

    public Evaluator(EvaluatorState state)
    {
        _state = state;
        _moduleLoader = new AssemblyManagerBuilder()
            .AddSystemAll()
            .ToModuleLoader();
    }

    private void PushScope(IrScope scope)
    {
        _state = new EvaluatorState(_state, scope);
    }

    public EvaluatorResult Eval(string inputText)
    {
        object? result = null;
        var diagnostics = new List<string>();

        var syntaxTree = SyntaxTree.Parse(inputText, nameof(Evaluator));
        if (syntaxTree.Diagnostics.HasDiagnostics)
        {
            diagnostics.AddRange(syntaxTree.Diagnostics.Select(d => d.ToString()));
        }
        else
        {
            var program = IrBuilder.Program(syntaxTree, _moduleLoader, _state.Scope);
            if (program.Diagnostics.Length > 0)
            {
                diagnostics.AddRange(program.Diagnostics.Select(d => d.ToString()));
            }
            else
            {
                PushScope(program.Scope);
                result = Eval(program);

                if (_state.Diagnostics.HasDiagnostics)
                    diagnostics.AddRange(_state.Diagnostics.Select(d => d.ToString()));
            }
        }

        return new EvaluatorResult(_state, syntaxTree, diagnostics, result);
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
