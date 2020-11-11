using Antlr4.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public class AstErrorSite
    {
        private readonly List<AstMessage> _messages = new List<AstMessage>();

        public IEnumerable<AstMessage> Errors => _messages.Where(m => m.MessageType == AstMessageType.Error);

        public bool HasErrors => Errors.Any();

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
            _messages.Add(error);
            return error;
        }
    }
}
