using Antlr4.Runtime;
using System.Collections.Generic;
using System.Linq;
using Zsharp.AST;

namespace Zsharp
{
    public static class ParserExtensions
    {
        public static IEnumerable<AstMessage> Errors(this ParserRuleContext ctx)
        {
            var errors = new List<AstMessage>();

            if (ctx.exception is not null)
                errors.Add(new AstMessage(AstMessageType.Error, ctx));

            if (ctx.children is not null)
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
