using System.Linq;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;
using Maja.UnitTests.Compiler.IR;

namespace Maja.UnitTests.Compiler.EmitCS;

public class EmitExpressionTests
{
    private readonly ITestOutputHelper _output;

    public EmitExpressionTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ExpressionLiteral()
    {
        const string code =
            "x := 42 + 101" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain(" x ")
            .And.Contain(" = ")
            .And.Contain(" 42 ")
            .And.Contain(" + ")
            .And.Contain(" 101")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void ExpressionIdentifer()
    {
        const string code =
            "x := 42" + Tokens.Eol +
            "y := x + 101" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain(" x ")
            .And.Contain(" y ")
            .And.Contain(" = ")
            .And.Contain(" 42")
            .And.Contain(" x + 101")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void ExpressionInvocationTypeBinaryOperator()
    {
        const string code =
            "fn: (p: U8): U8" + Tokens.Eol +
            Tokens.Indent1 + "ret p" + Tokens.Eol +
            "y: U8 = 42" + Tokens.Eol +
            "x := fn(y + 42)" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain("System.Byte y = (System.Byte)42;")
            .And.Contain("System.Byte x = fn((System.Byte)(y + (System.Byte)42));")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void ExpressionInvocationNestedTypeBinaryOperator()
    {
        const string code =
            "fn: (p: U8): U8" + Tokens.Eol +
            Tokens.Indent1 + "ret p" + Tokens.Eol +
            "y: U8 = 42" + Tokens.Eol +
            "x := fn(fn(y) + 42)" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain("System.Byte y = (System.Byte)42;")
            .And.Contain("System.Byte x = fn((System.Byte)(fn(y) + (System.Byte)42));")
            ;

        Emit.AssertBuild(emit);
    }
}
