﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Zsharp;
using Zsharp.AST;
using Zsharp.Semantics;

namespace UnitTests.Semantics
{
    [TestClass]
    public class ResolveTypesTests
    {
        private static AssemblyManager LoadTestAssemblies()
        {
            var assemblies = new AssemblyManager();
            assemblies.LoadAssembly(@"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\3.1.0\ref\netcoreapp3.1\System.Console.dll");
            return assemblies;
        }

        private static ExternalModuleLoader CreateModuleLoader()
        {
            var assemblies = LoadTestAssemblies();
            var loader = new ExternalModuleLoader(assemblies);
            return loader;
        }

        private static AstFile CompileFile(string code, IAstModuleLoader moduleLoader = null)
        {
            var compiler = new Compiler(moduleLoader ?? new ModuleLoader());
            var errors = compiler.Compile("UnitTests", "ResolveTypeTests", code);
            foreach (var err in errors)
            {
                Console.WriteLine(err);
            }

            errors.Should().BeEmpty();

            return ((AstModulePublic)compiler.Context.Modules.Modules.First()).Files.First();
        }

        [TestMethod]
        public void TopVariableTypeDef()
        {
            const string code =
                "v: U8" + Tokens.NewLine
                ;

            var file = CompileFile(code);

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

            var file = CompileFile(code);

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

            var file = CompileFile(code);

            var a = file.CodeBlock.ItemAt<AstAssignment>(0);
            var v = a.Variable as AstVariableDefinition;
            v.Should().NotBeNull();
            v.Parent.Should().Be(a);
            v.TypeReference.Should().NotBeNull();

            var sym = file.CodeBlock.Symbols.FindEntry(v.Identifier.Name, AstSymbolKind.Variable);
            sym.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TopVariableReference()
        {
            const string code =
                "v = 42" + Tokens.NewLine +
                "x = v" + Tokens.NewLine
                ;

            var file = CompileFile(code);

            var a = file.CodeBlock.ItemAt<AstAssignment>(0);
            var v = a.Variable as AstVariableDefinition;
            v.Should().NotBeNull();
            v.Parent.Should().Be(a);
            v.TypeReference.Should().NotBeNull();

            var sym = file.CodeBlock.Symbols.Find(v);
            sym.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void FunctionParameterReference()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "x = p" + Tokens.NewLine
                ;

            var file = CompileFile(code);

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

            var moduleLoader = CreateModuleLoader();
            var file = CompileFile(code, moduleLoader);

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

            var file = CompileFile(code);

            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var call = fn.CodeBlock.ItemAt<AstFunctionReference>(0);
            var p = call.Parameters.First();
            p.TypeReference.Should().NotBeNull();
        }

        [TestMethod]
        public void TemplateInstantiation()
        {
            const string code =
                "Struct<T>" + Tokens.NewLine +
                Tokens.Indent1 + "Id: T" + Tokens.NewLine +
                "s = Struct<U8>" + Tokens.NewLine +
                Tokens.Indent1 + "Id = 42" + Tokens.NewLine
                ;

            var file = CompileFile(code);

            //var template = file.CodeBlock.ItemAt<AstTypeDefinitionStruct>(0);
            var a = file.CodeBlock.ItemAt<AstAssignment>(1);
            var v = a.Variable;
            v.Symbol.Definition.Should().NotBeNull();
            //var id = v.TypeReference.Fields.First();
        }
    }
}
