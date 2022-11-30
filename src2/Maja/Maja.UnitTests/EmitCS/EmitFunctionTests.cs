using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Maja.UnitTests.EmitCS;

public class EmitFunctionTests
{
    private readonly ITestOutputHelper _output;

    public EmitFunctionTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Function()
    {
        const string code =
            "fn: ()" + Tokens.Eol + 
            Tokens.Indent1 + "ret" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should().StartWith("namespace")
            .And.Contain(" class ")
            .And.Contain(" static ")
            .And.Contain(" root")
            .And.Contain(" fn()")
            .And.Contain(" void ")
            .And.Contain(" return ")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void FunctionParameters()
    {
        const string code =
            "fn: (p1: U8, p2: Str)" + Tokens.Eol +
            Tokens.Indent1 + "ret p2" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should().StartWith("namespace")
            .And.Contain(" class ")
            .And.Contain(" fn(")
            .And.Contain("System.Byte p1")
            .And.Contain("System.String p2")
            .And.Contain(" System.String ")
            .And.Contain(" void ")
            .And.Contain(" return")
            ;

        Emit.AssertBuild(emit);
    }
}
