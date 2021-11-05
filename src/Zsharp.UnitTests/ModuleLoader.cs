using System.Collections.Generic;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.UnitTests
{
    public class ModuleLoader : IAstModuleLoader
    {
        public List<AstModuleExternal> Modules { get; } = new List<AstModuleExternal>();

        public AstSymbolTable SymbolTable { get; private set; }

        public void Initialize(AstSymbolTable symbolTable)
            => SymbolTable = symbolTable;

        public AstModuleExternal LoadExact(string fullModuleName)
        {
            var mod = Modules.FirstOrDefault(m => m.Identifier.NativeFullName == fullModuleName);

            if (mod is null)
            {
                mod = new AstModuleExternal(fullModuleName, SymbolTable);
                Modules.Add(mod);
            }
            return mod;
        }

        public IEnumerable<AstModuleExternal> LoadNamespace(string moduleNamespace)
            => Modules.Where(m => m.Identifier.NativeFullName.StartsWith(moduleNamespace));

        public IEnumerable<AstModuleExternal> LoadAll(string partialModuleName)
            => Modules.Where(m => m.Identifier.NativeFullName.StartsWith(partialModuleName));
    }
}
