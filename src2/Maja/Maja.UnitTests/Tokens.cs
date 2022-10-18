using Antlr4.Runtime;
using FluentAssertions;
using System.Collections.Generic;

namespace Maja.UnitTests;

internal static class Tokens
{
    public const string Eol = "\n";
    public const string Indent1 = "  ";
    public const string Indent2 = "    ";
    public const string Indent3 = "      ";

    public static void Assert(IEnumerable<IToken> lexedTokens, int[] expectedTokens)
    {
        int i = 0;
        foreach (var token in lexedTokens)
        {
            token.Type.Should().Be(expectedTokens[i], "at index {0}", i);
            i++;
        }
    }
}
