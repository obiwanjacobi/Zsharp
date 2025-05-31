using System.IO;
using System.Runtime.CompilerServices;
using Maja.Compiler.EmitCS;
using Maja.Compiler.EmitCS.CSharp.Project;
using Maja.Compiler.EmitCS.IR;
using Maja.Compiler.External;
using Maja.UnitTests.Compiler.IR;

namespace Maja.UnitTests.Compiler.EmitCS;

internal static class Emit
{
    public static string FromCode(string code, IExternalModuleLoader? moduleLoader = null, ITestOutputHelper? output = null, [CallerMemberName] string callerName = "")
    {
        if (moduleLoader is null)
            moduleLoader = new NullModuleLoader();

        var program = Ir.Build(code, moduleLoader, allowError: false, source: callerName);

        var lowering = new IrCodeRewriter();
        var codeProg = lowering.CodeRewrite(program);

        var builder = new CodeBuilder();

        var ns = builder.OnProgram(codeProg);

        if (output is not null)
        {
            var dump = ObjectDumper.Dump(ns);
            output.WriteLine(dump);
            output.WriteLine("");
        }

        return builder.ToString();
    }

    public static void AssertBuild(string emit, IExternalModuleLoader? moduleLoader = null, ITestOutputHelper? output = null, [CallerMemberName] string callerName = "")
    {
        var result = Build(emit, moduleLoader, callerName);

        output?.WriteLine(result);

        result.Should().NotBeNullOrEmpty()
            .And.NotContain("ERROR")
            .And.NotContain("FAILED");
    }

    public static string Build(string code, IExternalModuleLoader? moduleLoader = null, [CallerMemberName] string callerName = "")
    {
        var path = $"./{callerName}";
        Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(path, $"{callerName}.cs"), code);

        var project = new CSharpProject
        {
            TargetPath = $"{callerName}/bin"
        };

        if (moduleLoader is not null)
        {
            foreach (var assembly in moduleLoader.Assemblies)
            {
                project.AddReference(assembly);
            }
        }

        var build = new CSharpBuild()
        {
            Project = project,
            ProjectPath = path
        };

        return build.Compile(callerName);
    }
}
