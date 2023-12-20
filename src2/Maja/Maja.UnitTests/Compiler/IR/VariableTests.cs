using Maja.Compiler.Diagnostics;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;

namespace Maja.UnitTests.Compiler.IR;

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
        program.Root.Declarations.Should().HaveCount(1);
        var v = program.Root.Declarations[0].As<IrDeclarationVariable>();
        v.Symbol.Name.Value.Should().Be("x");
        v.Initializer!.TypeInferredSymbol.Should().NotBeNull();
        v.Initializer!.ConstantValue.Should().NotBeNull();
        v.Initializer!.ConstantValue!.Value.Should().Be(42);
        v.TypeSymbol.Should().Be(TypeSymbol.I64);

    }

    [Fact]
    public void VarDeclTypedInit()
    {
        const string code =
            "x: U8 = 42" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Declarations.Should().HaveCount(1);
        var v = program.Root.Declarations[0].As<IrDeclarationVariable>();
        v.Symbol.Name.Value.Should().Be("x");
        v.Symbol.Type.Should().Be(TypeSymbol.U8);
        // TODO: expression type != var type
        v.Initializer!.TypeInferredSymbol.Should().NotBeNull();
        v.Initializer!.ConstantValue.Should().NotBeNull();
        v.Initializer!.ConstantValue!.Value.Should().Be(42);
        v.TypeSymbol.Should().Be(TypeSymbol.U8);
    }

    [Fact]
    public void VarDeclTyped()
    {
        const string code =
            "x: U8" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Declarations.Should().HaveCount(1);
        var v = program.Root.Declarations[0].As<IrDeclarationVariable>();
        v.Symbol.Name.Value.Should().Be("x");
        v.Symbol.Type.Should().Be(TypeSymbol.U8);
    }

    [Fact]
    public void VarAssignment()
    {
        const string code =
            "x: U8" + Tokens.Eol +
            "x = 42" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Statements.Should().HaveCount(1);
        var stat = program.Root.Statements[0].As<IrStatementAssignment>();
        stat.Symbol.Name.Value.Should().Be("x");
        stat.Expression.As<IrExpressionLiteral>().ConstantValue!.Value.Should().Be(42);
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
