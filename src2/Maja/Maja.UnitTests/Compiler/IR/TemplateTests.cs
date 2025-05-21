using System.Linq;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;

namespace Maja.UnitTests.Compiler.IR;

public class TemplateTests
{
    [Fact]
    public void Func_TypeParams_Template()
    {
        const string code =
            "fn: <#T>(p: T): T" + Tokens.Eol +
            Tokens.Indent1 + "ret p" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Declarations.Should().HaveCount(1);
        var fn = program.Root.Declarations[0].As<IrDeclarationFunction>();
        fn.Symbol.Name.Value.Should().Be("fn");
        fn.Type.Symbol.Name.Value.Should().Be("(T):T");
        fn.TypeParameters.Should().HaveCount(1);
        var templType = fn.TypeParameters.First().As<IrTypeParameterTemplate>();
        templType.Symbol.Name.Value.Should().Be("T");
    }

    [Fact]
    public void Func_TypeParamsWithDefault_Template()
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
    public void Func_TypeParamsInstantiate_Template()
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
        fn.Symbol.Type.Name.Value.Should().Be("(U8):U8");
        fn.Symbol.Parameters[0].Type.Name.Value.Should().Be("U8");
        fn.Symbol.ReturnType.Name.Value.Should().Be("U8");
    }

    [Fact]
    public void Type_TypeParams_Template()
    {
        const string code =
            "Templ<#T>" + Tokens.Eol +
            Tokens.Indent1 + "fld1: T" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Declarations.Should().HaveCount(1);
        var type = program.Root.Declarations[0].As<IrDeclarationType>();
        type.Symbol.TemplateName.FullName.Should().Be("Defmod.Templ#1");
        type.TypeParameters.Should().HaveCount(1);
        var templType = type.TypeParameters.First().As<IrTypeParameterTemplate>();
        templType.Symbol.Name.Value.Should().Be("T");
    }

    [Fact]
    public void Type_TypeParamsInstantiate_Template()
    {
        const string code =
            "Templ<#T>" + Tokens.Eol +
            Tokens.Indent1 + "fld1: T" + Tokens.Eol +
            "s := Templ<U8>" + Tokens.Eol +
            Tokens.Indent1 + "fld1 = 42" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Declarations.Should().HaveCount(2);
        var v = program.Root.Declarations[1].As<IrDeclarationVariable>();
        v.TypeSymbol.As<TypeTemplateSymbol>().TemplateName.FullName.Should().Be("Defmod.Templ#U8");

        var instance = v.Initializer.As<IrExpressionTypeInitializer>();
        instance.TypeSymbol.As<TypeTemplateSymbol>().TemplateName.FullName.Should().Be("Defmod.Templ#U8");
        instance.Fields[0].Field.Type.Name.Value.Should().Be("U8");
    }
}