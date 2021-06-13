using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace UnitTests.AST
{
    [TestClass]
    public class AstModuleTests
    {
        [TestMethod]
        public void LoadExternal_System()
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
    }
}
