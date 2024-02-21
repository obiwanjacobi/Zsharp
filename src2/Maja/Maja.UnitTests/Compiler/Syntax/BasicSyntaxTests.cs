namespace Maja.UnitTests.Compiler.Syntax;

public class BasicSyntaxTests
{
    private readonly ITestOutputHelper _output;

    public BasicSyntaxTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void EndOfFile()
    {
        const string code =
            "fn1: ()" + Tokens.Eol +
            Tokens.Indent1 + "ret"
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void FunctionDeclaration_Comments()
    {
        const string code =
            "fn1: ()    #_ Comment1" + Tokens.Eol +
            Tokens.Indent1 + "## Warning" + Tokens.Eol +
            Tokens.Indent1 + "ret       #_ nothing" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void FunctionDeclaration_ExtraNewLines()
    {
        const string code =
            "fn1: ()" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol +
            Tokens.Eol +
            Tokens.Eol +
            "fn2: ()" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();

        //Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void FunctionDeclaration_ParamsIndent_Comments()
    {
        const string code =
            "fn1: (   #_ comment1" + Tokens.Eol +
            Tokens.Indent1 + "p: U8 #_ comment2" + Tokens.Eol +
            ")    #_ Comment3" + Tokens.Eol +
            Tokens.Indent1 + "## Warning" + Tokens.Eol +
            Tokens.Indent1 + "ret       #_ nothing" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void TypeDeclaration_Comments()
    {
        const string code =
            "MyType  #_ Comment1" + Tokens.Eol +
            Tokens.Indent1 + "## Warning" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8  #_ 1st" + Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void TypeDeclaration_ExtraNewLines()
    {
        const string code =
            "MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol +
            Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol +
            Tokens.Eol +
            Tokens.Eol +
            "YourType" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();

        //Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void VariableDeclaration_Comments()
    {
        const string code =
            "#_ comment1" + Tokens.Eol +
            "x := 42        ## warning" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void VariableDeclaration_ExtraNewLines()
    {
        const string code =
            "x := 42" + Tokens.Eol +
            Tokens.Eol +
            Tokens.Eol +
            "y := 101" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void Declarations_ExtraNewLines()
    {
        const string code =
            "MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol +
            Tokens.Eol +
            "fn: (p: U8): Bool" + Tokens.Eol +
            Tokens.Indent1 + "ret p = 42" + Tokens.Eol +
            Tokens.Eol +
            "x := MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld1 = 42" + Tokens.Eol +
            Tokens.Indent1 + "fld2 = \"42\"" + Tokens.Eol +
            Tokens.Eol +
            "b := fn(x.fld1)" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();

        //Syntax.RoundTrip(code, _output);
    }
}