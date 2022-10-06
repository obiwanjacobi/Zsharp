using System.Collections.Generic;
using Antlr4.Runtime;
using FluentAssertions;
using Maja.Compiler.Parser;
using Xunit;

namespace Maja.UnitTests.Lexer;

public class LexerTests
{
    private static IList<IToken> LexTokens(string code)
        => Maja.Compiler.Compiler.CreateLexer(code, nameof(LexerTests)).GetAllTokens();

    private static void AssertTokens(IEnumerable<IToken> lexedTokens, int[] expectedTokens)
    {
        int i = 0;
        foreach (var token in lexedTokens)
        {
            token.Type.Should().Be(expectedTokens[i]);
            i++;
        }
    }

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

        AssertTokens(tokens, expected);
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
            MajaLexer.Sp,
            MajaLexer.Sp,
            MajaLexer.Ret,
            MajaLexer.Eol,
            MajaLexer.Dedent,
        };

        AssertTokens(tokens, expected);
    }
}
