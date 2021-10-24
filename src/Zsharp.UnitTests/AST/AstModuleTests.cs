using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.UnitTests.AST
{
    [TestClass]
    public class AstModuleTests
    {
        [TestMethod]
        public void LoadAllExternal_System()
        {
            var symbolTable = new AstSymbolTable();
            var loader = Compile.CreateModuleLoader();
            loader.Initialize(symbolTable);

            var sysMods = loader.LoadAll("System");
            sysMods.Should().NotBeEmpty();
            sysMods.All(m => m.Symbols.Namespace.StartsWith("System"))
                .Should().BeTrue();
            sysMods.All(m => m.Symbols.FindSymbols(AstSymbolKind.Function)
                .All(e => e.SymbolLocality == AstSymbolLocality.Imported))
                .Should().BeTrue();
            sysMods.All(m => m.Symbols.FindSymbols(AstSymbolKind.Type).Where(s => s.HasDefinition)
                .All(e => e.SymbolLocality == AstSymbolLocality.Imported))
                .Should().BeTrue();
        }

        [TestMethod]
        public void LoadNamespaceExternal_System()
        {
            var symbolTable = new AstSymbolTable();
            var loader = Compile.CreateModuleLoader();
            loader.Initialize(symbolTable);

            var sysMods = loader.LoadNamespace("System");
            sysMods.Should().NotBeEmpty();
            sysMods.All(m => m.Symbols.Namespace.StartsWith("System."))
                .Should().BeTrue();
            sysMods.All(m => m.Symbols.FindSymbols(AstSymbolKind.Function)
                .All(e => e.SymbolLocality == AstSymbolLocality.Imported))
                .Should().BeTrue();
            sysMods.All(m => m.Symbols.FindSymbols(AstSymbolKind.Type).Where(s => s.HasDefinition)
                .All(e => e.SymbolLocality == AstSymbolLocality.Imported))
                .Should().BeTrue();
        }

        [TestMethod]
        public void ImportExternalNamespace()
        {
            const string code =
                "import System.*" + Tokens.NewLine
                ;

            var file = Build.File(code, Compile.CreateModuleLoader());
            var symbols = file.Symbols;
            symbols.Symbols.Any(e => e is null).Should().BeFalse();

            foreach(var mod in symbols.FindSymbols(AstSymbolKind.Module))
            { 
                mod.SymbolName.Should().StartWith("System.");
            }
        }

        [TestMethod]
        public void ImportExternalModuleName()
        {
            const string code =
                "import System.Console" + Tokens.NewLine
                ;

            var file = Build.File(code, Compile.CreateModuleLoader());
            var symbols = file.Symbols;
            symbols.Symbols.Any(e => e is null).Should().BeFalse();

            var mod = symbols.FindSymbols(AstSymbolKind.Module).Single();
            mod.SymbolName.Should().Be("System.Console");
            // System.System.Console...
            //mod.FullName.Should().Be("System.Console");
        }

        [TestMethod]
        public void ImportExternalModuleAlias()
        {
            const string code =
                "import Print = System.Console.WriteLine" + Tokens.NewLine
                ;

            var file = Build.File(code, Compile.CreateModuleLoader());
            var symbols = file.Symbols;
            symbols.Symbols.Any(e => e is null).Should().BeFalse();

            var mod = symbols.FindSymbols(AstSymbolKind.Module).Single();
            mod.SymbolName.Should().Be("System.Console");
            // System.System.Console...
            //mod.FullName.Should().Be("System.Console");
        }
    }
}
