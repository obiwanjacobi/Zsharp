using FluentAssertions;
using Maja.Compiler.Syntax;
using Maja.UnitTests.Compiler;
using Xunit;

namespace Maja.UnitTests.Compiler.Compilation;

public class CompilationTests
{
    [Fact]
    public void Compilation1()
    {
        const string code =
            "pub qualified.name" + Tokens.Eol
            ;

        var tree = SyntaxTree.Parse(code, "CompilationTests");
        var comp = Maja.Compiler.Compilation.Compilation.Create(tree);
        var model = comp.GetModel(tree);

        model.Should().NotBeNull();
        model.Program.Should().NotBeNull();
        model.SyntaxTree.Should().NotBeNull();
    }
}
