using Antlr4.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public class AstErrorSite
    {
        private readonly List<AstMessage> _messages = new();

        public IEnumerable<AstMessage> Errors
            => _messages.Where(m => m.MessageType == AstMessageType.Error);

        public bool HasErrors => Errors.Any();

        public AstMessage AddError(int line, int column, string text)
        {
            var error = new AstMessage(AstMessageType.Error, line, column)
            {
                Text = text
            };
            return AddMessage(error);
        }

        public AstMessage AddError(ParserRuleContext context, string text)
            => AddError(text, context);

        public AstMessage AddError(AstNode node, ParserRuleContext context, string text)
            => AddError(text, context, node);

        private AstMessage AddError(string text, ParserRuleContext context, AstNode? node = null)
        {
            var error = new AstMessage(AstMessageType.Error, context, node)
            {
                Text = text
            };
            return AddMessage(error);
        }

        private AstMessage AddMessage(AstMessage message)
        {
            var duplicate = FindDuplicate(message);
            if (duplicate is null)
            {
                _messages.Add(message);
                return message;
            }
            return duplicate;
        }

        private AstMessage? FindDuplicate(AstMessage message)
        {
            return _messages.SingleOrDefault(m => m.Equals(message));
        }
    }
}
