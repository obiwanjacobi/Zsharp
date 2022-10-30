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
            "Type" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var t = result.Members.First().As<TypeDeclarationSyntax>();
        t.Name.Text.Should().Be("Type");
        t.Fields.Items.Should().HaveCount(2);
        t.Fields.Items.First().Name.Text.Should().Be("fld1");
        t.Fields.Items.First().Type.Name.Text.Should().Be("U8");
        t.Fields.Items.Skip(1).First().Name.Text.Should().Be("fld2");
        t.Fields.Items.Skip(1).First().Type.Name.Text.Should().Be("Str");
    }

    [Fact]
    public void TypeEnumComma()
    {
        const string code =
            "Type" + Tokens.Eol +
            Tokens.Indent1 + "Option1, Option2" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var t = result.Members.First().As<TypeDeclarationSyntax>();
        t.Name.Text.Should().Be("Type");
        t.Enums.Items.Should().HaveCount(2);
        t.Enums.Items.First().Name.Text.Should().Be("Option1");
        t.Enums.Items.Skip(1).First().Name.Text.Should().Be("Option2");
    }

    [Fact]
    public void TypeEnumIndent()
    {
        const string code =
            "Type" + Tokens.Eol +
            Tokens.Indent1 + "Option1 = 0" + Tokens.Eol +
            Tokens.Indent1 + "Option2 = 1" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var t = result.Members.First().As<TypeDeclarationSyntax>();
        t.Name.Text.Should().Be("Type");
        t.Enums.Items.Should().HaveCount(2);
        t.Enums.Items.First().Name.Text.Should().Be("Option1");
        t.Enums.Items.First().Expression!.Text.Should().Be("0");
        t.Enums.Items.Skip(1).First().Name.Text.Should().Be("Option2");
        t.Enums.Items.Skip(1).First().Expression!.Text.Should().Be("1");
    }

    [Fact]
    public void TypeEnumStruct()
    {
        const string code =
            "Type" + Tokens.Eol +
            Tokens.Indent1 + "Option1, Option2" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var t = result.Members.First().As<TypeDeclarationSyntax>();
        t.Name.Text.Should().Be("Type");

        t.Enums.Items.Should().HaveCount(2);
        t.Enums.Items.First().Name.Text.Should().Be("Option1");
        t.Enums.Items.Skip(1).First().Name.Text.Should().Be("Option2");

        t.Fields.Items.Should().HaveCount(2);
        t.Fields.Items.First().Name.Text.Should().Be("fld1");
        t.Fields.Items.First().Type.Name.Text.Should().Be("U8");
        t.Fields.Items.Skip(1).First().Name.Text.Should().Be("fld2");
        t.Fields.Items.Skip(1).First().Type.Name.Text.Should().Be("Str");
    }
}