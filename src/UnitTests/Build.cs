using FluentAssertions;
using System.Linq;
using Zsharp;
using Zsharp.AST;

namespace UnitTests
{
    internal static class Build
    {
        public static AstModule Module(string code)
        {
            var file = Parser.ParseFile(code);
            var errors = file.Errors();
            errors.Should().BeEmpty();

            var context = new CompilerContext();
            var builder = new AstBuilder(context);
            builder.Build(file, "UnitTests");
            builder.HasErrors.Should().BeFalse();
            return context.Modules.Modules.First();
        }

        public static AstFile File(string code)
        {
            var file = Parser.ParseFile(code);
            var errors = file.Errors();
            errors.Should().BeEmpty();

            var builder = new AstBuilder(new CompilerContext());
            var astFile = builder.Build(file, "UnitTests");
            builder.HasErrors.Should().BeFalse();
            return astFile;
        }
    }
}
