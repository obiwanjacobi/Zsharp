using System.IO;
using System.Runtime.CompilerServices;
using Maja.Compiler.EmitCS;
using Maja.Compiler.EmmitCS.CSharp.Project;
using Maja.UnitTests.Compiler.IR;

namespace Maja.UnitTests.Compiler.EmitCS;

internal static class Emit
{
    public static string FromCode(string code, ITestOutputHelper? output = null, [CallerMemberName] string callerName = "")
    {
        var program = Ir.Build(code, allowError: false, source: callerName);
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

    public static void AssertBuild(string emit, ITestOutputHelper? output = null, [CallerMemberName] string callerName = "")
    {
        var result = Build(emit, callerName);

        if (output is not null)
            output.WriteLine(result);

        result.Should().NotBeNullOrEmpty()
            .And.NotContain("ERROR")
            .And.NotContain("FAILED.");
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
