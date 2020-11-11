using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Zsharp;
using Zsharp.AST;

namespace UnitTests
{
    internal static class Build
    {
        public static AstModulePublic Module(string code, IAstModuleLoader moduleLoader = null)
        {
            var file = Parser.ParseFile(code);
            var errors = file.Errors();
            PrintErrors(errors);
            errors.Should().BeEmpty();

            var context = new CompilerContext(moduleLoader ?? new ModuleLoader());
            var builder = new AstBuilder(context);
            builder.Build(file, "UnitTests");
            PrintErrors(builder.Errors);
            builder.HasErrors.Should().BeFalse();
            return (AstModulePublic)context.Modules.Modules.First();
        }

        public static AstFile File(string code, IAstModuleLoader moduleLoader = null)
        {
            var file = Parser.ParseFile(code);
            var errors = file.Errors();
            PrintErrors(errors);
            errors.Should().BeEmpty();

            var builder = new AstBuilder(new CompilerContext(moduleLoader ?? new ModuleLoader()));
            var astFile = builder.Build(file, "UnitTests");
            PrintErrors(builder.Errors);
            builder.HasErrors.Should().BeFalse();
            return astFile;
        }

        private static void PrintErrors(IEnumerable<AstError> errors)
        {
            foreach (var err in errors)
            {
                Console.WriteLine(err);
            }
        }
    }
}
