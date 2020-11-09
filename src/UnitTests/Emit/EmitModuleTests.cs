﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.Emit;

namespace UnitTests.Emit
{
    [TestClass]
    public class EmitModuleTests
    {
        [TestMethod]
        public void AssemblyModule()
        {
            const string code =
                "module test" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            emit.Context.Assembly.Should().NotBeNull();
            emit.Context.Module.Should().NotBeNull();
        }

        [TestMethod]
        public void ModuleClass()
        {
            const string code =
                "module test" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            emit.Context.Module.Types.Find("test").Should().NotBeNull();
        }
    }
}