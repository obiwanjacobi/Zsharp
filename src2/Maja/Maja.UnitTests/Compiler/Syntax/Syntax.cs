using System;
using System.Linq;
using Maja.Compiler.Syntax;

namespace Maja.UnitTests.Compiler.Syntax;

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
                    .Concat(tree.Diagnostics.Select(diag => $" - {diag.Text}"))
            );
            throw new Exception(errTxt);
        }

        return tree.Root;
    }

    public static string Write(CompilationUnitSyntax root)
    {
        var writer = new SyntaxWriter();
        return writer.Serialize(root);
    }

    public static void RoundTrip(string code, ITestOutputHelper? output = null)
    {
        var tree = Syntax.Parse(code, throwOnError: false);
        var result = Syntax.Write(tree);

        output?.WriteLine(result);
        result.Should().Be(code);
    }
}