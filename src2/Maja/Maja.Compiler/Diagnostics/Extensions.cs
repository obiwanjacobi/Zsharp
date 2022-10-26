using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Diagnostics;

internal static class Extensions
{
    public static IEnumerable<DiagnosticMessage> Errors(this ParserRuleContext ctx)
    {
        var errors = new List<DiagnosticMessage>();

        if (ctx.exception is not null)
        {
            var location = SyntaxLocation.From(ctx);
            errors.Add(new DiagnosticMessage(location, ctx.exception));
        }

        if (ctx.children is not null)
        {
            foreach (var c in ctx.children.OfType<ParserRuleContext>())
            {
                var childErrs = c.Errors();
                if (childErrs.Any())
                    errors.AddRange(childErrs);
            }
        }
        return errors;
    }
}