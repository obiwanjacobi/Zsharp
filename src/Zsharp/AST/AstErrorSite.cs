using Antlr4.Runtime;
using System.Collections.Generic;

namespace Zsharp.AST
{
    public class AstErrorSite
    {
        private readonly List<AstError> _errors = new List<AstError>();

        public IEnumerable<AstError> Errors => _errors;

        public bool HasErrors => _errors.Count > 0;

        public AstError AddError(ParserRuleContext context, string text)
            => AddError(text, context);

        public AstError AddError(AstNode node, ParserRuleContext context, string text)
            => AddError(text, context, node);

        private AstError AddError(string text, ParserRuleContext context, AstNode? node = null)
        {
            var error = new AstError(context, node)
            {
                Text = text
            };
            _errors.Add(error);
            return error;
        }
    }
}
