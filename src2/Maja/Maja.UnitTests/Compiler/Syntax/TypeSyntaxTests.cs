using System.Linq;
using Maja.Compiler.Syntax;

namespace Maja.UnitTests.Compiler.Syntax;

public class TypeSyntaxTests
{
    private readonly ITestOutputHelper _output;

    public TypeSyntaxTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void TypeDeclareStruct()
    {
        const string code =
            "Type" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var t = result.Members.First().As<DeclarationTypeSyntax>();
        t.Name.Text.Should().Be("Type");
        t.Fields!.Items.Should().HaveCount(2);
        t.Fields!.Items.First().Name.Text.Should().Be("fld1");
        t.Fields!.Items.First().Type.Name.Text.Should().Be("U8");
        t.Fields!.Items.Skip(1).First().Name.Text.Should().Be("fld2");
        t.Fields!.Items.Skip(1).First().Type.Name.Text.Should().Be("Str");

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void TypeDeclareStruct_Derived()
    {
        const string code =
            "Type : BaseType" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var t = result.Members.First().As<DeclarationTypeSyntax>();
        t.Name.Text.Should().Be("Type");
        t.BaseType.Should().NotBeNull();
        t.BaseType!.Name.Text.Should().Be("BaseType");

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void TypeDeclareStruct_Generics()
    {
        const string code =
            "Type<T>" + Tokens.Eol +
            Tokens.Indent1 + "fld1: T" + Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var t = result.Members.First().As<DeclarationTypeSyntax>();
        t.Name.Text.Should().Be("Type");
        t.TypeParameters.Should().HaveCount(1);
        var tp = t.TypeParameters.First();
        tp.Type.Text.Should().Be("T");
        t.Fields!.Items.Should().HaveCount(2);
        t.Fields!.Items.First().Name.Text.Should().Be("fld1");
        t.Fields!.Items.First().Type.Name.Text.Should().Be("T");
        t.Fields!.Items.Skip(1).First().Name.Text.Should().Be("fld2");
        t.Fields!.Items.Skip(1).First().Type.Name.Text.Should().Be("Str");

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void TypeDeclareStruct_GenericsDefault()
    {
        const string code =
            "Type<T = U8>" + Tokens.Eol +
            Tokens.Indent1 + "fld1: T" + Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var t = result.Members.First().As<DeclarationTypeSyntax>();
        t.Name.Text.Should().Be("Type");
        t.TypeParameters.Should().HaveCount(1);
        var tp = t.TypeParameters.First();
        tp.Type.Text.Should().Be("T");
        tp.DefaultType!.Text.Should().Be("U8");
        t.Fields!.Items.Should().HaveCount(2);
        t.Fields!.Items.First().Name.Text.Should().Be("fld1");
        t.Fields!.Items.First().Type.Name.Text.Should().Be("T");
        t.Fields!.Items.Skip(1).First().Name.Text.Should().Be("fld2");
        t.Fields!.Items.Skip(1).First().Type.Name.Text.Should().Be("Str");

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void TypeDeclareEnumComma()
    {
        const string code =
            "Type" + Tokens.Eol +
            Tokens.Indent1 + "Option1, Option2" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var t = result.Members.First().As<DeclarationTypeSyntax>();
        t.Name.Text.Should().Be("Type");
        t.Enums!.Items.Should().HaveCount(2);
        t.Enums!.Items.First().Name.Text.Should().Be("Option1");
        t.Enums!.Items.Skip(1).First().Name.Text.Should().Be("Option2");

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void TypeDeclareEnumIndent()
    {
        const string code =
            "Type" + Tokens.Eol +
            Tokens.Indent1 + "Option1 = 0" + Tokens.Eol +
            Tokens.Indent1 + "Option2 = 1" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var t = result.Members.First().As<DeclarationTypeSyntax>();
        t.Name.Text.Should().Be("Type");
        t.Enums!.Items.Should().HaveCount(2);
        t.Enums!.Items.First().Name.Text.Should().Be("Option1");
        t.Enums!.Items.First().Expression!.Text.Should().Be("0");
        t.Enums!.Items.Skip(1).First().Name.Text.Should().Be("Option2");
        t.Enums!.Items.Skip(1).First().Expression!.Text.Should().Be("1");

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void TypeDeclareEnumStruct()
    {
        const string code =
            "Type" + Tokens.Eol +
            Tokens.Indent1 + "Option1, Option2" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var t = result.Members.First().As<DeclarationTypeSyntax>();
        t.Name.Text.Should().Be("Type");

        t.Enums!.Items.Should().HaveCount(2);
        t.Enums!.Items.First().Name.Text.Should().Be("Option1");
        t.Enums!.Items.Skip(1).First().Name.Text.Should().Be("Option2");

        t.Fields!.Items.Should().HaveCount(2);
        t.Fields!.Items.First().Name.Text.Should().Be("fld1");
        t.Fields!.Items.First().Type.Name.Text.Should().Be("U8");
        t.Fields!.Items.Skip(1).First().Name.Text.Should().Be("fld2");
        t.Fields!.Items.Skip(1).First().Type.Name.Text.Should().Be("Str");

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void TypeInstantiateStruct()
    {
        const string code =
            "x := Type" + Tokens.Eol +
            Tokens.Indent1 + "fld1 = 42" + Tokens.Eol +
            Tokens.Indent1 + "fld2 = \"42\"" + Tokens.Eol
            //"x := Type { fld1 = 42, fld2 = \"42\" }" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<DeclarationVariableSyntax>();
        v.Name.Text.Should().Be("x");
        var t = v.Expression.As<ExpressionTypeInitializerSyntax>();
        t.Type.Name.Text.Should().Be("Type");
        var f = t.FieldInitializers.ToList();
        f.Should().HaveCount(2);
        f[0].Name.Text.Should().Be("fld1");
        f[0].Expression.As<ExpressionLiteralSyntax>().LiteralNumber!.Text.Should().Be("42");
        f[1].Name.Text.Should().Be("fld2");
        f[1].Expression.As<ExpressionLiteralSyntax>().LiteralString!.Text.Should().Be("42");

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void TypeInstantiateStruct_Generics()
    {
        const string code =
            "x := Type<U8>" + Tokens.Eol +
            Tokens.Indent1 + "fld1 = 42" + Tokens.Eol +
            Tokens.Indent1 + "fld2 = \"42\"" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<DeclarationVariableSyntax>();
        v.Name.Text.Should().Be("x");
        var t = v.Expression.As<ExpressionTypeInitializerSyntax>();
        t.Type.Name.Text.Should().Be("Type");
        t.Type.TypeArguments.Should().HaveCount(1);
        var f = t.FieldInitializers.ToList();
        f.Should().HaveCount(2);
        f[0].Name.Text.Should().Be("fld1");
        f[0].Expression.As<ExpressionLiteralSyntax>().LiteralNumber!.Text.Should().Be("42");
        f[1].Name.Text.Should().Be("fld2");
        f[1].Expression.As<ExpressionLiteralSyntax>().LiteralString!.Text.Should().Be("42");

        Syntax.RoundTrip(code, _output);
    }
}