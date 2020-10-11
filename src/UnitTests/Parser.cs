using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using static ZsharpParser;

namespace UnitTests
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
        public static ZsharpParser Create(string sourceCode)
        {
            var charStream = CharStreams.fromstring(sourceCode);
            var tokenStream = new CommonTokenStream(new ZsharpLexer(charStream));
            return new ZsharpParser(tokenStream);
        }

        public static FileContext ParseFile(string sourceCode)
        {
            var parser = Create(sourceCode);
            return parser.file();
        }

        public static string? ParseForError(string sourceCode)
        {
            var parser = Create(sourceCode);
            var file = parser.file();

            var errs = file.Errors().Select(e => e.Message);
            if (errs.Any())
                return String.Join(Environment.NewLine, errs);
            return null;
        }

        public static IEnumerable<Exception> Errors(this ParserRuleContext ctx)
        {
            var errors = new List<Exception>();

            if (ctx.exception != null)
                errors.Add(ctx.exception);

            if (ctx.children != null)
            {
                foreach (var c in ctx.children
                    .OfType<ParserRuleContext>())
                {
                    var childErrs = c.Errors();
                    if (childErrs.Any())
                        errors.AddRange(childErrs);
                }
            }
            return errors;
        }
    }
}
