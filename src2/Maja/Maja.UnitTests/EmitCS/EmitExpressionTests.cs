using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Maja.UnitTests.EmitCS;

public class EmitExpressionTests
{
    private readonly ITestOutputHelper _output;

    public EmitExpressionTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Expression()
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
}
