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
}
