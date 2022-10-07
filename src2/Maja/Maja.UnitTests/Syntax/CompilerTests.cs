using FluentAssertions;
using Maja.Compiler.Syntax;
using Xunit;

namespace Maja.UnitTests.Syntax;

public class CompilerTests
{
    [Fact]
    public void Parse_Multiple()
    {
        var compiler = new Maja.Compiler.Compiler();

        var pass1 = compiler.Parse("pub name1\n", nameof(Parse_Multiple));
        pass1.Children[0].Children[0].As<NameSyntax>().Text.Should().Be("name1");

        var pass2 = compiler.Parse("pub name2\n", nameof(Parse_Multiple));
        pass2.Children[0].Children[0].As<NameSyntax>().Text.Should().Be("name2");
    }
}