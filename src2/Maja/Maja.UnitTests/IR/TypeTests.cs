using FluentAssertions;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;
using Xunit;

namespace Maja.UnitTests.IR;

public class TypeTests
{
    [Fact]
    public void TypeEnums()
    {
        const string code =
            "MyType" + Tokens.Eol +
            Tokens.Indent1 + "Option1, Option2" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Members.Should().HaveCount(1);
        var type = program.Root.Members[0].As<IrTypeDeclaration>();
        type.Symbol.Name.Should().Be("MyType");
        type.Enums.Should().HaveCount(2);
        type.Enums[0].Symbol.Name.Should().Be("Option1");
        type.Enums[0].Value.Should().Be(0);
        type.Enums[0].Symbol.Type.Should().Be(TypeSymbol.I64);
        type.Enums[1].Value.Should().Be(1);
        type.Enums[1].Symbol.Name.Should().Be("Option2");
        type.Enums[1].Symbol.Type.Should().Be(TypeSymbol.I64);
    }

    [Fact]
    public void TypeFields()
    {
        const string code =
            "MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Members.Should().HaveCount(1);
        var type = program.Root.Members[0].As<IrTypeDeclaration>();
        type.Symbol.Name.Should().Be("MyType");
        type.Fields.Should().HaveCount(2);
        type.Fields[0].DefaultValue.Should().BeNull();
        type.Fields[0].Symbol.Name.Should().Be("fld1");
        type.Fields[0].Type.Symbol.Should().Be(TypeSymbol.U8);
        type.Fields[1].DefaultValue.Should().BeNull();
        type.Fields[1].Symbol.Name.Should().Be("fld2");
        type.Fields[1].Type.Symbol.Should().Be(TypeSymbol.Str);
    }
}
