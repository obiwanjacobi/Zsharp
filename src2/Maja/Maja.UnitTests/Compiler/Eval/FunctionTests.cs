namespace Maja.UnitTests.Compiler.Eval;

public class FunctionTests
{
    [Fact]
    public void FunctionRetVal()
    {
        const string code =
            "fn: (): U8" + Tokens.Eol +
            Tokens.Indent1 + "ret 42" + Tokens.Eol +
            "x := fn()" + Tokens.Eol
            ;

        var result = Eval.Run(code);
        result.TryLookupVariable("Defmod.x", out var value).Should().BeTrue();
        value.Should().Be(42);
    }

    [Fact]
    public void FunctionParam()
    {
        const string code =
            "fn: (p: U8): U8" + Tokens.Eol +
            Tokens.Indent1 + "ret p" + Tokens.Eol +
            "x := fn(42)" + Tokens.Eol
            ;

        var result = Eval.Run(code);
        result.TryLookupVariable("Defmod.x", out var value).Should().BeTrue();
        value.Should().Be(42);
    }
}
