using System.Linq;
using FluentAssertions;
using Maja.Compiler.Syntax;
using Xunit;

namespace Maja.UnitTests.Syntax;

public class TypeSyntaxTests
{
    [Fact]
    public void TypeStruct()
    {
        const string code =
            "Type" + Tokens.EOL +
            Tokens.INDENT1 + "fld1: U8" + Tokens.EOL +
            Tokens.INDENT1 + "fld2: Str" + Tokens.EOL
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var t = result.Members.First().As<TypeDeclarationSyntax>();
        t.Name.Text.Should().Be("Type");
        t.Fields.Members.Should().HaveCount(2);
        t.Fields.Members.First().Name.Text.Should().Be("fld1");
        t.Fields.Members.First().Type.Name.Text.Should().Be("U8");
        t.Fields.Members.Skip(1).First().Name.Text.Should().Be("fld2");
        t.Fields.Members.Skip(1).First().Type.Name.Text.Should().Be("Str");
    }

    [Fact]
    public void TypeEnum()
    {
        const string code =
            "Type" + Tokens.EOL +
            Tokens.INDENT1 + "Option1 = 0" + Tokens.EOL +
            Tokens.INDENT1 + "Option2 = 1" + Tokens.EOL
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var t = result.Members.First().As<TypeDeclarationSyntax>();
        t.Name.Text.Should().Be("Type");
        t.Enums.Members.Should().HaveCount(2);
        t.Enums.Members.First().Name.Text.Should().Be("Option1");
        t.Enums.Members.First().Expression!.Text.Should().Be("0");
        t.Enums.Members.Skip(1).First().Name.Text.Should().Be("Option2");
        t.Enums.Members.Skip(1).First().Expression!.Text.Should().Be("1");
    }
}