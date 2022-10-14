using FluentAssertions;
using Xunit;
using static Maja.Compiler.Parser.MajaParser;

namespace Maja.UnitTests.Parser;

public class CodeBlockParserTests
{
    private static CompilationUnitContext Parse(string code)
    {
        var parser = Maja.Compiler.Compiler.CreateParser(code);
        var parseTree = parser.compilationUnit();
        return parseTree;
    }

    [Fact]
    public void CommentSingle()
    {
        const string code =
            "#_ comment" + Tokens.EOL
            ;

        var result = Parse(code);
        var nl = result.newline();
        nl.Should().HaveCount(1);
        result.newline()[0].Comment().Should().NotBeNull();
    }

    [Fact]
    public void CommentMultiple()
    {
        const string code =
            "#_ comment 1" + Tokens.EOL +
            "#_ comment 2" + Tokens.EOL +
            "## warning" + Tokens.EOL
            ;

        var result = Parse(code);
        var nl = result.newline();
        nl.Should().HaveCount(3);
        result.newline()[0].Comment().Should().NotBeNull();
    }

    [Fact]
    public void CommentFunctionParameters()
    {
        const string code =
            "fn: (" + Tokens.EOL +
            Tokens.INDENT1 + "#_ comment p1" + Tokens.EOL +
            Tokens.INDENT1 + "p1: U8" + Tokens.EOL +
            Tokens.INDENT1 + "p2: Str" + Tokens.INDENT1 + "#_ comment p2" + Tokens.EOL +
            ")" + Tokens.EOL +
            Tokens.INDENT1 + "ret" + Tokens.EOL
            ;

        var result = Parse(code);
        var fn = result.membersDecl()[0].functionDecl();
        var paramList = fn.parameterList().parameterListIndent();
        var comment1 = paramList.children[1].As<CommentContext>();
        comment1.GetText().Should().Contain("comment p1");
        var comment2 = paramList.children[5].As<NewlineContext>().Comment();
        comment2.Symbol.Text.Should().Contain("comment p2");
    }
}