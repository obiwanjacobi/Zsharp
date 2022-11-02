using FluentAssertions;
using Maja.Compiler.Syntax;
using Xunit;

namespace Maja.UnitTests.Compilation;

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
    }
}
