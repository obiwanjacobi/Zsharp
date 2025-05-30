namespace Maja.UnitTests.Compiler.IR;

public class ModuleTests
{
    [Fact]
    public void Program()
    {
        const string code =
            "pub qualified.name" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Should().NotBeNull();
        program.Module.Exports.Should().HaveCount(1);
    }

    [Fact]
    public void Module()
    {
        const string code =
            "mod qualified.name" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Should().NotBeNull();
        program.Module.Symbol.Name.Namespace.Value.Should().Be("qualified");
        program.Module.Symbol.Name.Value.Should().Be("name");
    }

    [Fact]
    public void Export()
    {
        const string code =
            "pub qualified.name" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Exports.Should().HaveCount(1);
    }

    [Fact]
    public void Export2()
    {
        const string code =
            "pub qualified.name1, qualified.name2" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Exports.Should().HaveCount(2);
    }

    [Fact]
    public void Exports()
    {
        const string code =
            "pub qualified.name1" + Tokens.Eol +
            "pub qualified.name2" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Exports.Should().HaveCount(2);
    }

    [Fact]
    public void Exports2()
    {
        const string code =
            "pub qualified.name1, qualified.name2" + Tokens.Eol +
            "pub qualified.name3, qualified.name4" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Exports.Should().HaveCount(4);
    }

    [Fact]
    public void Import_Name()
    {
        const string code =
            "use qu_Alified.N_ame" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Imports.Should().HaveCount(1);

        var import = program.Module.Imports[0];
        import.SymbolName.Namespace.Value.Should().Be("qualified");
        import.SymbolName.Namespace.OriginalName.Should().Be("qu_Alified");
        import.SymbolName.Value.Should().Be("Name");
        import.SymbolName.OriginalName.Should().Be("N_ame");
        import.SymbolName.FullName.Should().Be("qualified.Name");
    }

    [Fact]
    public void Imports()
    {
        const string code =
            "use qualified.name1" + Tokens.Eol +
            "use qualified.name2" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Imports.Should().HaveCount(2);
    }
}
