using Antlr4.Runtime;
using System;

namespace Zsharp.AST
{
    public enum AstMessageType
    {
        Trace,
        Information,
        Warning,
        Error
    }

    public class AstMessage
    {
        public const string EmptyCodeBlock = "Empty Code Block (indicates a parse error).";
        public const string IndentationMismatch = "Number of Indentations is mismatched.";
        public const string IndentationInvalid = "Number of Indentation characters is invalid.";
        public const string SyntaxError = "The Syntax is invalid.";

        public AstMessage(AstMessageType messageType, ParserRuleContext context, AstNode? node = null)
        {
            MessageType = messageType;
            Node = node;
            Context = context;
            Text = String.Empty;
            Source = String.Empty;
        }

        public AstMessageType MessageType { get; }

        public ParserRuleContext Context { get; }

        public AstNode? Node { get; }

        public string Text { get; set; }

        public string Source { get; set; }

        public Exception? Error => Context.exception;

        public override string ToString()
            => $"{MessageType}: {Text} at {Context.Start.Line}, {Context.Start.Column + 1} ({Source})";
    }
}