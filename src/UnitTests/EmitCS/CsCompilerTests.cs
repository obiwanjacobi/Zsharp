using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
using Zsharp.EmitCS;

namespace UnitTests.EmitCS
{
    [TestClass]
    public class CsCompilerTests
    {
        [TestMethod]
        public void ProjectDebug()
        {
            var path = Path.Combine(Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location), "csout");

            var csc = new CsCompiler
            {
                ProjectPath = path,
                Debug = true
            };

            var stdOutput = csc.Compile("CsTestAssembly");
            Console.WriteLine(stdOutput);

            File.Exists(csc.Project.TargetPath).Should().BeTrue();
        }

        [TestMethod]
        public void ProjectRelease()
        {
            var path = Path.Combine(Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location), "csout");

            var csc = new CsCompiler
            {
                ProjectPath = path,
                Debug = false
            };

            var stdOutput = csc.Compile("CsTestAssembly");
            Console.WriteLine(stdOutput);

            File.Exists(csc.Project.TargetPath).Should().BeTrue();
        }
    }
}
