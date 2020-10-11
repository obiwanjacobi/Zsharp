using System;
using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstError
    {
        public const string EmptyCodeBlock = "Empty Code Block (indicates a parse error).";
        public const string IndentationMismatch = "Number of Indentations is mismatched.";
        public const string IndentationInvalid = "Number of Indentation character is invalid.";

        public AstError(ParserRuleContext ctx)
        {
            Context = ctx;
            Text = String.Empty;
        }

        public ParserRuleContext Context {get;}

        public string Text { get; set; }
    }
}