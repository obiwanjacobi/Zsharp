using Maja.Compiler.Eval;

namespace Maja.UnitTests.Compiler.IR;

public class EvaluatorTests
{
    [Fact]
    public void VariableAssignment()
    {
        const string code =
            "x := 42" + Tokens.Eol
            ;

        var result = Eval(code);
        result.TryLookupVariable("Defmod.x", out var value).Should().BeTrue();
        value.Should().Be(42);
    }

    private EvaluatorResult Eval(string code)
    {
        var eval = new Evaluator();
        return eval.Eval(code);
    }
}
