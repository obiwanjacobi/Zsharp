using System.Collections.Generic;
using Antlr4.Runtime;
using Maja.Compiler.Parser;
using Xunit;

namespace Maja.UnitTests.Lexer;

public class LexerTests
{
    private static IList<IToken> LexTokens(string code)
        => Maja.Compiler.Compiler.CreateLexer(code, nameof(LexerTests)).GetAllTokens();

    [Fact]
    public void Pub1()
    {
        const string code =
            "pub qualified.name" + Tokens.EOL
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
            "pub" + Tokens.EOL +
            Tokens.INDENT1 + "qualified.name" + Tokens.EOL
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
            "pub" + Tokens.EOL +
            Tokens.INDENT1 + "qualified.name" + Tokens.EOL +
            Tokens.INDENT1 + "qualified.name" + Tokens.EOL
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
            "fn: ()" + Tokens.EOL +
            Tokens.INDENT1 + "ret" + Tokens.EOL
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
}
