using FluentAssertions;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;
using Xunit;

namespace Maja.UnitTests.IR;

public class VariableTests
{
    [Fact]
    public void VarDeclInferred()
    {
        const string code =
            "x := 42" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Members.Should().HaveCount(1);
        var v = program.Root.Members[0].As<IrVariableDeclaration>();
        v.Symbol.Name.Should().Be("x");
        v.Initializer!.TypeSymbol.Should().Be(TypeSymbol.I64);
    }

    [Fact]
    public void VarDeclTypedInit()
    {
        const string code =
            "x: U8 = 42" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Members.Should().HaveCount(1);
        var v = program.Root.Members[0].As<IrVariableDeclaration>();
        v.Symbol.Name.Should().Be("x");
        v.Symbol.Type.Should().Be(TypeSymbol.U8);
        // TODO: expression type != var type
        v.Initializer!.TypeSymbol.Should().Be(TypeSymbol.I64);
    }

    [Fact]
    public void VarDeclTyped()
    {
        const string code =
            "x: U8" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Members.Should().HaveCount(1);
        var v = program.Root.Members[0].As<IrVariableDeclaration>();
        v.Symbol.Name.Should().Be("x");
        v.Symbol.Type.Should().Be(TypeSymbol.U8);
    }

    [Fact]
    public void VarDeclDuplicate_Error()
    {
        const string code =
            "x: U8" + Tokens.Eol +
            "x: U64" + Tokens.Eol
            ;

        var program = Ir.Build(code, allowError: true);
        program.Diagnostics.Should().HaveCount(1);
        var err = program.Diagnostics[0];
        err.MessageKind.Should().Be(DiagnosticMessageKind.Error);
        err.Text.Should().Contain("Variable name 'x' is already declared.");
    }

    [Fact]
    public void VarNotFound_Error()
    {
        const string code =
            "x := y" + Tokens.Eol
            ;

        var program = Ir.Build(code, allowError: true);
        program.Diagnostics.Should().HaveCount(1);
        var err = program.Diagnostics[0];
        err.MessageKind.Should().Be(DiagnosticMessageKind.Error);
        err.Text.Should().Contain("Variable reference 'y' cannot be resolved. Variable not found.");
    }
}
