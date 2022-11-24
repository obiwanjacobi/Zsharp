using System.IO;
using System.Runtime.CompilerServices;
using Maja.Compiler.EmitCS;
using Maja.Compiler.EmmitCS.CSharp.Project;
using Maja.UnitTests.IR;
using Xunit.Abstractions;

namespace Maja.UnitTests.EmitCS;

internal static class Emit
{
    public static string FromCode(string code, ITestOutputHelper? output = null)
    {
        var program = Ir.Build(code);
        var builder = new CodeBuilder();
        var ns = builder.OnProgram(program);
        if (output is not null)
        {
            var dump = ObjectDumper.Dump(ns);
            output.WriteLine(dump);
            output.WriteLine("");
        }
        return builder.ToString();
    }

    public static string Build(string code, [CallerMemberName] string callerName = "")
    {
        var path = $"./{callerName}";
        Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(path, $"{callerName}.cs"), code);

        var build = new CSharpBuild()
        {
            Project = new CSharpProject
            {
                TargetPath = $"{callerName}/bin"
            },
            ProjectPath = path
        };

        return build.Compile(callerName);
    }
}
