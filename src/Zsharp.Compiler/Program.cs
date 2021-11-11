using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Zsharp;
using Zsharp.AST;
using Zsharp.External;

namespace ZsharpCompiler
{
    public static class Program
    {
        public static void Main(string file, string? config, string? output, string[] @ref)
        {
            var console = new ConsoleWriter(OutputLevel.Verbose);

            if (console.DisplayBanner)
            {
                console.BannerLine("Z# Compiler v0.1");
                console.BannerLine("Copyright (c) Jacobi Software 2021");
            }

            if (String.IsNullOrEmpty(file))
            {
                console.ErrorLine("There is no file specified to compile. Use the -h option to get help.");
                return;
            }

            if (!Path.IsPathRooted(file))
                file = Path.GetFullPath(file);

            var dotnetDir = RuntimeEnvironment.GetRuntimeDirectory();
            console.InfoLine($"Using .NET from: {dotnetDir}");
            
            var assemblyManager = new AssemblyManager(dotnetDir);
            foreach (var name in @ref)
            {
                if(assemblyManager.LoadAssembly(name) is null)
                    console.ErrorLine($"Assembly {name} could not be loaded.");
            }

            if (console.Level == OutputLevel.Verbose)
            { 
                console.InfoLine("Loaded Assemblies:");
                foreach (var assembly in assemblyManager.Assemblies)
                {
                    console.InfoLine($"  - {assembly.Name}");
                }
            }

            var moduleLoader = new ExternalModuleLoader(assemblyManager);
            var compiler = new Compiler(moduleLoader);

            if (console.Level == OutputLevel.Debug)
            {
                console.DebugLine("Loaded Modules:");
                foreach (var module in moduleLoader.SymbolTable.FindSymbols(AstSymbolKind.Module))
                {
                    console.DebugLine(module.SymbolName.FullName);
                }
            }

            console.ProgressLine();
            console.ProgressLine($"Compiling: {file}.");

            if (String.IsNullOrEmpty(config))
                config = "Release";
            if (String.IsNullOrEmpty(output))
                output = Path.Combine(Path.GetDirectoryName(file) ?? ".\\", Path.GetFileNameWithoutExtension(file));
            
            var result = compiler.Compile(file, config, output, 
                assemblyManager.Assemblies.Select(a => a.Loaction).ToArray());

            console.ProgressLine(result);
            console.ProgressLine("Done.");
        }
    }

    internal class ConsoleWriter
    {
        public bool DisplayBanner {get;set;}
        public OutputLevel Level { get; }

        public ConsoleWriter(OutputLevel level)
        {
            Level = level;
            DisplayBanner = true;
        }

        public void BannerLine(string text)
        {
            if (DisplayBanner)
                Console.WriteLine(text);
        }

        public void InfoLine(string text)
        {
            if (Level == OutputLevel.Verbose ||
                Level == OutputLevel.Debug)
                Console.WriteLine(text);
        }

        public void DebugLine(string text)
        {
            if (Level == OutputLevel.Debug)
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

        public void ErrorLine(string text)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {text}");
            Console.ForegroundColor = color;
        }
    }

    internal enum OutputLevel
    {
        Debug,
        Verbose,
        Normal,
    }
}
