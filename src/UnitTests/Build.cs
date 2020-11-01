using FluentAssertions;
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
            errors.Should().BeEmpty();

            var context = new CompilerContext(moduleLoader ?? new ModuleLoader());
            var builder = new AstBuilder(context);
            builder.Build(file, "UnitTests");
            builder.HasErrors.Should().BeFalse();
            return (AstModulePublic)context.Modules.Modules.First();
        }

        public static AstFile File(string code, IAstModuleLoader moduleLoader = null)
        {
            var file = Parser.ParseFile(code);
            var errors = file.Errors();
            errors.Should().BeEmpty();

            var builder = new AstBuilder(new CompilerContext(moduleLoader ?? new ModuleLoader()));
            var astFile = builder.Build(file, "UnitTests");
            builder.HasErrors.Should().BeFalse();
            return astFile;
        }
    }
}
