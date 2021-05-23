﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace UnitTests.AST
{
    [TestClass]
    public class AstTemplateTests
    {
        [TestMethod]
        public void TemplateTypeDefinition()
        {
            const string code =
                "MyStruct<T>" + Tokens.NewLine +
                Tokens.Indent1 + "Id: T" + Tokens.NewLine +
                Tokens.Indent1 + "Name: Str" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var s = file.CodeBlock.ItemAt<AstTypeDefinitionStruct>(0);
            s.Symbol.Definition.Should().Be(s);
            s.BaseType.Should().BeNull();
            s.IsTemplate.Should().BeTrue();
            var tp = (AstTemplateParameterDefinition)s.TemplateParameters.First();
            tp.Identifier.Name.Should().Be("T");

            var f = s.Fields.First();
            f.Identifier.Name.Should().Be("Id");
            f.TypeReference.Should().NotBeNull();
            f.TypeReference.IsTemplateParameter.Should().BeTrue();
            f.Symbol.Definition.Should().Be(f);
        }

        [TestMethod]
        public void TemplateTypeUsage()
        {
            const string code =
                "MyStruct<T>" + Tokens.NewLine +
                Tokens.Indent1 + "Id: T" + Tokens.NewLine +
                Tokens.Indent1 + "Name: Str" + Tokens.NewLine +
                "s = MyStruct<U8>" + Tokens.NewLine +
                Tokens.Indent1 + "Id = 42" + Tokens.NewLine +
                Tokens.Indent1 + "Name = \"Test42\"" + Tokens.NewLine
                ;

            var file = Build.File(code);

            var a = file.CodeBlock.ItemAt<AstAssignment>(1);
            a.Fields.Should().HaveCount(2);
            var id = a.Fields.First();
            id.Symbol.Should().NotBeNull();
            id.Expression.Should().NotBeNull();
            var name = a.Fields.Skip(1).First();
            name.Symbol.Should().NotBeNull();
            name.Expression.Should().NotBeNull();
        }

        [TestMethod]
        public void TemplateIntrinsicTypeUsage()
        {
            const string code =
                "s: Array<U8>" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var v = file.CodeBlock.ItemAt<AstVariableDefinition>(0);
            v.TypeReference.TypeDefinition.Should().NotBeNull();
        }

        [TestMethod]
        public void TemplateFunctionDefinition()
        {
            const string code =
                "fn: <T>(c: U16): T" + Tokens.NewLine +
                Tokens.Indent1 + "return c.T()" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            fn.IsTemplate.Should().BeTrue();
            var tp = fn.TemplateParameters.First();
            tp.Identifier.Name.Should().Be("T");
        }

        [TestMethod]
        public void TemplateFunctionReference()
        {
            const string code =
                "fn<U8>(42)" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunctionReference>(0);
            fn.IsTemplate.Should().BeTrue();
            var tp = fn.TemplateParameters.First();
            tp.TypeReference.Identifier.Name.Should().Be("U8");
        }
    }
}
