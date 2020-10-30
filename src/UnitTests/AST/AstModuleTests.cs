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

        [TestMethod]
        public void LoadExternalAssembly()
        {
            var assemblies = LoadTestAssemblies();

            assemblies.Assemblies.Should().HaveCount(1);
        }

        [TestMethod]
        public void LoadExternalModules()
        {
            var assemblies = LoadTestAssemblies();

            var modules = new ExternalModuleLoader(assemblies);

            modules.Modules.Should().HaveCount(1);
            var system = modules.LoadExternal("System");
            system.Should().NotBeNull();
        }
    }
}
