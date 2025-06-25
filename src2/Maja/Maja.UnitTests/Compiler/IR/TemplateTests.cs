using System.Linq;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;

namespace Maja.UnitTests.Compiler.IR;

public class TemplateTests
{
    [Fact]
    public void Func_Template()
    {
        const string code =
            "fn: <#T>(p: T): T" + Tokens.Eol +
            Tokens.Indent1 + "ret p" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Declarations.Should().HaveCount(1);
        var fn = program.Module.Declarations.ElementAt(0).As<IrDeclarationFunction>();
        fn.Symbol.Name.Value.Should().Be("fn");
        fn.Type.Symbol.Name.Value.Should().Be("(T):T");
        fn.TypeParameters.Should().HaveCount(1);
        var templType = fn.TypeParameters.First().As<IrTypeParameterTemplate>();
        templType.Symbol.Name.Value.Should().Be("T");
    }

    [Fact]
    public void Func_Template_Defaults()
    {
        const string code =
            "fn: <#T = Str>(p: T): T" + Tokens.Eol +
            Tokens.Indent1 + "ret p" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Declarations.Should().HaveCount(1);
        var fn = program.Module.Declarations.ElementAt(0).As<IrDeclarationFunction>();
        fn.TypeParameters.Should().HaveCount(1);
        var templType = fn.TypeParameters.First().As<IrTypeParameterTemplate>();
        templType.Type!.Symbol.Name.Value.Should().Be("Str");
    }

    [Fact]
    public void Func_TemplateInstantiate()
    {
        const string code =
            "fn: <#T>(p: T): T" + Tokens.Eol +
            Tokens.Indent1 + "ret p" + Tokens.Eol +
            "v := fn<U8>(42)" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Declarations.Should().HaveCount(3);
        var v = program.Module.Declarations.ElementAt(1).As<IrDeclarationVariable>();
        v.TypeSymbol.Should().Be(TypeSymbol.U8);
        var fn = v.Initializer.As<IrExpressionInvocation>();
        fn.Symbol.Type.Name.Value.Should().Be("(U8):U8");
        fn.Symbol.Parameters[0].Type.Name.Value.Should().Be("U8");
        fn.Symbol.ReturnType.Name.Value.Should().Be("U8");

        // TODO: test the template instantiation
    }

    [Fact]
    public void Type_Template()
    {
        const string code =
            "Templ<#T>" + Tokens.Eol +
            Tokens.Indent1 + "fld1: T" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Declarations.Should().HaveCount(1);
        var type = program.Module.Declarations.ElementAt(0).As<IrDeclarationType>();
        type.TypeParameters.Should().HaveCount(1);
        var templType = type.TypeParameters.First().As<IrTypeParameterTemplate>();
        templType.Symbol.Name.Value.Should().Be("T");
    }

    [Fact]
    public void Type_TemplateInstantiate()
    {
        const string code =
            "Templ<#T>" + Tokens.Eol +
            Tokens.Indent1 + "fld1: T" + Tokens.Eol +
            "s := Templ<U8>" + Tokens.Eol +
            Tokens.Indent1 + "fld1 = 42" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Declarations.Should().HaveCount(3);
        var v = program.Module.Declarations.ElementAt(1).As<IrDeclarationVariable>();
        v.TypeSymbol.As<TypeTemplateSymbol>().Name.FullName.Should().Be("Defmod.Templ#U8");

        var instance = v.Initializer.As<IrExpressionTypeInitializer>();
        instance.TypeSymbol.As<TypeTemplateSymbol>().Name.FullName.Should().Be("Defmod.Templ#U8");
        instance.Fields[0].Field.Type.Name.Value.Should().Be("U8");
    }

    [Fact]
    public void Type_TemplateInstantiate_UseField()
    {
        const string code =
            "Templ<#T>" + Tokens.Eol +
            Tokens.Indent1 + "fld1: T" + Tokens.Eol +
            "s := Templ<U8>" + Tokens.Eol +
            Tokens.Indent1 + "fld1 = 42" + Tokens.Eol +
            "f: U8 = s.fld1" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Declarations.Should().HaveCount(4);
        var f = program.Module.Declarations.ElementAt(2).As<IrDeclarationVariable>();
        f.TypeSymbol.Should().Be(TypeSymbol.U8);

        var instance = f.Initializer.As<IrExpressionMemberAccess>();
        instance.Members.Should().HaveCount(1);
        instance.Members[0].Name.Value.Should().Be("fld1");
        instance.Members[0].Type.Should().Be(TypeSymbol.U8);
        instance.Expression.TypeSymbol.Name.Value.Should().Be("Templ#U8");
    }

    [Fact]
    public void Type_TemplateInstantiate_UseFieldInFunction()
    {
        const string code =
            "Templ<#T>" + Tokens.Eol +
            Tokens.Indent1 + "fld1: T" + Tokens.Eol +
            "fn: (p: U8): Bool" + Tokens.Eol +
            Tokens.Indent1 + "ret p = 42" + Tokens.Eol +
            "s := Templ<U8>" + Tokens.Eol +
            Tokens.Indent1 + "fld1 = 42" + Tokens.Eol +
            "b := fn(s.fld1)" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Declarations.Should().HaveCount(5);
        var b = program.Module.Declarations.ElementAt(3).As<IrDeclarationVariable>();
        b.TypeSymbol.Should().Be(TypeSymbol.Bool);

        var invoke = b.Initializer.As<IrExpressionInvocation>();
        invoke.Arguments[0].Expression.TypeSymbol.Name.Value.Should().Be("U8");
    }
}