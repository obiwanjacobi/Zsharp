using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.Syntax;

namespace Maja.UnitTests.Compiler.Syntax;

internal static class Syntax
{
    public static CompilationUnitSyntax Parse(string code, bool throwOnError = true, [CallerMemberName] string sourceName = "")
    {
        var tree = SyntaxTree.Parse(code, sourceName);

        if (throwOnError &&
            (tree.Root.HasError || 
            tree.Diagnostics.Has(DiagnosticMessageKind.Error) || tree.Diagnostics.Has(DiagnosticMessageKind.Critical)))
        {
            var errTxt = String.Join(Environment.NewLine,
                tree.Root.GetErrors().Select(err => err.Text)
                    .Concat(tree.Diagnostics.Select(diag => $" - {diag.Text}"))
            );
            throw new Exception(errTxt);
        }

        return tree.Root;
    }

    public static SyntaxTree ParseCore(string code, [CallerMemberName] string sourceName = "")
    {
        var tree = SyntaxTree.Parse(code, sourceName);
        return tree;
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