using System.Linq;
using FluentAssertions;
using Xunit;

namespace Maja.UnitTests.Syntax;

public class ModuleSyntaxTests
{
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
    }

    [Fact]
    public void DirectivePub1_Single()
    {
        const string code =
            "pub qualified.name" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();
        var pubs = result.PublicExports.ToArray();
        pubs.Should().HaveCount(1);
        pubs[0].QualifiedNames.Should().HaveCount(1);
    }

    [Fact]
    public void DirectivePub1_Multiple()
    {
        const string code =
            "pub qualified.name1, qualified.name2" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();
        var pubs = result.PublicExports.ToArray();
        pubs.Should().HaveCount(1);
        pubs[0].QualifiedNames.Should().HaveCount(2);
    }

    [Fact]
    public void DirectivePub2_Single()
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
    }

    [Fact]
    public void DirectiveUseImport()
    {
        const string code =
            "use qualified.name" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();
        var uses = result.UseImports.ToArray();
        uses.Should().HaveCount(1);
        uses[0].QualifiedName.Text.Should().Be("qualified.name");
    }
}