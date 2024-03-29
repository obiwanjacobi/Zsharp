﻿using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Zsharp.AST;
using Zsharp.External;

namespace Zsharp.UnitTests
{
    internal static class Compile
    {
        public static IAstModuleLoader CreateModuleLoader()
        {
            return new AssemblyManagerBuilder()
                .AddZsharpRuntime()
                .AddMsCoreLib()
                .AddSystemConsole()
                .AddSystemRuntime()
                .AddSystemCollections()
                .ToModuleLoader();
        }

        public static AstFile File(string code, IAstModuleLoader moduleLoader = null)
        {
            var compiler = new Compiler(moduleLoader ?? new ModuleLoader());
            var errors = compiler.ParseAst("UnitTests", "UnitTests", code);

            errors.PrintErrors();
            errors.Should().BeEmpty();

            return ((AstModuleImpl)compiler.Context.Modules.Modules.First()).Files.First();
        }
    }

    internal class AssemblyManagerBuilder
    {
        private static readonly string _dotNetBasePath = RuntimeEnvironment.GetRuntimeDirectory();
        private readonly AssemblyManager _assemblyManager = new(_dotNetBasePath);

        public AssemblyManagerBuilder(bool preloadDependencies = true)
        {
            _assemblyManager.PreloadDependencies = preloadDependencies;
        }

        public AssemblyManager AssemblyManager => _assemblyManager;

        public IAstModuleLoader ToModuleLoader()
            => new ExternalModuleLoader(_assemblyManager);

        public AssemblyManagerBuilder AddZsharpRuntime()
        {
            _assemblyManager.LoadAssembly("Zsharp.Runtime.dll");
            return this;
        }

        public AssemblyManagerBuilder AddSystemAll()
        {
            AddMsCoreLib();

            var files = Directory.EnumerateFiles(_dotNetBasePath, "System.*.dll");

            foreach (var file in files)
            { 
                try
                { 
                   _assemblyManager.LoadAssembly(file);
                }
                catch (BadImageFormatException)
                { }
            }
            return this;
        }

        public AssemblyManagerBuilder AddMsCoreLib()
        {
            _assemblyManager.LoadAssembly(Path.Combine(_dotNetBasePath, "mscorlib.dll"));
            return this;
        }

        public AssemblyManagerBuilder AddSystemConsole()
        {
            _assemblyManager.LoadAssembly(Path.Combine(_dotNetBasePath, "System.Console.dll"));
            return this;
        }

        public AssemblyManagerBuilder AddSystemRuntime()
        {
            _assemblyManager.LoadAssembly(Path.Combine(_dotNetBasePath, "System.Runtime.dll"));
            return this;
        }

        public AssemblyManagerBuilder AddSystemCollections()
        {
            _assemblyManager.LoadAssembly(Path.Combine(_dotNetBasePath, "System.Collections.dll"));
            return this;
        }
    }
}
