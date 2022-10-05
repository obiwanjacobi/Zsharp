using System.Linq;
using FluentAssertions;
using Maja.Compiler.Syntax;
using Xunit;

namespace Maja.UnitTests.Syntax
{
    public class ModuleSyntaxTests
    {
        private static CompilationUnitSyntax Parse(string code)
        {
            var parser = Maja.Compiler.Compiler.CreateParser(code);
            var parseTree = parser.compilation_unit();

            //var errs = parseTree.Errors().Select(e => e.Text);
            //if (errs.Any())
            //    throw new Exception(String.Join(Environment.NewLine, errs));

            var builder = new SyntaxNodeBuilder(nameof(ModuleSyntaxTests));
            var syntax = builder.VisitCompilation_unit(parseTree);
            return (CompilationUnitSyntax)syntax[0];
        }

        [Fact]
        public void DeclPub1_Single()
        {
            const string code =
                "pub qualified.name" + Tokens.EOL
                ;

            var result = Parse(code);
            result.Should().NotBeNull();
            var pubs = result.PublicExports.ToArray();
            pubs.Should().HaveCount(1);
            pubs[0].Names.Should().HaveCount(1);
        }

        [Fact]
        public void DeclPub1_Multiple()
        {
            const string code =
                "pub qualified.name1, qualified.name2" + Tokens.EOL
                ;

            var result = Parse(code);
            result.Should().NotBeNull();
            var pubs = result.PublicExports.ToArray();
            pubs.Should().HaveCount(1);
            pubs[0].Names.Should().HaveCount(2);
        }

        [Fact]
        public void UseImport()
        {
            const string code =
                "use qualified.name" + Tokens.EOL
                ;

            var result = Parse(code);
            result.Should().NotBeNull();
            var uses = result.UseImports.ToArray();
            uses.Should().HaveCount(1);
            uses[0].QualifiedName.Name.Should().Be("qualified.name");
        }
    }
}