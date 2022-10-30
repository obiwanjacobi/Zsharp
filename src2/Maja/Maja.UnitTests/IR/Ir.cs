using System.Runtime.CompilerServices;
using FluentAssertions;
using Maja.Compiler.IR;
using Maja.Compiler.Syntax;

namespace Maja.UnitTests.IR;

internal static class Ir
{
    public static IrProgram Build(string code, bool allowError = false, [CallerMemberName] string source = "")
    {
        var tree = SyntaxTree.Parse(code, source);
        var program = IrBuilder.Program(tree);
        if (!allowError)
        {
            program.Diagnostics.Should().BeEmpty();
            program.Syntax.Should().Be(tree.Root);
        }
        return program;
    }
}
