using System.Linq;

namespace Maja.UnitTests.Compiler.Eval;

public class StatementTests
{
    [Fact]
    public void If()
    {
        const string code =
            "x: U8 = 0" + Tokens.Eol +
            "if x = 0" + Tokens.Eol +
            Tokens.Indent1 + "x = 42" + Tokens.Eol
            ;

        var result = Eval.Run(code);
        result.TryLookupVariable("Defmod.x", out var value).Should().BeTrue();
        value.Should().Be(42);
    }

    [Fact]
    public void IfElse()
    {
        const string code =
            "x: U8 = 0" + Tokens.Eol +
            "if x <> 0" + Tokens.Eol +
            Tokens.Indent1 + "x = 101" + Tokens.Eol +
            "else" + Tokens.Eol +
            Tokens.Indent1 + "x = 42" + Tokens.Eol
            ;

        var result = Eval.Run(code);
        result.TryLookupVariable("Defmod.x", out var value).Should().BeTrue();
        value.Should().Be(42);
    }

    [Fact]
    public void IfElseIf()
    {
        const string code =
            "x: U8 = 101" + Tokens.Eol +
            "if x = 0" + Tokens.Eol +
            Tokens.Indent1 + "x = 101" + Tokens.Eol +
            "else if x > 100" + Tokens.Eol +
            Tokens.Indent1 + "x = 42" + Tokens.Eol
            ;

        var result = Eval.Run(code);
        result.TryLookupVariable("Defmod.x", out var value).Should().BeTrue();
        value.Should().Be(42);
    }

    [Fact]
    public void Loop()
    {
        const string code =
            "x: U8 = 0" + Tokens.Eol +
            "loop" + Tokens.Eol +
            Tokens.Indent1 + "x = x + 1" + Tokens.Eol
            ;

        var result = Eval.Run(code, allowError: true);
        result.Diagnostics.First().Should().Contain("endless loops");
    }

    [Fact]
    public void LoopCountLiteral()
    {
        const string code =
            "x: U8 = 0" + Tokens.Eol +
            "loop 42" + Tokens.Eol +
            Tokens.Indent1 + "x = x + 1" + Tokens.Eol
            ;

        var result = Eval.Run(code);
        result.TryLookupVariable("Defmod.x", out var value).Should().BeTrue();
        value.Should().Be(42);
    }

    [Fact]
    public void LoopCountVariable()
    {
        const string code =
            "x: U8 = 0" + Tokens.Eol +
            "c: I32 = 42" + Tokens.Eol +
            "loop c" + Tokens.Eol +
            Tokens.Indent1 + "x = x + 1" + Tokens.Eol
            ;

        var result = Eval.Run(code);
        result.TryLookupVariable("Defmod.x", out var value).Should().BeTrue();
        value.Should().Be(42);
    }

    [Fact]
    public void LoopRangeLiteral()
    {
        const string code =
            "x: U8 = 0" + Tokens.Eol +
            "loop [0..42]" + Tokens.Eol +
            Tokens.Indent1 + "x = x + 1" + Tokens.Eol
            ;

        var result = Eval.Run(code);
        result.TryLookupVariable("Defmod.x", out var value).Should().BeTrue();
        value.Should().Be(42);
    }

    [Fact]
    public void LoopRangeVariables()
    {
        const string code =
            "x: U8 = 0" + Tokens.Eol +
            "s: I32 = 0" + Tokens.Eol +
            "e: I32 = 42" + Tokens.Eol +
            "loop [s..e]" + Tokens.Eol +
            Tokens.Indent1 + "x = x + 1" + Tokens.Eol
            ;

        var result = Eval.Run(code);
        result.TryLookupVariable("Defmod.x", out var value).Should().BeTrue();
        value.Should().Be(42);
    }
}
