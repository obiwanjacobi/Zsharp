using System;
using System.Linq;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.Syntax;

namespace Maja.UnitTests.Syntax
{
    internal static class Syntax
    {
        public static CompilationUnitSyntax Parse(string code, bool throwOnError = true)
        {
            var parser = Compiler.CreateParser(code, "SyntaxTests", false);
            var parseTree = parser.compilationUnit();

            if (throwOnError)
            {
                var errs = parseTree.Errors().Select(e => e.Text);
                if (errs.Any())
                    throw new Exception(String.Join(Environment.NewLine, errs));
            }

            var builder = new ParserNodeConvertor(nameof(Syntax));
            var syntax = builder.VisitCompilationUnit(parseTree);
            return (CompilationUnitSyntax)syntax[0].Node!;
        }
    }
}
