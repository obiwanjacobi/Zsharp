using System.Linq;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;

namespace Maja.UnitTests.Compiler.IR;

public class TemplateTests
{
    [Fact]
    public void FnTypeParams_Template()
    {
        const string code =
            "fn: <#T>(p: T): T" + Tokens.Eol +
            Tokens.Indent1 + "ret p" + Tokens.Eol
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
            "fn: <#T = Str>(p: T): T" + Tokens.Eol +
            Tokens.Indent1 + "ret p" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Declarations.Should().HaveCount(1);
        var fn = program.Root.Declarations[0].As<IrDeclarationFunction>();
        fn.TypeParameters.Should().HaveCount(1);
        var templType = fn.TypeParameters.First().As<IrTypeParameterTemplate>();
        templType.Type!.Symbol.Name.Value.Should().Be("Str");
    }

    [Fact]
    public void FnTypeParamsInstantiate_Template()
    {
        const string code =
            "fn: <#T>(p: T): T" + Tokens.Eol +
            Tokens.Indent1 + "ret p" + Tokens.Eol +
            "v := fn<U8>(42)" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Declarations.Should().HaveCount(2);
        var v = program.Root.Declarations[1].As<IrDeclarationVariable>();
        v.TypeSymbol.Should().Be(TypeSymbol.U8);
        var fn = v.Initializer.As<IrExpressionInvocation>();
    }
}