using FluentAssertions;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;
using Xunit;

namespace Maja.UnitTests.IR;

public class ExpressionTests
{
    [Fact]
    public void ArithmeticLiterals()
    {
        const string code =
            "x := 12 + 30" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Members.Should().HaveCount(1);
        var v = program.Root.Members[0].As<IrVariableDeclaration>();
        v.Symbol.Name.Value.Should().Be("x");
        v.Initializer!.TypeSymbol.Should().Be(TypeSymbol.I64);
        //v.Initializer!.ConstantValue.Should().NotBeNull();
    }

    [Fact]
    public void InvocationAssign()
    {
        const string code =
            "fn: (): U8" + Tokens.Eol +
            Tokens.Indent1 + "ret 42" + Tokens.Eol +
            "x := fn()" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Members.Should().HaveCount(2);
        var v = program.Root.Members[1].As<IrVariableDeclaration>();
        v.Symbol.Name.Value.Should().Be("x");
        v.Symbol.Type.Should().Be(TypeSymbol.U8);
        v.Initializer!.TypeSymbol.Should().Be(TypeSymbol.U8);
        v.Initializer!.ConstantValue.Should().BeNull();
        v.Type.Should().Be(TypeSymbol.U8);
    }

    [Fact]
    public void Invocation()
    {
        const string code =
            "fn: ()" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol +
            "fn()" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Members.Should().HaveCount(1);
        program.Root.Statements.Should().HaveCount(1);
        var fn = program.Root.Statements[0]
            .As<IrStatementExpression>().Expression
            .As<IrExpressionInvocation>();
        fn.Symbol!.Name.Value.Should().Be("fn");
        fn.Arguments.Should().HaveCount(0);

    }

    [Fact]
    public void InvocationAssign_ErrorCannotAssignVoid()
    {
        const string code =
            "fn: ()" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol +
            "x := fn()" + Tokens.Eol
            ;

        var program = Ir.Build(code, allowError: true);
        program.Diagnostics.Should().HaveCount(1);
        var err = program.Diagnostics[0];
        err.MessageKind.Should().Be(DiagnosticMessageKind.Error);
        err.Text.Should().Contain("Cannot assign Void").And.Contain("x");
    }

    [Fact]
    public void Invocation_ErrorNotFound()
    {
        const string code =
            "x := fn()" + Tokens.Eol
            ;

        var program = Ir.Build(code, allowError: true);
        program.Diagnostics.Should().HaveCount(1);
        var err = program.Diagnostics[0];
        err.MessageKind.Should().Be(DiagnosticMessageKind.Error);
        err.Text.Should().Contain("Function reference 'fn' cannot be resolved. Function not found.");
    }
}
