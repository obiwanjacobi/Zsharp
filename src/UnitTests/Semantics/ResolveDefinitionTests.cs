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

            var sym = file.CodeBlock.Symbols.FindSymbol(v.Identifier.SymbolName.ToCanonical(), AstSymbolKind.Variable);
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
        public void FunctionReturnTypeReferenceIntrinsic()
        {
            const string code =
                "fn: (): Str" + Tokens.NewLine +
                Tokens.Indent1 + "return \"Hello Z#\"" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            fn.FunctionType.TypeReference.TypeDefinition.Should().NotBeNull();
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
        public void FunctionReferenceByParameterType_Forward()
        {
            const string code =
                "fn(42)" + Tokens.NewLine +
                "fn: (p: I32)" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var fnRef = file.CodeBlock.ItemAt<AstFunctionReference>(0);
            fnRef.Should().NotBeNull();
            fnRef.Symbol.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void FunctionReferenceInferredTypeRef()
        {
            const string code =
                "fn: (): Str" + Tokens.NewLine +
                Tokens.Indent1 + "return \"Hello Z#\"" + Tokens.NewLine +
                "s = fn()" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var assign = file.CodeBlock.ItemAt<AstAssignment>(1);
            assign.Expression.RHS.FunctionReference.FunctionType.TypeReference.Should().NotBeNull();
            assign.Variable.TypeReference.TypeDefinition
                .Should().BeEquivalentTo(assign.Expression.TypeReference.TypeDefinition);
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
        public void FunctionParameterReferenceTwo()
        {
            const string code =
                "fn: (p: U8, s: Str)" + Tokens.NewLine +
                Tokens.Indent1 + "x = s" + Tokens.NewLine +
                Tokens.Indent1 + "y = p" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var a = fn.CodeBlock.ItemAt<AstAssignment>(0);
            var v = a.Variable as AstVariableDefinition;
            var p = a.Expression.RHS.VariableReference.ParameterDefinition;
            p.Should().NotBeNull();
            v.TypeReference.Identifier.Name.Should().Be(p.TypeReference.Identifier.Name);

            a = fn.CodeBlock.ItemAt<AstAssignment>(1);
            v = a.Variable as AstVariableDefinition;
            p = a.Expression.RHS.VariableReference.ParameterDefinition;
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

            call.Symbol.FindFunctionDefinition(call).Should().NotBeNull();
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
            var p = call.FunctionType.Parameters.First();
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
            id.Expression.TypeReference.TypeDefinition.Should().NotBeNull();

            var typeSymbol = v.Symbol.SymbolTable.Find(v.TypeReference.Identifier, AstSymbolKind.Type);
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
            var symbol = symbols.FindSymbol("Myenum.Zero", AstSymbolKind.Field);
            symbol.Definition.Should().NotBeNull();

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
            var symbol = symbols.FindSymbol("fn;U8", AstSymbolKind.Function);
            symbol.Definition.Should().NotBeNull();

            var a = file.CodeBlock.ItemAt<AstAssignment>(1);
            a.Expression.RHS.FunctionReference.FunctionDefinition.Should().NotBeNull();
        }
    }
}
