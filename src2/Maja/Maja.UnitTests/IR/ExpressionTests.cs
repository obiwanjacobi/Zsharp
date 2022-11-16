using System.Linq;
using FluentAssertions;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;
using Xunit;

namespace Maja.UnitTests.IR;

public class ExpressionTests
{
    [Theory]
    [InlineData("42", 42, "I8")]
    [InlineData("256", 256, "I16")]
    [InlineData("65535", 65535, "I32")]
    [InlineData("4294967295", 4294967295, "I64")]
    //[InlineData("42.42", 42.42, "F16")]
    public void ParseNumber(string text, object value, string type)
    {
        var types = IrNumber.ParseNumber(text, out var actualValue);
        actualValue.Should().Be(value);
        types.Where(t => t.Name.Value == type).Should().HaveCount(1);
    }

    [Fact]
    public void ArithmeticLiterals()
    {
        const string code =
            "x := 12 + 30" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Declarations.Should().HaveCount(1);
        var v = program.Root.Declarations[0].As<IrVariableDeclaration>();
        v.Symbol.Name.Value.Should().Be("x");
        v.Initializer!.TypeInferredSymbol.Should().NotBeNull();
        v.Initializer!.ConstantValue.Should().NotBeNull();
        v.Initializer!.ConstantValue!.Value.Should().Be(42);
        v.TypeSymbol.Should().Be(TypeSymbol.I64);
    }

    [Fact]
    public void DiscardAssignment()
    {
        const string code =
            "fn: ()" + Tokens.Eol +
            Tokens.Indent1 + "_ = 42" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Declarations.Should().HaveCount(1);
        var expr = program.Root.Declarations[0].As<IrFunctionDeclaration>()
            .Body.Statements[0].As<IrStatementExpression>()
            .Expression.As<IrExpressionBinary>();
        var left = expr.Left.As<IrExpressionIdentifier>();
        left.IsDiscard.Should().BeTrue();
        var right = expr.Right.As<IrExpressionLiteral>();
        right.ConstantValue!.Value.Should().Be(42);
    }
}
