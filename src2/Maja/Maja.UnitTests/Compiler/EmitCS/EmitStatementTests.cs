namespace Maja.UnitTests.Compiler.EmitCS;

public class EmitStatementTests
{
    private readonly ITestOutputHelper _output;

    public EmitStatementTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void StatementIf()
    {
        const string code =
            "x := 42" + Tokens.Eol + 
            "a := 0" + Tokens.Eol +
            "if x = 42" + Tokens.Eol +
            Tokens.Indent1 + "a = 42" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain(" x ")
            .And.Contain(" a ")
            .And.Contain("if (")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void StatementIfElse()
    {
        const string code =
            "x := 42" + Tokens.Eol +
            "a := 0" + Tokens.Eol +
            "if x = 42" + Tokens.Eol +
            Tokens.Indent1 + "a = 42" + Tokens.Eol +
            "else" + Tokens.Eol +
            Tokens.Indent1 + "a = 101" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain(" x ")
            .And.Contain(" a ")
            .And.Contain("if (")
            .And.Contain("else")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void StatementIfElseIfElse()
    {
        const string code =
            "x := 42" + Tokens.Eol +
            "a := 0" + Tokens.Eol +
            "if x = 42" + Tokens.Eol +
            Tokens.Indent1 + "a = 42" + Tokens.Eol +
            "else if x < 100" + Tokens.Eol +
            Tokens.Indent1 + "a = 101" + Tokens.Eol +
            "else" + Tokens.Eol +
            Tokens.Indent1 + "a = 1" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain(" x ")
            .And.Contain(" a ")
            .And.Contain("if (")
            .And.Contain("else if (")
            .And.Contain("else")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void StatementLoop()
    {
        const string code =
            "a := 0" + Tokens.Eol +
            "loop" + Tokens.Eol +
            Tokens.Indent1 + "a = a + 1" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain(" a ")
            .And.Contain("while ((System.Boolean)true)")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void StatementLoop_ForRange()
    {
        const string code =
            "a := 0" + Tokens.Eol +
            "loop [0..9]" + Tokens.Eol +
            Tokens.Indent1 + "a = a + 1" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain("for (")
            .And.Contain("Int32 __i = 0")
            .And.Contain("__i < 9")
            .And.Contain("__i = (System.Int32)(__i + (System.Int32)1)")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void StatementLoop_Literal()
    {
        const string code =
            "a := 0" + Tokens.Eol +
            "loop 42" + Tokens.Eol +
            Tokens.Indent1 + "a = a + 1" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain("for (")
            .And.Contain("__i = ")
            .And.Contain("__i < 42")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void StatementLoop_For()
    {
        const string code =
            "a := 0" + Tokens.Eol +
            "x: I32 = 42" + Tokens.Eol +
            "loop x" + Tokens.Eol +
            Tokens.Indent1 + "a = a + 1" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain("for (")
            .And.Contain("__x = ")
            .And.Contain("__x < x")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void StatementLoop_While()
    {
        const string code =
            "a := 0" + Tokens.Eol +
            "loop a < 42" + Tokens.Eol +
            Tokens.Indent1 + "a = a + 1" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain(" a ")
            .And.Contain("while (")
            .And.Contain("(a < 42)")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }
}
