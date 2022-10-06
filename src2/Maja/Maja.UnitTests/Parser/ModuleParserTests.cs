using FluentAssertions;
using Xunit;
using static Maja.Compiler.Parser.MajaParser;

namespace Maja.UnitTests.Parser;

public class ModuleParserTests
{
    private static CompilationUnitContext Parse(string code)
    {
        var parser = Maja.Compiler.Compiler.CreateParser(code);
        var parseTree = parser.compilationUnit();
        return parseTree;
    }

    [Fact]
    public void DeclPub1()
    {
        const string code =
            "pub qualified.name" + Tokens.EOL
            ;

        var result = Parse(code);
        result.pub1Decl().Should().NotBeNull();
    }
}