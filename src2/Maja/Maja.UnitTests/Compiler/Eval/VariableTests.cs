namespace Maja.UnitTests.Compiler.Eval;

public class VariableTests
{
    [Fact]
    public void VariableAssignment()
    {
        const string code =
            "x : U8 = 42" + Tokens.Eol
            ;

        var result = Eval.Run(code);
        result.TryLookupVariable("Defmod.x", out var value).Should().BeTrue();
        value.Should().Be(42);
    }

    [Fact]
    public void VariableInfer()
    {
        const string code =
            "x := 42" + Tokens.Eol
            ;

        var result = Eval.Run(code);
        result.TryLookupVariable("Defmod.x", out var value).Should().BeTrue();
        value.Should().Be(42);
    }
}
