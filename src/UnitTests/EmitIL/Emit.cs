using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Zsharp;
using Zsharp.AST;
using Zsharp.EmitIL;
using Zsharp.External;

namespace UnitTests.EmitIL
{
    internal static class Emit
    {
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
            var errors = compiler.Compile("UnitTests", "EmitCodeTests", code);
            foreach (var err in errors)
            {
                Console.WriteLine(err);
            }
            errors.Should().BeEmpty();

            var module = compiler.Context.Modules.Modules.First();
            var emit = new EmitCode("EmitCodeTest");
            emit.Visit(module);
            return emit;
        }

        public static void InvokeStatic(string assemblyName, string typeName = null, string methodName = null)
        {
            if (String.IsNullOrEmpty(typeName))
                typeName = assemblyName;
            if (String.IsNullOrEmpty(methodName))
                methodName = "Main";

            var thisAssembly = Assembly.GetExecutingAssembly();
            var path = Path.GetDirectoryName(thisAssembly.Location);

            var assembly = Assembly.LoadFile(Path.Combine(path, assemblyName + ".dll"));
            var canonicalName = AstDotName.ToCanonical(typeName);
            var type = assembly.ExportedTypes.Single(t => t.Name == canonicalName);
            var method = type.GetMethods().Single(m => m.Name == methodName);
            method.Invoke(null, null);
        }
    }
}
