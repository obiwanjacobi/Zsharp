using System.Linq;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;

namespace Maja.UnitTests.Compiler.IR;

public class TypeTests
{
    [Fact]
    public void TypeDeclareEnums()
    {
        const string code =
            "MyType" + Tokens.Eol +
            Tokens.Indent1 + "Option1, Option2" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Declarations.Should().HaveCount(1);
        var type = program.Root.Declarations[0].As<IrDeclarationType>();
        var symbol = type.Symbol.As<DeclaredTypeSymbol>();
        symbol.Name.Value.Should().Be("Mytype");
        symbol.Enums.Should().HaveCount(2);
        type.Enums.Should().HaveCount(2);
        type.Enums[0].Symbol.Name.Value.Should().Be("Option1");
        type.Enums[0].Value.Should().Be(0);
        type.Enums[0].Symbol.Type.Should().Be(TypeSymbol.I64);
        type.Enums[1].Value.Should().Be(1);
        type.Enums[1].Symbol.Name.Value.Should().Be("Option2");
        type.Enums[1].Symbol.Type.Should().Be(TypeSymbol.I64);
    }

    [Fact]
    public void TypeDeclareFields()
    {
        const string code =
            "MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Declarations.Should().HaveCount(1);
        var type = program.Root.Declarations[0].As<IrDeclarationType>();
        var symbol = type.Symbol.As<DeclaredTypeSymbol>();
        symbol.Name.Value.Should().Be("Mytype");
        symbol.Fields.Should().HaveCount(2);
        type.Fields.Should().HaveCount(2);
        type.Fields[0].DefaultValue.Should().BeNull();
        type.Fields[0].Symbol.Name.Value.Should().Be("fld1");
        type.Fields[0].Type.Symbol.Should().Be(TypeSymbol.U8);
        type.Fields[1].DefaultValue.Should().BeNull();
        type.Fields[1].Symbol.Name.Value.Should().Be("fld2");
        type.Fields[1].Type.Symbol.Should().Be(TypeSymbol.Str);
    }

    [Fact]
    public void TypeDeclareDuplicate_Error()
    {
        const string code =
            "MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol +
            "MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol
            ;

        var program = Ir.Build(code, allowError: true);
        program.Diagnostics.Should().HaveCount(1);
        var err = program.Diagnostics[0];
        err.MessageKind.Should().Be(DiagnosticMessageKind.Error);
        err.Text.Should().Contain("Type 'MyType' is already declared.");
    }

    [Fact]
    public void TypeNotFound_Error()
    {
        const string code =
            "x: MyType" + Tokens.Eol
            ;

        var program = Ir.Build(code, allowError: true);
        program.Diagnostics.Should().HaveCount(1);
        var err = program.Diagnostics[0];
        err.MessageKind.Should().Be(DiagnosticMessageKind.Error);
        err.Text.Should().Contain("Type reference 'MyType' cannot be resolved. Type not found.");
    }

    [Fact]
    public void TypeInstantiateFields()
    {
        const string code =
            "MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol +
            "x := MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld1 = 42" + Tokens.Eol +
            Tokens.Indent1 + "fld2 = \"42\"" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Declarations.Should().HaveCount(2);
        var v = program.Root.Declarations[1].As<IrDeclarationVariable>();
        v.TypeSymbol.Name.Value.Should().Be("Mytype");
        var t = v.Initializer.As<IrExpressionTypeInitializer>();
        t.TypeSymbol.Name.Value.Should().Be("Mytype");
        t.Fields.Should().HaveCount(2);
        var f = t.Fields.ToList();
        f[0].Field.Name.Value.Should().Be("fld1");
        f[0].Expression.As<IrExpressionLiteral>().ConstantValue!.ToI32().Should().Be(42);
        f[1].Field.Name.Value.Should().Be("fld2");
        f[1].Expression.As<IrExpressionLiteral>().ConstantValue!.ToStr().Should().Be("42");
    }
}
