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
            .And.Contain("while (true)")
            ;

        Emit.AssertBuild(emit);
    }

    //[Fact]
    // fails until loop expression tranform is implemented
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
            .Contain(" x ")
            .And.Contain(" a ")
            .And.Contain("for (")
            .And.Contain("x--;")
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
            ;

        Emit.AssertBuild(emit);
    }
}
