using Maja.Compiler.Syntax;

namespace Maja.UnitTests.Syntax
{
    internal static class Syntax
    {
        public static CompilationUnitSyntax Parse(string code)
        {
            var parser = Maja.Compiler.Compiler.CreateParser(code);
            var parseTree = parser.compilation_unit();

            //var errs = parseTree.Errors().Select(e => e.Text);
            //if (errs.Any())
            //    throw new Exception(String.Join(Environment.NewLine, errs));

            var builder = new SyntaxNodeBuilder(nameof(ModuleSyntaxTests));
            var syntax = builder.VisitCompilation_unit(parseTree);
            return (CompilationUnitSyntax)syntax[0];
        }
    }
}
