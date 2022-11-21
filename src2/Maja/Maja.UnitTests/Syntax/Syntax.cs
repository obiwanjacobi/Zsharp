using System;
using System.Linq;
using Maja.Compiler.Syntax;

namespace Maja.UnitTests.Syntax;

internal static class Syntax
{
    public static CompilationUnitSyntax Parse(string code, bool throwOnError = true)
    {
        var tree = SyntaxTree.Parse(code, "SyntaxTests");

        if (throwOnError &&
            (tree.Root.HasError || tree.Diagnostics.Any()))
        {
            var errTxt = String.Join(Environment.NewLine,
                tree.Root.GetErrors().Select(err => err.Text)
                    .Concat(tree.Diagnostics.Select(diag => diag.Text))
            );
            throw new Exception(errTxt);
        }

        return tree.Root;
    }
}
