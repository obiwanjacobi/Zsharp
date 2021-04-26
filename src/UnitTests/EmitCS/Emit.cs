using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Zsharp;
using Zsharp.AST;
using Zsharp.EmitCS;
using Zsharp.Semantics;

namespace UnitTests.EmitCS
{
    internal static class Emit
    {
        private const string AssemblyName = "EmitCsCodeTest";

        public static AssemblyManager LoadTestAssemblies()
        {
            var assemblies = new AssemblyManager();
            assemblies.LoadAssembly(@"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\3.1.0\ref\netcoreapp3.1\System.Console.dll");
            return assemblies;
        }

        public static ExternalModuleLoader CreateModuleLoader()
        {
            var assemblies = LoadTestAssemblies();
            var loader = new ExternalModuleLoader(assemblies);
            //loader.Modules.Should().HaveCount(2);
            return loader;
        }

        public static EmitCode Create(string code, IAstModuleLoader moduleLoader = null)
        {
            var compiler = new Compiler(moduleLoader ?? new ModuleLoader());
            var errors = compiler.Compile("UnitTests", AssemblyName, code);
            foreach (var err in errors)
            {
                Console.WriteLine(err);
            }
            errors.Should().BeEmpty();

            var module = compiler.Context.Modules.Modules.First();
            var emit = new EmitCode(AssemblyName);
            emit.Visit(module);
            return emit;
        }

        public static string Build(string projectPath)
        {
            var compiler = new CsCompiler()
            {
                Debug = true,
                ProjectPath = projectPath,
            };

            //compiler.Project.AddReference

            var output = String.Join(Environment.NewLine, compiler.Compile(AssemblyName));
            Console.WriteLine(output);

            if (OutputHasErrors(output))
                throw new ZsharpException($"Build Failed: {projectPath}");

            return compiler.Project.TargetPath;
        }

        private static bool OutputHasErrors(string output)
            => output.Contains("Build FAILED.");

        public static void InvokeStatic(string assemblyName, string typeName, string methodName)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var path = Path.GetDirectoryName(thisAssembly.Location);

            Console.WriteLine("---------------------------------------------------------");

            var assembly = Assembly.LoadFile(Path.Combine(path, assemblyName + ".dll"));
            var type = assembly.ExportedTypes.Single(t => t.Name == typeName);
            var method = type.GetMethods().Single(m => m.Name == methodName);
            method.Invoke(null, null);
        }
    }
}
