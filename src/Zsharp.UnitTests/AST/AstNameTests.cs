using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.UnitTests.AST
{
    [TestClass]
    public class AstNameTests
    {
        private static void Assert(AstName uot,
            string ns, string symbol, AstNameKind kind,
            int partCount, string postfix = "", string prefix = "")
        {
            uot.Should().NotBeNull();
            uot.Kind.Should().Be(kind);
            uot.Namespace.Should().Be(ns);
            uot.Symbol.Should().Be(symbol);
            uot.Parts.Count().Should().Be(partCount);
            uot.Postfix.Should().Be(postfix);
            uot.Prefix.Should().Be(prefix);
        }

        [TestMethod]
        public void FromExternal_DotName()
        {
            var uot = AstName.FromExternal("Root.Namespace", "Symbol.Field");
            Assert(uot, "Root.Namespace", "Symbol.Field", AstNameKind.External, 3);
        }

        [TestMethod]
        public void FromExternal_Prefix()
        {
            var uot = AstName.FromExternal("Root.Namespace", "get_Property");
            Assert(uot, "Root.Namespace", "Property", AstNameKind.External, 3, "", "get_");
        }

        [TestMethod]
        public void FromLocal_DotName()
        {
            var uot = AstName.FromLocal("Symbol.Field");
            Assert(uot, "", "Symbol.Field", AstNameKind.Local, 1);
        }

        [TestMethod]
        public void FromCanonical_DotName()
        {
            var uot = AstName.FromLocal("Symbol.Field");
            Assert(uot, "", "Symbol.Field", AstNameKind.Local, 1);
        }

        [TestMethod]
        public void Parse_FullName()
        {
            var uot = AstName.ParseFullName("Root.Namespace.Symbol");
            Assert(uot, "Root.Namespace", "Symbol", AstNameKind.Local, 3);
        }

        [TestMethod]
        public void Parse_Symbol()
        {
            var uot = AstName.ParseFullName("Symbol");
            Assert(uot, "", "Symbol", AstNameKind.Local, 1);
        }

        [TestMethod]
        public void GetArgumentCount_Defintion()
        {
            var uot = AstName.FromExternal("Root.Namespace", "Generic`3");
            Assert(uot, "Root.Namespace", "Generic", AstNameKind.External, 3, "%3");
            uot.GetArgumentCount().Should().Be(3);
        }

        [TestMethod]
        public void GetArgumentCount_Reference()
        {
            var uot = AstName.FromExternal("Root.Namespace", "Generic;U8;U16");
            Assert(uot, "Root.Namespace", "Generic", AstNameKind.External, 3, ";U8;U16");
            uot.GetArgumentCount().Should().Be(2);
        }
    }
}
