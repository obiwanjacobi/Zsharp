using Antlr4.Runtime;
using System;

namespace Zsharp.AST
{
    public class AstError
    {
        public const string EmptyCodeBlock = "Empty Code Block (indicates a parse error).";
        public const string IndentationMismatch = "Number of Indentations is mismatched.";
        public const string IndentationInvalid = "Number of Indentation characters is invalid.";
        public const string SyntaxError = "The Syntax is invalid.";

        public AstError(ParserRuleContext context, AstNode? node = null)
        {
            Node = node;
            Context = context;
            Text = String.Empty;
        }

        public ParserRuleContext Context { get; }

        public AstNode? Node { get; }

        public string Text { get; set; }

        public Exception? Error => Context.exception;
    }
}