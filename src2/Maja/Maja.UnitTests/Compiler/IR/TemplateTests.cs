using System.Linq;
using Maja.Compiler.IR;

namespace Maja.UnitTests.Compiler.IR;

public class TemplateTests
{
    [Fact]
    public void FnTypeParams_Template()
    {
        const string code =
            "fn: <#T>(p: T)" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Declarations.Should().HaveCount(1);
        var fn = program.Root.Declarations[0].As<IrDeclarationFunction>();
        fn.TypeParameters.Should().HaveCount(1);
        var templType = fn.TypeParameters.First().As<IrTypeParameterTemplate>();
        templType.Symbol.Name.Value.Should().Be("T");
    }

    [Fact]
    public void FnTypeParamsWithDefault_Template()
    {
        const string code =
            "fn: <#T = Str>(p: T)" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Declarations.Should().HaveCount(1);
        var fn = program.Root.Declarations[0].As<IrDeclarationFunction>();
        fn.TypeParameters.Should().HaveCount(1);
        var templType = fn.TypeParameters.First().As<IrTypeParameterTemplate>();
        templType.Type!.Symbol.Name.Value.Should().Be("Str");
    }
}