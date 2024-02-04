using System.Linq;
using Xunit.Abstractions;

namespace Maja.UnitTests.Compiler.Syntax;

public class ModuleSyntaxTests
{
    private readonly ITestOutputHelper _output;
    public ModuleSyntaxTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void DirectiveModule()
    {
        const string code =
            "mod qualified.name" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();
        result.Module.Should().NotBeNull();
        result.Module!.Identifier.Text.Should().Be("qualified.name");

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void DirectivePublic_Single()
    {
        const string code =
            "pub qualified.name" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();
        var pubs = result.PublicExports.ToArray();
        pubs.Should().HaveCount(1);
        pubs[0].QualifiedNames.Should().HaveCount(1);

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void DirectivePublic_MultipleComma()
    {
        const string code =
            "pub qualified.name1, qualified.name2" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();
        var pubs = result.PublicExports.ToArray();
        pubs.Should().HaveCount(1);
        pubs[0].QualifiedNames.Should().HaveCount(2);

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void DirectivePublic_SingleNewline()
    {
        const string code =
            "pub" + Tokens.Eol +
            Tokens.Indent1 + "qualified.name" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();
        var pubs = result.PublicExports.ToArray();
        pubs.Should().HaveCount(1);
        pubs[0].QualifiedNames.Should().HaveCount(1);

        //Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void DirectiveUseImport_Single()
    {
        const string code =
            "use qualified.name" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();
        var uses = result.UseImports.ToArray();
        uses.Should().HaveCount(1);
        uses[0].QualifiedNames.Should().HaveCount(1);

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void DirectiveUseImport_MultipleComma()
    {
        const string code =
            "use qualified.name1, qualified.name2" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();
        var uses = result.UseImports.ToArray();
        uses.Should().HaveCount(1);
        uses[0].QualifiedNames.Should().HaveCount(2);

        Syntax.RoundTrip(code, _output);
    }
}