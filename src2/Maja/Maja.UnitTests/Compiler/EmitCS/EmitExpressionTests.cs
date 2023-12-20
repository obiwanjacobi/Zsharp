namespace Maja.UnitTests.Compiler.EmitCS;

public class EmitExpressionTests
{
    private readonly ITestOutputHelper _output;

    public EmitExpressionTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ExpressionLiteral()
    {
        const string code =
            "x := 42 + 101" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain(" x ")
            .And.Contain(" = ")
            .And.Contain(" 42 ")
            .And.Contain(" + ")
            .And.Contain(" 101")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void ExpressionIdentifer()
    {
        const string code =
            "x := 42" + Tokens.Eol +
            "y := x + 101" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain(" x ")
            .And.Contain(" y ")
            .And.Contain(" = ")
            .And.Contain(" 42")
            .And.Contain(" x + 101")
            ;

        Emit.AssertBuild(emit);
    }
}
