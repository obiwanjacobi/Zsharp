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
    }
}