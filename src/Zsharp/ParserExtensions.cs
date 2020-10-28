using Antlr4.Runtime;
using System.Collections.Generic;
using System.Linq;
using Zsharp.AST;

namespace Zsharp
{
    public static class ParserExtensions
    {
        public static IEnumerable<AstError> Errors(this ParserRuleContext ctx)
        {
            var errors = new List<AstError>();

            if (ctx.exception != null)
                errors.Add(new AstError(ctx));

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
