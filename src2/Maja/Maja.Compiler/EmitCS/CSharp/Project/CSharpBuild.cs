using System;
using System.Diagnostics;
using System.IO;

namespace Maja.Compiler.EmmitCS.CSharp.Project;

internal sealed class CSharpBuild
{
    public CSharpBuild()
    {
        Project = new CSharpProject();
        ProjectPath = ".maja";
    }

    public string ProjectPath { get; set; }

    public bool Debug { get; set; }

    public CSharpProject Project { get; set; }

    public string Compile(string assemblyName)
    {
        var path = GetProjectPath();

        if (!Directory.Exists(path))
            _ = Directory.CreateDirectory(path);

        var projectFilePath = Path.Combine(path, $"{assemblyName}.csproj");
        Project.SaveAs(projectFilePath);
        Project.TargetPath = Path.Combine(path, "bin",
            Debug ? "Debug" : "Release",
            Project.TargetFrameworkMoniker,
            $"{assemblyName}.dll");

        return Build(projectFilePath);
    }

    private string Build(string projectFilePath)
    {
        var path = Path.GetDirectoryName(projectFilePath) ?? ".\\";
        var projectFile = Path.GetFileName(projectFilePath);

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = BuildCommandLine(projectFile),
            WorkingDirectory = path,
            RedirectStandardOutput = true,
        };

        using var process = Process.Start(startInfo);
        if (process is not null)
        {
            var result = process.StandardOutput.ReadToEnd();

            if (!process.HasExited &&
                !process.WaitForExit(90000))
                throw new Exception($"The dotnet build process timed out.\n{result}");

            return result;
        }

        return "Build FAILED - dotnet process could not be started.";
    }

    private string BuildCommandLine(string projectFile)
    {
        var config = Debug ? "Debug" : "Release";
        return $"build -c {config} {projectFile}";
    }

    public string GetProjectPath()
    {
        if (Path.IsPathRooted(ProjectPath))
            return ProjectPath;

        //var dir = Directory.GetCurrentDirectory();
        var dir = Environment.CurrentDirectory;
        //var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        return Path.Combine(dir, ProjectPath);
    }
}