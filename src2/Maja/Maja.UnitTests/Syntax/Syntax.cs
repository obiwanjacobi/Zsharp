﻿using Maja.Compiler.Syntax;

namespace Maja.UnitTests.Syntax
{
    internal static class Syntax
    {
        public static CompilationUnitSyntax Parse(string code)
        {
            var parser = Maja.Compiler.Compiler.CreateParser(code);
            var parseTree = parser.compilationUnit();

            //var errs = parseTree.Errors().Select(e => e.Text);
            //if (errs.Any())
            //    throw new Exception(String.Join(Environment.NewLine, errs));

            var builder = new ParserNodeConvertor(nameof(ModuleSyntaxTests));
            var syntax = builder.VisitCompilationUnit(parseTree);
            return (CompilationUnitSyntax)syntax[0].Node!;
        }
    }
}
