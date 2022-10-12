using System.Linq;
using FluentAssertions;
using Maja.Compiler.Syntax;
using Xunit;

namespace Maja.UnitTests.Syntax;

public class ModuleSyntaxTests
{
    [Fact]
    public void DeclPub1_Single()
    {
        const string code =
            "pub qualified.name" + Tokens.EOL
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();
        var pubs = result.PublicExports.ToArray();
        pubs.Should().HaveCount(1);
        pubs[0].QualifiedNames.Should().HaveCount(1);
    }

    [Fact]
    public void DeclPub1_Multiple()
    {
        const string code =
            "pub qualified.name1, qualified.name2" + Tokens.EOL
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();
        var pubs = result.PublicExports.ToArray();
        pubs.Should().HaveCount(1);
        pubs[0].QualifiedNames.Should().HaveCount(2);
    }

    [Fact]
    public void DeclPub2_Single()
    {
        const string code =
            "pub" + Tokens.EOL +
            Tokens.INDENT1 + "qualified.name" + Tokens.EOL
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();
        var pubs = result.PublicExports.ToArray();
        pubs.Should().HaveCount(1);
        pubs[0].QualifiedNames.Should().HaveCount(1);
    }

    [Fact]
    public void UseImport()
    {
        const string code =
            "use qualified.name" + Tokens.EOL
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();
        var uses = result.UseImports.ToArray();
        uses.Should().HaveCount(1);
        uses[0].QualifiedName.Text.Should().Be("qualified.name");
    }
}