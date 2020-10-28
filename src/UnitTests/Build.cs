using FluentAssertions;
using System;
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

            var builder = new AstBuilder();
            builder.Build(file);
            builder.HasErrors.Should().BeFalse();
            return builder.Modules.First();
        }

        public static AstFile File(string code)
        {
            var file = Parser.ParseFile(code);
            var errors = file.Errors();
            errors.Should().BeEmpty();

            var builder = new AstBuilder();
            var astFile = builder.BuildFile(String.Empty, file);
            builder.HasErrors.Should().BeFalse();
            return astFile;
        }
    }
}
