using FluentAssertions;
using Maja.UnitTests.EmitCS;
using Xunit;
using Xunit.Abstractions;

namespace Maja.UnitTests.EmitCS;

public class EmitModuleTests
{
    private readonly ITestOutputHelper _output;

    public EmitModuleTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Module()
    {
        const string code =
            "mod qualified.name" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should().StartWith("namespace")
            .And.Contain("qualified")
            .And.Contain("class")
            .And.Contain("static")
            .And.Contain("name")
            ;
    }

    [Fact]
    public void ModuleInit()
    {
        const string code =
            "mod qualified.name" + Tokens.Eol + 
            "x := 42" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code, _output);
        _output.WriteLine(emit);

        emit.Should().StartWith("namespace")
            .And.Contain("qualified")
            .And.Contain("class")
            .And.Contain("static")
            .And.Contain("name")
            .And.Contain("x")
            ;
    }

    [Fact]
    public void ModuleBuild()
    {
        const string code =
            "mod qualified.name" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        var output = Emit.Build(emit);
        _output.WriteLine(output);

        output.Should().NotBeNullOrEmpty()
            .And.NotContain("ERROR");
    }
}
