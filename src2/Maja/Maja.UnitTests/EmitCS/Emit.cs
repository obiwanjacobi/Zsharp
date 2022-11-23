using System.IO;
using System.Runtime.CompilerServices;
using Maja.Compiler.EmitCS;
using Maja.Compiler.EmmitCS.CSharp.Project;
using Maja.UnitTests.IR;

namespace Maja.UnitTests.EmitCS;

internal static class Emit
{
    public static string FromCode(string code)
    {
        var program = Ir.Build(code);
        var builder = new CodeBuilder();
        builder.OnProgram(program);
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
