using FluentAssertions;
using Maja.UnitTests.EmitCS;
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

        var emit = Emit.FromCode(code, _output);
        _output.WriteLine(emit);

        emit.Should().StartWith("namespace")
            .And.Contain(" class ")
            .And.Contain(" static ")
            .And.Contain(" root")
            .And.Contain(" fn()")
            .And.Contain(" System.Void ")
            .And.Contain(" return ")
            ;
    }
}
