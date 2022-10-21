using FluentAssertions;
using Xunit;
using static Maja.Compiler.Parser.MajaParser;

namespace Maja.UnitTests.Parser;

public class CodeBlockParserTests
{
    private static CompilationUnitContext Parse(string code)
    {
        var parser = Maja.Compiler.Compiler.CreateParser(code, nameof(CodeBlockParserTests), throwOnError: true);
        var parseTree = parser.compilationUnit();
        return parseTree;
    }

    [Fact]
    public void CommentSingle()
    {
        const string code =
            "#_ comment" + Tokens.Eol
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
            "#_ comment 1" + Tokens.Eol +
            "#_ comment 2" + Tokens.Eol +
            "## warning" + Tokens.Eol
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
            "fn: (" + Tokens.Eol +
            Tokens.Indent1 + "#_ comment p1" + Tokens.Eol +
            Tokens.Indent1 + "p1: U8" + Tokens.Eol +
            Tokens.Indent1 + "p2: Str" + Tokens.Indent1 + "#_ comment p2" + Tokens.Eol +
            ")" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
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