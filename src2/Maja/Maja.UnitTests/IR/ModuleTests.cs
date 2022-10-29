using FluentAssertions;
using Xunit;

namespace Maja.UnitTests.IR;

public class ModuleTests
{
    [Fact]
    public void Program()
    {
        const string code =
            "pub qualified.name" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Exports.Should().HaveCount(1);
    }

    [Fact]
    public void Export()
    {
        const string code =
            "pub qualified.name" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Exports.Should().HaveCount(1);
    }

    [Fact]
    public void Export2()
    {
        const string code =
            "pub qualified.name1, qualified.name2" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Exports.Should().HaveCount(2);
    }

    [Fact]
    public void Exports()
    {
        const string code =
            "pub qualified.name1" + Tokens.Eol +
            "pub qualified.name2" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Exports.Should().HaveCount(2);
    }

    [Fact]
    public void Exports2()
    {
        const string code =
            "pub qualified.name1, qualified.name2" + Tokens.Eol +
            "pub qualified.name3, qualified.name4" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Exports.Should().HaveCount(4);
    }

    [Fact]
    public void Import()
    {
        const string code =
            "use qualified.name" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Imports.Should().HaveCount(1);
    }

    [Fact]
    public void Imports()
    {
        const string code =
            "use qualified.name1" + Tokens.Eol +
            "use qualified.name2" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Imports.Should().HaveCount(2);
    }
}
