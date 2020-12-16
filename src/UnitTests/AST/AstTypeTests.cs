using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace UnitTests.AST
{
    [TestClass]
    public class AstTypeTests
    {
        [TestMethod]
        public void FunctionType()
        {
            const string code =
                "fn: (): U8" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinition>(0);
            var t = fn.TypeReference;
            t.IsOptional.Should().BeFalse();
            t.IsError.Should().BeFalse();
            t.Identifier.Name.Should().Be("U8");
        }

        [TestMethod]
        public void FunctionOptionalErrorType()
        {
            const string code =
                "fn: (): U8!?" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinition>(0);
            var t = fn.TypeReference;
            t.IsOptional.Should().BeTrue();
            t.IsError.Should().BeTrue();
            t.Identifier.Name.Should().Be("U8");
        }

        [TestMethod]
        public void FunctionParameterType()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var t = fn.Parameters.First().TypeReference;
            t.IsOptional.Should().BeFalse();
            t.IsError.Should().BeFalse();
            t.Identifier.Name.Should().Be("U8");
        }

        [TestMethod]
        public void FunctionParameterCustomType()
        {
            const string code =
                "fn: (p: SomeType)" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var t = fn.Parameters.First().TypeReference;
            t.IsOptional.Should().BeFalse();
            t.IsError.Should().BeFalse();
            t.Identifier.Name.Should().Be("SomeType");
        }

        [TestMethod]
        public void FunctionParameterOptionalCustomType()
        {
            const string code =
                "fn: (p: SomeType?)" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var t = fn.Parameters.First().TypeReference;
            t.IsOptional.Should().BeTrue();
            t.IsError.Should().BeFalse();
            t.Identifier.Name.Should().Be("SomeType");
        }

        [TestMethod]
        public void TopVariableType()
        {
            const string code =
                "v: U8" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var a = file.CodeBlock.ItemAt<AstVariableDefinition>(0);
            var t = a.TypeReference;
            t.IsOptional.Should().BeFalse();
            t.IsError.Should().BeFalse();
            t.Identifier.Name.Should().Be("U8");
        }

        [TestMethod]
        public void EnumTypeDefinition()
        {
            const string code =
                "MyEnum" + Tokens.NewLine +
                Tokens.Indent1 + "None = 0" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var e = file.CodeBlock.ItemAt<AstTypeDefinitionEnum>(0);
            e.Symbol.Definition.Should().Be(e);
            e.BaseType.Identifier.Name.Should().Be("I32");

            var f = e.Fields.First();
            f.Identifier.Name.Should().Be("None");
            f.Expression.Should().NotBeNull();
            f.Symbol.Definition.Should().Be(f);
        }

        [TestMethod]
        public void EnumTypeDefinition_BaseType()
        {
            const string code =
                "MyEnum: U8" + Tokens.NewLine +
                Tokens.Indent1 + "None = 0" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var e = file.CodeBlock.ItemAt<AstTypeDefinitionEnum>(0);
            e.Symbol.Definition.Should().Be(e);
            e.BaseType.Identifier.Name.Should().Be("U8");

            var f = e.Fields.First();
            f.Identifier.Name.Should().Be("None");
            f.Expression.Should().NotBeNull();
            f.Symbol.Definition.Should().Be(f);
        }

        [TestMethod]
        public void EnumTypeDefinition_AutoNumber()
        {
            const string code =
                "Count" + Tokens.NewLine +
                Tokens.Indent1 + "Zero" + Tokens.NewLine +
                Tokens.Indent1 + "One" + Tokens.NewLine +
                Tokens.Indent1 + "Two" + Tokens.NewLine +
                Tokens.Indent1 + "Three" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var e = file.CodeBlock.ItemAt<AstTypeDefinitionEnum>(0);
            e.Symbol.Definition.Should().Be(e);

            var f = e.Fields.First();
            f.Identifier.Name.Should().Be("Zero");
            f.Expression.RHS.LiteralNumeric.Value.Should().Be(0);

            f = e.Fields.Skip(1).First();
            f.Identifier.Name.Should().Be("One");
            f.Expression.RHS.LiteralNumeric.Value.Should().Be(1);

            f = e.Fields.Skip(2).First();
            f.Identifier.Name.Should().Be("Two");
            f.Expression.RHS.LiteralNumeric.Value.Should().Be(2);

            f = e.Fields.Skip(3).First();
            f.Identifier.Name.Should().Be("Three");
            f.Expression.RHS.LiteralNumeric.Value.Should().Be(3);
        }

        [TestMethod]
        public void StructTypeDefinition()
        {
            const string code =
                "MyStruct" + Tokens.NewLine +
                Tokens.Indent1 + "Id: U32" + Tokens.NewLine +
                Tokens.Indent1 + "Name: Str" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var s = file.CodeBlock.ItemAt<AstTypeDefinitionStruct>(0);
            s.Symbol.Definition.Should().Be(s);
            s.BaseType.Should().BeNull();

            var f = s.Fields.First();
            f.Identifier.Name.Should().Be("Id");
            f.TypeReference.Should().NotBeNull();
            f.Symbol.Definition.Should().Be(f);
        }

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
