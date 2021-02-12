using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace UnitTests.Semantics
{
    [TestClass]
    public class ResolveDefinitionTests
    {
        [TestMethod]
        public void TopVariableInferDef()
        {
            const string code =
                "v = 42" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var a = file.CodeBlock.ItemAt<AstAssignment>(0);
            var v = a.Variable as AstVariableDefinition;
            v.Should().NotBeNull();
            v.Parent.Should().Be(a);

            var sym = file.CodeBlock.Symbols.Find(v);
            sym.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TopVariableTypeDef()
        {
            const string code =
                "v: U8" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var v = file.CodeBlock.ItemAt<AstVariableDefinition>(0);
            v.TypeReference.Should().NotBeNull();
            v.TypeReference.TypeDefinition.Should().NotBeNull();
            v.TypeReference.TypeDefinition.IsIntrinsic.Should().BeTrue();

            var sym = file.CodeBlock.Symbols.Find(v);
            sym.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TopVariableTypeDefInit()
        {
            const string code =
                "v: U8 = 42" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var a = file.CodeBlock.ItemAt<AstAssignment>(0);
            var v = a.Variable as AstVariableDefinition;
            v.Should().NotBeNull();
            v.TypeReference.Should().NotBeNull();
            v.TypeReference.TypeDefinition.Should().NotBeNull();
            v.TypeReference.TypeDefinition.IsIntrinsic.Should().BeTrue();

            var sym = file.CodeBlock.Symbols.Find(v);
            sym.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TopVariableInferType()
        {
            const string code =
                "v = 42" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var a = file.CodeBlock.ItemAt<AstAssignment>(0);
            var v = a.Variable as AstVariableDefinition;
            v.Should().NotBeNull();
            v.Parent.Should().Be(a);
            v.TypeReference.Should().NotBeNull();

            var sym = file.CodeBlock.Symbols.FindEntry(v.Identifier.Name, AstSymbolKind.Variable);
            sym.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TopVariableInferReference()
        {
            const string code =
                "v = 42" + Tokens.NewLine +
                "x = v" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var a = file.CodeBlock.ItemAt<AstAssignment>(0);
            var v = a.Variable as AstVariableDefinition;
            v.Should().NotBeNull();
            v.Parent.Should().Be(a);
            v.TypeReference.Should().NotBeNull();

            var sym = file.CodeBlock.Symbols.Find(v);
            sym.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TopVariableReference()
        {
            const string code =
                "x: U8" + Tokens.NewLine +
                "v = x + 1" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var x = file.CodeBlock.ItemAt<AstVariableDefinition>(0);
            var a = file.CodeBlock.ItemAt<AstAssignment>(1);
            var vd = a.Variable as AstVariableDefinition;
            vd.Should().NotBeNull();
            var vr = a.Expression.LHS.VariableReference;
            vr.Should().NotBeNull();

            vr.VariableDefinition.Should().Be(x);
            vr.Symbol.Should().Be(x.Symbol);
        }

        [TestMethod]
        public void FunctionReference()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "fn()" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            fn.Should().NotBeNull();

            var fnRef = fn.CodeBlock.ItemAt<AstFunctionReference>(0);
            fnRef.Should().NotBeNull();
            fnRef.Symbol.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void FunctionForwardReference()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "fn2()" + Tokens.NewLine +
                "fn2: ()" + Tokens.NewLine +
                Tokens.Indent1 + "fn()" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            fn.Should().NotBeNull();

            var fnRef = fn.CodeBlock.ItemAt<AstFunctionReference>(0);
            fnRef.Should().NotBeNull();
            fnRef.Identifier.Name.Should().Be("fn2");
            fnRef.Symbol.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void FunctionParameterReference()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "x = p" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var a = fn.CodeBlock.ItemAt<AstAssignment>(0);
            var v = a.Variable as AstVariableDefinition;
            var p = a.Expression.RHS.VariableReference.ParameterDefinition;
            p.Should().NotBeNull();
            v.TypeReference.Identifier.Name.Should().Be(p.TypeReference.Identifier.Name);
        }

        [TestMethod]
        public void ImportFunctionNameAlias()
        {
            const string code =
                "import Print = System.Console.WriteLine" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "Print(\"Hello World\")" + Tokens.NewLine
                ;

            var moduleLoader = Compile.CreateModuleLoader();
            var file = Compile.File(code, moduleLoader);

            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var call = fn.CodeBlock.ItemAt<AstFunctionReference>(0);

            call.Symbol.FindOverloadDefinition(call).Should().NotBeNull();
        }

        [TestMethod]
        public void FunctionCallParameterReference()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "fn(42)" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var call = fn.CodeBlock.ItemAt<AstFunctionReference>(0);
            var p = call.Parameters.First();
            p.TypeReference.Should().NotBeNull();
        }

        [TestMethod]
        public void StructTemplateInstantiation()
        {
            const string code =
                "Struct<T>" + Tokens.NewLine +
                Tokens.Indent1 + "Id: T" + Tokens.NewLine +
                "s = Struct<U8>" + Tokens.NewLine +
                Tokens.Indent1 + "Id = 42" + Tokens.NewLine
                ;

            var file = Compile.File(code);
            var template = file.CodeBlock.ItemAt<AstTypeDefinitionStruct>(0);
            var a = file.CodeBlock.ItemAt<AstAssignment>(1);
            var v = a.Variable;
            v.Symbol.Definition.Should().NotBeNull();
            var id = a.Fields.First();
            // TODO:
            //id.Expression.TypeReference.TypeDefinition.Should().NotBeNull();

            var typeSymbol = v.Symbol.SymbolTable.FindEntry(v.TypeReference.Identifier, AstSymbolKind.Type);
            var typeDef = typeSymbol.DefinitionAs<AstTemplateInstanceStruct>();
            typeDef.TemplateDefinition.Should().Be(template);
        }

        [TestMethod]
        public void EnumUseOption()
        {
            const string code =
                "MyEnum" + Tokens.NewLine +
                Tokens.Indent1 + "Zero" + Tokens.NewLine +
                "v = MyEnum.Zero" + Tokens.NewLine
                ;

            var file = Compile.File(code);
            var symbols = file.Symbols;
            var entry = symbols.FindEntry("Myenum.Zero", AstSymbolKind.Field);
            entry.Definition.Should().NotBeNull();

            var a = file.CodeBlock.ItemAt<AstAssignment>(1);
            var v = a.Variable;
            v.Symbol.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TemplateFunction()
        {
            const string code =
                "fn: <T>(p: T): Str" + Tokens.NewLine +
                Tokens.Indent1 + "return \"\"" + Tokens.NewLine +
                "s = fn<U8>(42)" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var symbols = file.Symbols;
            var entry = symbols.FindEntry("fn", AstSymbolKind.Function);
            entry.Definition.Should().NotBeNull();

            var a = file.CodeBlock.ItemAt<AstAssignment>(1);
            a.Expression.RHS.FunctionReference.FunctionDefinition.Should().NotBeNull();
        }
    }
}
