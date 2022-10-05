using Antlr4.Runtime;
using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.Syntax;

namespace Maja.Compiler;

internal static class Extensions
{
    public static T[] Append<T>(this T[] first, T[] last)
    {
        if (last is null || last.Length == 0) return first;

        var arr = new T[first.Length + last.Length];
        first.CopyTo(arr, 0);
        last.CopyTo(arr, first.Length);
        return arr;
    }

    public static IEnumerable<CompilerMessage> Errors(this ParserRuleContext ctx)
    {
        var errors = new List<CompilerMessage>();

        if (ctx.exception is not null)
        {
            var location = SyntaxLocation.From(ctx);
            errors.Add(new CompilerMessage(CompilerMessageKind.Error, location, ctx.exception));
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