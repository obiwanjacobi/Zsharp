using FluentAssertions;
using System;
using Zlang.NET.AST;

namespace UnitTests
{
    internal static class Build
    {
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
