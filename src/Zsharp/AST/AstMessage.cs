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

    public sealed class AstMessage : IEquatable<AstMessage>
    {
        public AstMessage(AstMessageType messageType, ParserRuleContext context, AstNode? node = null)
        {
            MessageType = messageType;
            Node = node;
            Context = context;
            Text = String.Empty;
            Source = String.Empty;
        }

        public AstMessage(AstMessageType messageType, int line, int column)
        {
            MessageType = messageType;
            _location = (line, column);
            Text = String.Empty;
            Source = String.Empty;
        }

        public AstMessageType MessageType { get; }

        public ParserRuleContext? Context { get; }

        public AstNode? Node { get; }

        public string Text { get; set; }

        public string Source { get; set; }

        private (int Line, int Column)? _location;
        public (int Line, int Column) Location
        {
            get
            {
                if (_location != null)
                    return _location.Value;

                if (Context != null)
                    return (Context.Start.Line, Context.Start.Column + 1);

                return (0, 0);
            }
        }

        public Exception? Error => Context?.exception;

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