using FluentAssertions;
using Xunit;
using static Maja.Compiler.Parser.MajaParser;

namespace Maja.UnitTests.Parser;

public class ModuleParserTests
{
    private static CompilationUnitContext Parse(string code)
    {
        var parser = Maja.Compiler.Compiler.CreateParser(code);
        var parseTree = parser.compilationUnit();
        return parseTree;
    }

    [Fact]
    public void DeclPubSingle()
    {
        const string code =
            "pub qualified.name" + Tokens.EOL
            ;

        var result = Parse(code);
        result.pubDecl().Should().HaveCount(1);
    }

    [Fact]
    public void DeclPubMultiple()
    {
        const string code =
            "pub qualified.name, qualified.name" + Tokens.EOL
            ;

        var result = Parse(code);
        result.pubDecl().Should().HaveCount(1);
    }

    [Fact]
    public void DeclPubSingleIndent()
    {
        const string code =
            "pub" + Tokens.EOL +
            Tokens.INDENT1 + "qualified.name" + Tokens.EOL
            ;

        var result = Parse(code);
        result.pubDecl().Should().HaveCount(1);
    }

    [Fact]
    public void DeclPubMultipleIndent()
    {
        const string code =
            "pub" + Tokens.EOL +
            Tokens.INDENT1 + "qualified.name" + Tokens.EOL +
            Tokens.INDENT1 + "qualified.name" + Tokens.EOL
            ;

        var result = Parse(code);
        result.pubDecl().Should().HaveCount(1);
    }
}