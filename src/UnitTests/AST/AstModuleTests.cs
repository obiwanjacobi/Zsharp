using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.Semantics;

namespace UnitTests.AST
{
    [TestClass]
    public class AstModuleTests
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
            //loader.Modules.Should().HaveCount(2);
            return loader;
        }

        [TestMethod]
        public void LoadExternalAssembly()
        {
            var assemblies = LoadTestAssemblies();

            assemblies.Assemblies.Should().HaveCount(1);
        }

        //[TestMethod]
        //public void LoadExternal_System()
        //{
        //    var loader = CreateModuleLoader();

        //    var system = loader.LoadExternal("System");
        //    system.Should().NotBeNull();
        //    system.Symbols.Namespace.Should().Be("System");
        //    system.Symbols.Entries.All(e => e.SymbolLocality == AstSymbolLocality.Imported).Should().BeTrue();
        //}
    }
}
