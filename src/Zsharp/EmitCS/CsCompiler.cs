using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Zsharp.EmitCS
{
    public sealed class CsCompiler
    {
        public CsCompiler()
        {
            Project = new Project();
            ProjectPath = ".";
        }

        public string ProjectPath { get; set; }

        public bool Debug { get; set; }

        public Project Project { get; }

        public string Compile(string assemblyName)
        {
            var path = GetProjectPath();

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            Project.SaveAs(Path.Combine(path, $"{assemblyName}.csproj"));
            Project.TargetPath = Path.Combine(path, "bin",
                Debug ? "Debug" : "Release",
                Project.TargetFrameworkMoniker,
                $"{assemblyName}.dll");

            return Build(path);
        }

        private string Build(string path)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = BuildCommandLine(),
                WorkingDirectory = path,
                RedirectStandardOutput = true,
            };

            using var proc = Process.Start(startInfo);
            if (proc is not null)
            {
                var result = proc.StandardOutput.ReadToEnd();

                if (!proc.HasExited &&
                    !proc.WaitForExit(90000))
                    throw new ZsharpException($"The dotnet build process timed out.\n{result}");

                return result;
            }

            return "Build FAILED - dotnet process could not be started.";
        }

        private string BuildCommandLine()
        {
            var config = Debug ? "Debug" : "Release";
            var cmdLine = new StringBuilder();

            cmdLine.Append($"build -c {config}");

            return cmdLine.ToString();
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
}
