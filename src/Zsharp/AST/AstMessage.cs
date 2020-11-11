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

    public class AstMessage : IEquatable<AstMessage>
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

        public (int Line, int Column) Location => (Context.Start.Line, Context.Start.Column + 1);

        public Exception? Error => Context.exception;

        public bool Equals(AstMessage? other)
        {
            if (other != null)
            {
                return
                    Object.ReferenceEquals(Context, other.Context) &&
                    Object.ReferenceEquals(Node, other.Node) &&
                    String.Compare(Text, other.Text) == 0;
            }
            return false;
        }

        public override string ToString()
            => $"{MessageType}: {Text} at {Location.Line}, {Location.Column} ({Source})";
    }
}