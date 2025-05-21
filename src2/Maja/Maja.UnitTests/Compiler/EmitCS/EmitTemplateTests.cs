namespace Maja.UnitTests.Compiler.EmitCS;

public class EmitTemplateTests
{
    private readonly ITestOutputHelper _output;

    public EmitTemplateTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void FuncTemplate()
    {
        const string code =
            "fn: <#T>(p: T): T" + Tokens.Eol +
            Tokens.Indent1 + "ret p" + Tokens.Eol +
            "v := fn<U8>(42)" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain(" fn(System.Byte")
            .And.Contain("  ")
            .And.Contain(" return ")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void TypeTemplate()
    {
        const string code =
            "Templ<#T>" + Tokens.Eol +
            Tokens.Indent1 + "fld1: T" + Tokens.Eol +
            "s := Templ<U8>" + Tokens.Eol +
            Tokens.Indent1 + "fld1 = 42" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain(" fn()")
            .And.Contain(" void ")
            .And.Contain(" return ")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }
}
