using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.UnitTests.Semantics
{
    [TestClass]
    public class ResolveTemplateTests
    {
        [TestMethod]
        public void StructTemplateInstantiation()
        {
            const string code =
                "Struct<#T>" + Tokens.NewLine +
                Tokens.Indent1 + "Id: T" + Tokens.NewLine +
                "s = Struct<U8>" + Tokens.NewLine +
                Tokens.Indent1 + "Id = 42" + Tokens.NewLine
                ;

            var file = Compile.File(code);
            var template = file.CodeBlock.LineAt<AstTypeDefinitionStruct>(0);
            var a = file.CodeBlock.LineAt<AstAssignment>(1);
            var v = a.Variable;
            v.Symbol.Definition.Should().NotBeNull();
            var id = a.Fields.First();
            id.Expression.TypeReference.TypeDefinition.Should().NotBeNull();

            var typeSymbol = v.Symbol.SymbolTable.FindSymbol(v.TypeReference.Identifier, AstSymbolKind.Type);
            var typeDef = typeSymbol.DefinitionAs<AstTemplateInstanceStruct>();
            typeDef.TemplateDefinition.Should().Be(template);
        }

        [TestMethod]
        public void TemplateIntrinsicTypeUsage()
        {
            const string code =
                "s: Array<U8>" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var v = file.CodeBlock.LineAt<AstVariableDefinition>(0);
            var typeRef = v.TypeReference as AstTypeReferenceType;
            typeRef.IsTemplate.Should().BeTrue();
            var typeDef = typeRef.TypeDefinition as AstTemplateInstanceType;
            typeDef.Should().NotBeNull();
            typeDef.TemplateParameterArguments.Count.Should().Be(1);
        }

        [TestMethod]
        public void TemplateFunction()
        {
            const string code =
                "fn: <#T>(p: T): Str" + Tokens.NewLine +
                Tokens.Indent1 + "return \"\"" + Tokens.NewLine +
                "s = fn<U8>(42)" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var symbols = file.SymbolTable;
            var symbol = symbols.FindSymbol("fn;U8", AstSymbolKind.Function);
            symbol.Definition.Should().NotBeNull();

            var a = file.CodeBlock.LineAt<AstAssignment>(1);
            a.Expression.RHS.FunctionReference.FunctionDefinition.Should().NotBeNull();
        }

        [TestMethod]
        public void TemplateFunctionDefinition_ReturnType()
        {
            const string code =
                "fn: <#T>(p: U8): T" + Tokens.NewLine +
                Tokens.Indent1 + "return T(p)" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var fn = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            fn.Should().NotBeNull();
            fn.IsTemplate.Should().BeTrue();
            fn.TemplateParameters.Should().HaveCount(1);

            var symbols = file.SymbolTable;
            var symbol = symbols.FindSymbol("fn%1", AstSymbolKind.Function);
            symbol.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TemplateFunctionReference_ReturnType()
        {
            const string code =
                "fn: <#T>(p: U8): T" + Tokens.NewLine +
                Tokens.Indent1 + "return p.T()" + Tokens.NewLine +
                "s = fn<Str>(42)" + Tokens.NewLine
                ;

            var moduleLoader = new AssemblyManagerBuilder()
                .AddZsharpRuntime()
                .ToModuleLoader();

            var file = Compile.File(code, moduleLoader);

            var assign = file.CodeBlock.LineAt<AstAssignment>(1);
            var fn = assign.Expression.RHS.FunctionReference;
            fn.Should().NotBeNull();

            var symbols = file.SymbolTable;
            var symbol = symbols.FindSymbol("fn;Str", AstSymbolKind.Function);
            symbol.Definition.Should().NotBeNull();
        }
    }
}
