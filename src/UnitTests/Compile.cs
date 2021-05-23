using FluentAssertions;
using System.Linq;
using Zsharp;
using Zsharp.AST;
using Zsharp.External;

namespace UnitTests
{
    internal static class Compile
    {
        public static IAstModuleLoader CreateModuleLoader()
        {
            return new AssemblyManagerBuilder()
                .AddZsharpRuntime()
                .AddSystemConsole()
                .ToModuleLoader();
        }

        public static AstFile File(string code, IAstModuleLoader moduleLoader = null)
        {
            var compiler = new Compiler(moduleLoader ?? new ModuleLoader());
            var errors = compiler.Compile("UnitTests", "UnitTests", code);

            errors.PrintErrors();
            errors.Should().BeEmpty();

            return ((AstModulePublic)compiler.Context.Modules.Modules.First()).Files.First();
        }
    }

    internal class AssemblyManagerBuilder
    {
        private readonly AssemblyManager _assemblyManager = new();
        public AssemblyManager AssemblyManager => _assemblyManager;

        public IAstModuleLoader ToModuleLoader()
            => new ExternalModuleLoader(_assemblyManager);

        public AssemblyManagerBuilder AddZsharpRuntime()
        {
            _assemblyManager.LoadAssembly("Zsharp.Runtime.dll");
            return this;
        }

        public AssemblyManagerBuilder AddSystemConsole()
        {
            _assemblyManager.LoadAssembly(@"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\3.1.0\ref\netcoreapp3.1\System.Console.dll");
            return this;
        }
    }
}
