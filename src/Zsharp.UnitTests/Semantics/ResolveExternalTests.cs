using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.UnitTests.Semantics
{
    [TestClass]
    public class ResolveExternalTests
    {
        [TestMethod]
        public void ExternalZsharpConversion()
        {
            const string code =
                "v = U16(42)" + Tokens.NewLine
                ;

            var moduleLoader = new AssemblyManagerBuilder()
                .AddZsharpRuntime()
                .ToModuleLoader();

            var file = Compile.File(code, moduleLoader);

            var assign = file.CodeBlock.LineAt<AstAssignment>(0);
            assign.Expression.RHS.FunctionReference.FunctionDefinition.Should().NotBeNull();
            assign.Expression.RHS.FunctionReference.FunctionDefinition.IsExternal.Should().BeTrue();
        }

        [TestMethod]
        public void ExternalFunctionVoidRet()
        {
            const string code =
                "import System.Console" + Tokens.NewLine +
                "WriteLine(\"Test\")" + Tokens.NewLine
                ;

            var moduleLoader = new AssemblyManagerBuilder()
                .AddSystemConsole()
                .ToModuleLoader();

            var file = Compile.File(code, moduleLoader);

            var fn = file.CodeBlock.LineAt<AstFunctionReference>(0);
            var typeRef = fn.FunctionType.TypeReference;
            typeRef.Symbol.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void ExternalGenericTypeUse()
        {
            const string code =
                "import System.Collections.Generic.List" + Tokens.NewLine +
                "l: List<Str>" + Tokens.NewLine
                ;

            var moduleLoader = new AssemblyManagerBuilder()
                .AddSystemCollections()
                .ToModuleLoader();

            var file = Compile.File(code, moduleLoader);

            var variable = file.CodeBlock.LineAt<AstVariableDefinition>(0);
            var typeRef = variable.TypeReference as AstTypeReferenceType;
            typeRef.IsTemplateOrGeneric.Should().BeTrue();

            var templArg = typeRef.TemplateArguments.ElementAt(0);
            templArg.HasParameterDefinition.Should().BeTrue();
            templArg.ParameterDefinitionAs<AstGenericParameterDefinition>().Should().NotBeNull();
            var templType = templArg.TypeReference as AstTypeReferenceType;
            templType.TypeDefinition.Should().NotBeNull();
        }

        [TestMethod]
        public void ExternalGenericTypeConstruction()
        {
            const string code =
                "import System.Collections.Generic.List" + Tokens.NewLine +
                "l = List<Str>()" + Tokens.NewLine
                ;

            var moduleLoader = new AssemblyManagerBuilder()
                .AddSystemCollections()
                .ToModuleLoader();

            var file = Compile.File(code, moduleLoader);

            var assign = file.CodeBlock.LineAt<AstAssignment>(0);

            var expr = assign.Expression;
            expr.Should().NotBeNull();
            expr.RHS.FunctionReference.Should().NotBeNull();


            var variable = assign.Variable as AstVariableDefinition;
            variable.Should().NotBeNull();
            var typeRef = variable.TypeReference;
            var typeDef = typeRef.TypeDefinition;
            typeDef.Should().NotBeNull();
        }

        [TestMethod]
        public void ExternalFunctionArray()
        {
            const string code =
                "import Zsharp.Runtime.Types" + Tokens.NewLine +
                "import System.Console" + Tokens.NewLine +
                "arr = Array<C16>(2)" + Tokens.NewLine +
                "WriteLine(arr)" + Tokens.NewLine
                ;

            var moduleLoader = new AssemblyManagerBuilder()
                .AddZsharpRuntime()
                .AddSystemConsole()
                .ToModuleLoader();

            var file = Compile.File(code, moduleLoader);

            var fn = file.CodeBlock.LineAt<AstFunctionReference>(0);
            var typeRef = fn.FunctionType.TypeReference;
            typeRef.Symbol.Definition.Should().NotBeNull();
        }
    }
}
