﻿using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Zsharp;
using Zsharp.AST;
using Zsharp.EmitCS;

namespace Zsharp.UnitTests.EmitCS
{
    internal static class Emit
    {
        private const string ZsharpRuntime = "Zsharp.Runtime.dll";
        private const string AssemblyName = "EmitCsCodeTest";

        public static IAstModuleLoader CreateModuleLoader()
            => Compile.CreateModuleLoader();

        public static EmitCode Create(string code, IAstModuleLoader moduleLoader = null)
        {
            var compiler = new Compiler(moduleLoader ?? new ModuleLoader());
            var errors = compiler.ParseAst("UnitTests", AssemblyName, code);
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
            var csCompiler = new CsCompiler()
            {
                Debug = true,
                ProjectPath = projectPath,
            };

            var runtimePath = Path.Combine(
                // [test] net5.0/debug/bin/UnitTest/src
                "..", "..", "..", "..", "..",
                "Zsharp.Runtime",
                "bin",
                csCompiler.Debug ? "Debug" : "Release",
                csCompiler.Project.TargetFrameworkMoniker,
                ZsharpRuntime);

            csCompiler.Project.AddReference(runtimePath);

            var output = csCompiler.Compile(AssemblyName);
            Console.WriteLine(output);

            if (OutputHasErrors(output))
                throw new ZsharpException($"Build Failed: {projectPath}");

            return csCompiler.Project.TargetPath;
        }

        public static EmitCode Run(string code, string testName, IAstModuleLoader moduleLoader = null)
        {
            var emit = Emit.Create(code, moduleLoader);

            emit.SaveAs($@".\{testName}\{testName}.cs");

            Console.WriteLine(emit.ToString());

            var targetPath = Emit.Build(testName);

            File.Exists(targetPath).Should().BeTrue();
            return emit;
        }

        private static bool OutputHasErrors(string output)
            => output.Contains("Build FAILED.");

        public static void InvokeStatic(string assemblyName, string typeName = null, string methodName = null)
        {
            if (String.IsNullOrEmpty(typeName))
                typeName = assemblyName;
            if (String.IsNullOrEmpty(methodName))
                methodName = "Main";

            var thisAssembly = Assembly.GetExecutingAssembly();
            var path = Path.GetDirectoryName(thisAssembly.Location);

            Console.WriteLine("---------------------------------------------------------");

            var assembly = Assembly.LoadFile(Path.Combine(path, assemblyName + ".dll"));
            var canonicalName = AstSymbolName.ToCanonical(typeName);
            var type = assembly.ExportedTypes.Single(t => t.Name == canonicalName);
            var method = type.GetMethods().Single(m => m.Name == methodName);
            method.Invoke(null, null);
        }
    }
}
