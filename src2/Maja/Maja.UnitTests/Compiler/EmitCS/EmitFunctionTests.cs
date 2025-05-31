namespace Maja.UnitTests.Compiler.EmitCS;

public class EmitFunctionTests
{
    private readonly ITestOutputHelper _output;

    public EmitFunctionTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Function()
    {
        const string code =
            "fn: ()" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
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

    [Fact]
    public void FunctionParameters()
    {
        const string code =
            "fn: (p1: U8, p2: Str)" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain(" fn(")
            .And.Contain("System.Byte p1")
            .And.Contain("System.String p2")
            .And.Contain(" System.String ")
            .And.Contain(" void ")
            .And.Contain(" return")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void FunctionTypeParameters()
    {
        const string code =
            "fn: <T>(p1: T)" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain(" fn")
            .And.Contain("<T>")
            .And.Contain("T p1")
            .And.Contain(" void ")
            .And.Contain(" return")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void FunctionReturnValue()
    {
        const string code =
            "fn: (): Str" + Tokens.Eol +
            Tokens.Indent1 + "ret \"42\"" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain(" fn(")
            .And.Contain(" System.String ")
            .And.Contain(" return \"42\"")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void FunctionInvocation()
    {
        const string code =
            "fn: (): U8" + Tokens.Eol +
            Tokens.Indent1 + "ret 42" + Tokens.Eol +
            "x := fn()" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain(" fn()")
            .And.Contain(" System.Byte ")
            .And.Contain(" return 42;")
            .And.Contain(" x = DefMod.fn();")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void FunctionInvocationParams()
    {
        const string code =
            "fn: (p: U8): U8" + Tokens.Eol +
            Tokens.Indent1 + "ret p" + Tokens.Eol +
            "x := fn(42)" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain("System.Byte fn(")
            .And.Contain(" System.Byte ")
            .And.Contain(" return p;")
            .And.Contain(" x = DefMod.fn((System.Byte)42);")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void FunctionInvocationTypeParams()
    {
        const string code =
            "fn: <T>(p: T): T" + Tokens.Eol +
            Tokens.Indent1 + "ret p" + Tokens.Eol +
            "x := fn<U8>(42)" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain("T fn<T>(T p)")
            .And.Contain(" return p;")
            .And.Contain(" x = DefMod.fn<System.Byte>(")
            .And.Contain("42);")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void FunctionPublicExport()
    {
        const string code =
            "pub fn: ()" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain("public static class DefMod")
            .And.Contain("public static void fn()")
            .And.NotContain("<unknown>")
            ;

        Emit.AssertBuild(emit);
    }
}
