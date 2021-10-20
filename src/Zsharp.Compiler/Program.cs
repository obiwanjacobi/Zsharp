using System;
using System.Linq;
using System.Runtime.InteropServices;
using Zsharp;
using Zsharp.AST;
using Zsharp.External;

namespace ZsharpCompiler
{
    public static class Program
    {
        public static void Main(string filePath, string? configuration, string? output, string[] reference)
        {
            var console = new ConsoleWriter(OutputLevel.Verbose);

            console.BannerLine("Z# Compiler v0.1");
            console.BannerLine("Copyright (c) Jacobi Software 2021");
            console.InfoLine($"Using .NET from: {RuntimeEnvironment.GetRuntimeDirectory()}");
            console.InfoLine("Loaded Assemblies:");
            var assemblyManager = new AssemblyManager(RuntimeEnvironment.GetRuntimeDirectory());
            foreach (var name in reference)
            {
                _ = assemblyManager.LoadAssembly(name);
                console.InfoLine(name);
            }
            
            var moduleLoader = new ExternalModuleLoader(assemblyManager);
            var compiler = new Compiler(moduleLoader);

            console.DebugLine("Loaded Modules:");
            foreach (var module in moduleLoader.SymbolTable.FindSymbols(AstSymbolKind.Module))
            {
                console.DebugLine(module.FullName);
            }

            console.ProgressLine();
            console.ProgressLine($"Compiling: {filePath}.");

            if (String.IsNullOrEmpty(configuration))
                configuration = "Release";
            if (String.IsNullOrEmpty(output))
                output = ".\\";
            
            var result = compiler.Compile(filePath, configuration, output, 
                assemblyManager.Assemblies.Select(a => a.Loaction).ToArray());

            console.ProgressLine(result);
            console.ProgressLine("Done.");
        }
    }

    internal class ConsoleWriter
    {
        private readonly OutputLevel _level;

        public ConsoleWriter(OutputLevel level)
        {
            _level = level;
        }

        public void BannerLine(string text)
        {
            if (_level != OutputLevel.NoBanner)
                Console.WriteLine(text);
        }

        public void InfoLine(string text)
        {
            if (_level == OutputLevel.Verbose ||
                _level == OutputLevel.Debug)
                Console.WriteLine(text);
        }

        public void DebugLine(string text)
        {
            if (_level == OutputLevel.Debug)
                Console.WriteLine(text);
        }

        public void ProgressLine(string text)
        {
            Console.WriteLine(text);
        }

        public void ProgressLine()
        {
            Console.WriteLine();
        }
    }

    internal enum OutputLevel
    {
        Debug,
        Verbose,
        NoBanner,
        Normal,
    }
}
