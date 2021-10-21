using Antlr4.Runtime;
using System;
using System.IO;
using System.Linq;
using Zsharp;
using Zsharp.Parser;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.UnitTests
{
    internal static class Tokens
    {
        public const string NewLine = "\n";
        public const string Indent1 = "    ";
        public const string Indent2 = "        ";
        public const string Indent3 = "            ";
    }

    internal static class Parser
    {
        public enum ErrorMode
        {
            Passive,
            Active
        }

        public static ZsharpParser Create(string sourceCode,
            ErrorMode errorMode = ErrorMode.Active)
        {
            var stream = new AntlrInputStream(sourceCode);
            var lexer = new ZsharpLexer(stream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ZsharpParser(tokenStream);

            if (errorMode == ErrorMode.Active)
            {
                lexer.RemoveErrorListeners();
                lexer.AddErrorListener(new ThrowingErrorListener<int>());
                parser.RemoveErrorListeners();
                parser.AddErrorListener(new ThrowingErrorListener<IToken>());
            }

            return parser;
        }

        public static FileContext ParseFile(string sourceCode)
        {
            var parser = Create(sourceCode);
            return parser.file();
        }

        public static string ParseForError(string sourceCode)
        {
            var parser = Create(sourceCode, ErrorMode.Passive);
            var file = parser.file();

            var errs = file.Errors().Select(e => e.Error.Message);
            if (errs.Any())
                return String.Join(Environment.NewLine, errs);
            return null;
        }
    }

    internal class ThrowingErrorListener<TSymbol> : IAntlrErrorListener<TSymbol>
    {
        public void SyntaxError(TextWriter output, IRecognizer recognizer,
            TSymbol offendingSymbol, int line, int charPositionInLine, string msg,
                RecognitionException e)
        {
            throw new Exception($"Syntax Error: {msg} ({line}:{charPositionInLine}).", e);
        }
    }
}
