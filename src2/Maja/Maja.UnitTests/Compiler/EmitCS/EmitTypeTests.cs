namespace Maja.UnitTests.Compiler.EmitCS;

public class EmitTypeTests
{
    private readonly ITestOutputHelper _output;

    public EmitTypeTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void TypeDeclaration()
    {
        const string code =
            "MyType" + Tokens.Eol +
            Tokens.Indent1 + "Field: U8" + Tokens.Eol +
            Tokens.Indent1 + "Name: Str" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain(" Mytype")
            .And.Contain(" System.Byte Field { get; set; }")
            .And.Contain(" System.String Name { get; set; }")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void TypeDeclarationInitialValue()
    {
        const string code =
            "MyType" + Tokens.Eol +
            Tokens.Indent1 + "Field: U8 = 42" + Tokens.Eol +
            Tokens.Indent1 + "Name: Str = \"Test\"" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain(" Mytype")
            .And.Contain(" System.Byte Field { get; set; } = 42")
            .And.Contain(" System.String Name { get; set; } = \"Test\"")
            ;

        Emit.AssertBuild(emit);
    }

    [Fact]
    public void TypeInitializer()
    {
        const string code =
            "MyType" + Tokens.Eol +
            Tokens.Indent1 + "Field: U8" + Tokens.Eol +
            Tokens.Indent1 + "Name: Str" + Tokens.Eol +
            "x := MyType" + Tokens.Eol +
            Tokens.Indent1 + "Field = 42" + Tokens.Eol +
            Tokens.Indent1 + "Name = \"42\"" + Tokens.Eol
            ;

        var emit = Emit.FromCode(code);
        _output.WriteLine(emit);

        emit.Should()
            .Contain(" Mytype")
            .And.Contain(" System.Byte Field { get; set; }")
            .And.Contain(" System.String Name { get; set; }")
            ;

        Emit.AssertBuild(emit);
    }
}