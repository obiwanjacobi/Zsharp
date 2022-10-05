using FluentAssertions;
using Xunit;
using static Maja.Compiler.Parser.MajaParser;

namespace Maja.UnitTests.Parser;

public class ModuleParserTests
{
    private static Compilation_unitContext Parse(string code)
    {
        var parser = Maja.Compiler.Compiler.CreateParser(code);
        var parseTree = parser.compilation_unit();
        return parseTree;
    }

    [Fact]
    public void DeclPub1()
    {
        const string code =
            "pub qualified.name" + Tokens.EOL
            ;

        var result = Parse(code);
        result.decl_pub1().Should().NotBeNull();
    }
}