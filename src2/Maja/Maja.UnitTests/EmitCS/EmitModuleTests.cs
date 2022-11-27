using FluentAssertions;
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

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void ModuleField()
    {
        const string code =
            "mod qualified.name" + Tokens.Eol + 
            "x := 42" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should().StartWith("namespace")
            .And.Contain("qualified")
            .And.Contain("class")
            .And.Contain("static")
            .And.Contain("name")
            .And.Contain("x")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void ModuleFieldInit()
    {
        const string code =
            "mod qualified.name" + Tokens.Eol +
            "x: U8" + Tokens.Eol +
            "x = 42" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should().StartWith("namespace")
            .And.Contain("qualified")
            .And.Contain("class")
            .And.Contain("static")
            .And.Contain("name")
            .And.Contain("x = 42;")
            ;

        Emit.AssertBuild(emit);
    }
}
