using static Maja.Compiler.Parser.MajaParser;

namespace Maja.UnitTests.Compiler.Parser;

public class ModuleParserTests
{
    private static CompilationUnitContext Parse(string code)
    {
        var parser = Compiler.CreateParser(code, nameof(ModuleParserTests), throwOnError: true);
        var parseTree = parser.compilationUnit();
        return parseTree;
    }

    [Fact]
    public void DeclPubSingle()
    {
        const string code =
            "pub qualified.name" + Tokens.Eol
            ;

        var result = Parse(code);
        result.directivePub().Should().HaveCount(1);
    }

    [Fact]
    public void DeclPubMultiple()
    {
        const string code =
            "pub qualified.name, qualified.name" + Tokens.Eol
            ;

        var result = Parse(code);
        result.directivePub().Should().HaveCount(1);
    }

    [Fact]
    public void DeclPubSingleIndent()
    {
        const string code =
            "pub" + Tokens.Eol +
            Tokens.Indent1 + "qualified.name" + Tokens.Eol
            ;

        var result = Parse(code);
        result.directivePub().Should().HaveCount(1);
    }

    [Fact]
    public void DeclPubMultipleIndent()
    {
        const string code =
            "pub" + Tokens.Eol +
            Tokens.Indent1 + "qualified.name" + Tokens.Eol +
            Tokens.Indent1 + "qualified.name" + Tokens.Eol
            ;

        var result = Parse(code);
        result.directivePub().Should().HaveCount(1);
    }
}