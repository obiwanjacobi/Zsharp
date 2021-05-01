using System;
using System.Diagnostics;
using System.IO;

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
            var config = Debug ? "Debug" : "Release";
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"build -c {config}",
                WorkingDirectory = path,
                RedirectStandardOutput = true,
            };
            using var proc = Process.Start(startInfo);
            proc.WaitForExit();
            return proc.StandardOutput.ReadToEnd();
        }

        private string GetProjectPath()
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
