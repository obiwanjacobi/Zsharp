using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Maja.Compiler.Parser;

namespace Maja.UnitTests.Compiler.Parser;

public class LexerTests
{
    private static IList<IToken> LexTokens(string code)
        => Compiler.CreateLexer(code, nameof(LexerTests), throwOnError: true).GetAllTokens();

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

    [Theory]
    [MemberData(nameof(GetSingleTokenData))]
    public void LexSingleToken(int kind, string text)
    {
        var tokens = LexTokens(text);
        tokens.Single().Type.Should().Be(kind);
    }

    public static IEnumerable<object[]> GetSingleTokenData()
    {
        foreach (var t in GetSingleTokens())
            yield return new object[] { t.kind, t.text };
    }

    private static IEnumerable<(int kind, string text)> GetSingleTokens()
    {
        return new (int kind, string text)[]
        {
            (MajaLexer.And, "and"),
            (MajaLexer.AngleClose, ">"),
            (MajaLexer.AngleOpen, "<"),
            (MajaLexer.At, "@"),
            (MajaLexer.BackTick, "`"),
            (MajaLexer.BitAnd, "&"),
            (MajaLexer.BitNot, "~"),
            (MajaLexer.BitOr, "|"),
            (MajaLexer.BitRollL, "|<"),
            (MajaLexer.BitRollR, ">|"),
            (MajaLexer.BitShiftL, "<<"),
            (MajaLexer.BitXor_Imm, "^"),
            (MajaLexer.BracketClose, "]"),
            (MajaLexer.BracketOpen, "["),
            (MajaLexer.Brk, "brk"),
            //(MajaLexer.Character, "x"),
            (MajaLexer.CharQuote, "'"),
            (MajaLexer.Cnt, "cnt"),
            (MajaLexer.Colon, ":"),
            (MajaLexer.Comma, ","),
            //(MajaLexer.Comment, "##"),
            //(MajaLexer.Comment, "#_"),
            (MajaLexer.CurlyClose, "}"),
            (MajaLexer.CurlyOpen, "{"),
            //(MajaLexer.Discard, "_"),
            (MajaLexer.Divide, "/"),
            (MajaLexer.Dollar, "$"),
            (MajaLexer.Dot, "."),
            (MajaLexer.Elif, "elif"),
            (MajaLexer.Else, "else"),
            (MajaLexer.Eol, "\n"),
            (MajaLexer.Eol, "\r\n"),
            (MajaLexer.Eq, "="),
            (MajaLexer.Error, "!"),
            (MajaLexer.False, "false"),
            (MajaLexer.GtEq, ">="),
            (MajaLexer.Hash, "#"),
            (MajaLexer.Identifier, "a"),
            (MajaLexer.Identifier, "abc"),
            (MajaLexer.If, "if"),
            (MajaLexer.In, "in"),
            (MajaLexer.Loop, "loop"),
            (MajaLexer.LtEq, "=<"),
            (MajaLexer.Minus, "-"),
            (MajaLexer.Mod, "mod"),
            (MajaLexer.Modulo, "%"),
            (MajaLexer.Multiply, "*"),
            (MajaLexer.Neq, "<>"),
            (MajaLexer.Not, "not"),
            //(MajaLexer.NumberBin, "0b101010"),
            (MajaLexer.NumberDec, "42"),
            //(MajaLexer.NumberDecPrefix, "0d"),
            //(MajaLexer.NumberHex, "0xDEAD"),
            //(MajaLexer.NumberOct, "0c192"),
            (MajaLexer.Or, "or"),
            (MajaLexer.ParenClose, ")"),
            (MajaLexer.ParenOpen, "("),
            (MajaLexer.Plus, "+"),
            (MajaLexer.Power, "**"),
            (MajaLexer.Pub, "pub"),
            (MajaLexer.Question, "?"),
            (MajaLexer.Range, ".."),
            (MajaLexer.Ret, "ret"),
            (MajaLexer.Root, "//"),
            (MajaLexer.Self, "self"),
            (MajaLexer.SemiColon, ";"),
            (MajaLexer.Sp, " "),
            (MajaLexer.Spread, "..."),
            //(MajaLexer.String, "a"),
            //(MajaLexer.String, "abc"),
            //(MajaLexer.String, "abc def"),
            (MajaLexer.StrQuote, "\""),
            (MajaLexer.True, "true"),
            (MajaLexer.Use, "use")
        };
    }
}
