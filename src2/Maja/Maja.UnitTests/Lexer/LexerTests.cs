using System.Collections.Generic;
using Antlr4.Runtime;
using Maja.Compiler.Parser;
using Xunit;

namespace Maja.UnitTests.Lexer;

public class LexerTests
{
    private static IList<IToken> LexTokens(string code)
        => Maja.Compiler.Compiler.CreateLexer(code, nameof(LexerTests), throwOnError: true).GetAllTokens();

    [Fact]
    public void Pub1()
    {
        const string code =
            "pub qualified.name" + Tokens.Eol
            ;

        var tokens = LexTokens(code);
        var expected = new[]
        {
            MajaLexer.Pub,
            MajaLexer.Sp,
            MajaLexer.Identifier,
            MajaLexer.Dot,
            MajaLexer.Identifier,
            MajaLexer.Eol
        };

        Tokens.Assert(tokens, expected);
    }

    [Fact]
    public void Pub2()
    {
        const string code =
            "pub" + Tokens.Eol +
            Tokens.Indent1 + "qualified.name" + Tokens.Eol
            ;

        var tokens = LexTokens(code);
        var expected = new[]
        {
            MajaLexer.Pub,
            MajaLexer.Eol,
            MajaLexer.Indent,
            MajaLexer.Identifier,
            MajaLexer.Dot,
            MajaLexer.Identifier,
            MajaLexer.Eol,
            MajaLexer.Dedent
        };

        Tokens.Assert(tokens, expected);
    }

    [Fact]
    public void Pub3()
    {
        const string code =
            "pub" + Tokens.Eol +
            Tokens.Indent1 + "qualified.name" + Tokens.Eol +
            Tokens.Indent1 + "qualified.name" + Tokens.Eol
            ;

        var tokens = LexTokens(code);
        var expected = new[]
        {
            MajaLexer.Pub,
            MajaLexer.Eol,
            MajaLexer.Indent,
            MajaLexer.Identifier,
            MajaLexer.Dot,
            MajaLexer.Identifier,
            MajaLexer.Eol,
            MajaLexer.Identifier,
            MajaLexer.Dot,
            MajaLexer.Identifier,
            MajaLexer.Eol,
            MajaLexer.Dedent
        };

        Tokens.Assert(tokens, expected);
    }

    [Fact]
    public void FunctionDecl()
    {
        const string code =
            "fn: ()" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
            ;

        var tokens = LexTokens(code);
        var expected = new[]
        {
            MajaLexer.Identifier,
            MajaLexer.Colon,
            MajaLexer.Sp,
            MajaLexer.ParenOpen,
            MajaLexer.ParenClose,
            MajaLexer.Eol,
            MajaLexer.Indent,
            MajaLexer.Ret,
            MajaLexer.Eol,
            MajaLexer.Dedent,
        };

        Tokens.Assert(tokens, expected);
    }

    [Fact]
    public void CommentFunctionParameters()
    {
        const string code =
            "fn: (" + Tokens.Eol +
            Tokens.Indent1 + "#_ comment p1" + Tokens.Eol +
            Tokens.Indent1 + "p1: U8" + Tokens.Eol +
            Tokens.Indent1 + "p2: Str" + Tokens.Indent1 + "#_ comment p2" + Tokens.Eol +
            ")" + Tokens.Eol
            ;

        var tokens = LexTokens(code);
        var expected = new[]
        {
            MajaLexer.Identifier,
            MajaLexer.Colon,
            MajaLexer.Sp,
            MajaLexer.ParenOpen,
            MajaLexer.Eol,
            MajaLexer.Indent,
            MajaLexer.Comment,
            MajaLexer.Eol,
            MajaLexer.Identifier,
            MajaLexer.Colon,
            MajaLexer.Sp,
            MajaLexer.Identifier,
            MajaLexer.Eol,
            MajaLexer.Identifier,
            MajaLexer.Colon,
            MajaLexer.Sp,
            MajaLexer.Identifier,
            MajaLexer.Sp,
            MajaLexer.Sp,
            MajaLexer.Comment,
            MajaLexer.Eol,
            MajaLexer.Dedent,
            MajaLexer.ParenClose,
            MajaLexer.Eol,
        };

        Tokens.Assert(tokens, expected);
    }

    [Fact]
    public void FunctionInvocation()
    {
        const string code =
            "fn(42)" + Tokens.Eol;

        var tokens = LexTokens(code);
        var expected = new[]
        {
            MajaLexer.Identifier,
            MajaLexer.ParenOpen,
            MajaLexer.NumberDec,
            MajaLexer.ParenClose,
            MajaLexer.Eol,
        };

        Tokens.Assert(tokens, expected);
    }
}
