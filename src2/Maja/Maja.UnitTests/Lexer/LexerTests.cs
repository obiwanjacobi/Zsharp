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
            MajaLexer.PUB,
            MajaLexer.SP,
            MajaLexer.IDENTIFIER,
            MajaLexer.DOT,
            MajaLexer.IDENTIFIER,
            MajaLexer.EOL
        };

        AssertTokens(tokens, expected);
    }
}
