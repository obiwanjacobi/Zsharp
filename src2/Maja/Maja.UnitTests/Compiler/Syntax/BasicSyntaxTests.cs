namespace Maja.UnitTests.Compiler.Syntax;

public class BasicSyntaxTests
{
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
    }

    [Fact]
    public void TypeDeclaration_ExtraNewLines()
    {
        const string code =
            "MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol +
            Tokens.Eol +
            Tokens.Eol +
            "YourType" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Should().NotBeNull();

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
    }
}