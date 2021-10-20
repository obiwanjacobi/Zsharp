using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zsharp.Compiler.Tests
{
    internal static class Compile
    {
        public static void File(string file, string testName)
        {
            var dir = Directory.GetCurrentDirectory();
            var output = Path.Combine(dir, testName, testName);
            var filePath = Path.Combine(".", file);
            ZsharpCompiler.Program.Main(filePath, "Debug", output, new[] 
            { 
                "Zsharp.Runtime.dll", 
                "System.Console.dll" 
            });
        }
    }
}
