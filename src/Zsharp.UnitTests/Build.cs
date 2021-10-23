using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Zsharp;
using Zsharp.AST;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.UnitTests
{
    internal static class Build
    {
        public static AstModuleImpl Module(string code, IAstModuleLoader moduleLoader = null)
        {
            var file = ParseFile(code);
            var context = new CompilerContext(moduleLoader ?? new ModuleLoader());
            _ = BuildFile(file, context);
            return (AstModuleImpl)context.Modules.Modules.First();
        }

        public static AstFile File(string code, IAstModuleLoader moduleLoader = null)
        {
            var file = ParseFile(code);
            var context = new CompilerContext(moduleLoader ?? new ModuleLoader());
            return BuildFile(file, context);
        }

        private static AstFile BuildFile(FileContext file, CompilerContext context)
        {
            var builder = new AstBuilder(context);
            var astFile = builder.Build(file, "UnitTests");
            PrintErrors(builder.Errors);
            builder.HasErrors.Should().BeFalse();
            return astFile;
        }

        private static FileContext ParseFile(string code)
        {
            var file = Parser.ParseFile(code);
            var errors = file.Errors();
            PrintErrors(errors);
            errors.Should().BeEmpty();
            return file;
        }

        public static void PrintErrors(this IEnumerable<AstMessage> errors)
        {
            foreach (var err in errors)
            {
                Console.WriteLine(err);
            }
        }
    }
}
