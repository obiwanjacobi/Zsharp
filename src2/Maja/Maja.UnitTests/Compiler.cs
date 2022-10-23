using Antlr4.Runtime;
using Maja.Compiler.Parser;

namespace Maja.UnitTests;

internal static class Compiler
{
    public static MajaParser CreateParser(string code, string sourceName = "", bool throwOnError = false)
    {
        var lexer = CreateLexer(code, sourceName, throwOnError);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new MajaParser(tokenStream);

        if (throwOnError)
        {
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new ThrowingErrorListener<IToken>());
        }
#if DEBUG
        else
            parser.AddErrorListener(new DiagnosticErrorListener());
#endif

        return parser;
    }

    public static MajaLexer CreateLexer(string code, string sourceName = "", bool throwOnError = false)
    {
        var stream = new AntlrInputStream(code)
        {
            name = sourceName
        };
        var lexer = new MajaLexer(stream)
        {
            WhitespaceMode = Dentlr.WhitespaceMode.Skip
        };
        lexer.InitializeTokens(MajaLexer.Indent, MajaLexer.Dedent, MajaLexer.Eol);

        if (throwOnError)
        {
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new ThrowingErrorListener<int>());
        }

        return lexer;
    }
}