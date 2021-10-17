using System;
using Zsharp;
using Zsharp.External;

namespace ZsharpCompiler
{
    internal static class Program
    {
        static void Main(string filePath, string? configuration, string? output, string[] reference)
        {
            var assemblyManager = new AssemblyManager();
            var moduleLoader = new ExternalModuleLoader(assemblyManager);
            var compiler = new Compiler(moduleLoader);

            Console.WriteLine($"Compiling: {filePath}.");

            if (String.IsNullOrEmpty(configuration))
                configuration = "Release";
            if (String.IsNullOrEmpty(output))
                output = ".\\";

            var result = compiler.Compile(filePath, configuration, output, reference);

            Console.WriteLine(result);
            Console.WriteLine("Done.");
        }
    }
}
