﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Zsharp.AST;
using Zsharp.Dgml;

namespace Zsharp.UnitTests.Dgml
{
    [TestClass]
    public class DgmlBuilderTests
    {
        public TestContext TestContext { get; set; }

        private string GetPath(string fileName)
        {
            return Path.Combine(TestContext.DeploymentDirectory, fileName);
        }

        private void CreateDgmlFile(AstFile file, string fileName)
        {
            var builder = new AstDgmlBuilder();
            builder.CreateCommon();
            builder.WriteFile(file);
            builder.SaveAs(GetPath(fileName));
        }

        private void CreateDgmlNode(AstNode node, string fileName)
        {
            var builder = new AstDgmlBuilder();
            builder.CreateCommon();
            builder.WriteNode(node);
            builder.SaveAs(GetPath(fileName));
        }

        [TestMethod]
        public void File_If()
        {
            const string code =
                "MyFn: (p: U8): Bool" + Tokens.NewLine +
                Tokens.Indent1 + "if p = 42" + Tokens.NewLine +
                Tokens.Indent2 + "return true" + Tokens.NewLine +
                Tokens.Indent1 + "return false" + Tokens.NewLine
                ;

            CreateDgmlFile(Build.File(code), "DgmlBuilderTests_File_If.dgml");
        }

        [TestMethod]
        public void File_IfElse()
        {
            const string code =
                "MyFn: (p: U8): Bool" + Tokens.NewLine +
                Tokens.Indent1 + "if p = 42" + Tokens.NewLine +
                Tokens.Indent2 + "return true" + Tokens.NewLine +
                Tokens.Indent1 + "else" + Tokens.NewLine +
                Tokens.Indent2 + "return false" + Tokens.NewLine
                ;

            CreateDgmlFile(Build.File(code), "DgmlBuilderTests_File_IfElse.dgml");
        }

        [TestMethod]
        public void File_IfElseIfElse()
        {
            const string code =
                "MyFn: (p: U8): Bool" + Tokens.NewLine +
                Tokens.Indent1 + "if p = 42" + Tokens.NewLine +
                Tokens.Indent2 + "return true" + Tokens.NewLine +
                Tokens.Indent1 + "else if p > 99" + Tokens.NewLine +
                Tokens.Indent2 + "return true" + Tokens.NewLine +
                Tokens.Indent1 + "else" + Tokens.NewLine +
                Tokens.Indent2 + "return false" + Tokens.NewLine
                ;

            CreateDgmlFile(Build.File(code), "DgmlBuilderTests_File_IfElseIfElse.dgml");
        }

        [TestMethod]
        public void File_IfNested()
        {
            const string code =
                "MyFn: (p: U8): U8" + Tokens.NewLine +
                Tokens.Indent1 + "if p > 10" + Tokens.NewLine +
                Tokens.Indent2 + "if p = 42" + Tokens.NewLine +
                Tokens.Indent3 + "return 1" + Tokens.NewLine +
                Tokens.Indent2 + "return 2" + Tokens.NewLine +
                Tokens.Indent1 + "return 3" + Tokens.NewLine
                ;

            CreateDgmlFile(Build.File(code), "DgmlBuilderTests_File_IfNested.dgml");
        }

        [TestMethod]
        public void Node_Expression()
        {
            const string code =
                "x = 42 + 101 / 2" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var assign = file.CodeBlock.LineAt<AstAssignment>(0);

            CreateDgmlNode(assign.Expression, "DgmlBuilderTests_Node_Expression.dgml");
        }
    }
}
