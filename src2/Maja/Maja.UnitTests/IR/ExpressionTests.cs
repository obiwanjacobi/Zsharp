using FluentAssertions;
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
        v.Symbol.Name.Should().Be("x");
        v.Initializer!.TypeSymbol.Should().Be(TypeSymbol.I64);
        //v.Initializer!.ConstantValue.Should().NotBeNull();
    }

    [Fact]
    public void InvocationAssign()
    {
        const string code =
            "x := fn()" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Members.Should().HaveCount(1);
        var v = program.Root.Members[0].As<IrVariableDeclaration>();
        v.Symbol.Name.Should().Be("x");
        v.Initializer!.TypeSymbol.Should().Be(TypeSymbol.I64);
        v.Initializer!.ConstantValue.Should().BeNull();
    }
}
