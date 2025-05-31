using Maja.Compiler.External;

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
            .And.Contain("42 ")
            .And.Contain(" + ")
            .And.Contain("101")
            .And.NotContain("<unknown>")
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
            .And.Contain("42")
            .And.Contain("(DefMod.x + (System.Int64)101)")
            .And.NotContain("<unknown>")
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

        var moduleLoader = new AssemblyManagerBuilder()
            .AddMsCoreLib()
            .AddMaja()
            .ToModuleLoader();

        var emit = Emit.FromCode(code, moduleLoader);
        _output.WriteLine(emit);

        emit.Should()
            .Contain("System.Byte y = (System.Byte)42;")
            .And.Contain("System.Byte x = DefMod.fn(Maja.Operators.ArithmeticAddU8(DefMod.y, (System.Byte)42));")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit, moduleLoader);
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
            .And.Contain("System.Byte x = DefMod.fn((System.Byte)(DefMod.fn(DefMod.y) + (System.Byte)42));")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void ExpressionMemberAccessField()
    {
        const string code =
            "MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol +
            "x := MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld1 = 42" + Tokens.Eol +
            Tokens.Indent1 + "fld2 = \"42\"" + Tokens.Eol +
            "y := x.fld1" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain("y = DefMod.x.fld1;")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void ExpressionMemberAccessFunction()
    {
        const string code =
            "MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol +
            "fn: (): MyType" + Tokens.Eol +
            Tokens.Indent1 + "ret MyType" + Tokens.Eol +
            Tokens.Indent2 + "fld1 = 42" + Tokens.Eol +
            Tokens.Indent2 + "fld2 = \"42\"" + Tokens.Eol +
            "y := fn().fld1" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain("y = DefMod.fn().fld1;")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }
}
