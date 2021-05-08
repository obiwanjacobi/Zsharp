using System.Collections.Generic;
using System.Linq;
using Zsharp.AST;

namespace UnitTests
{
    public class ModuleLoader : IAstModuleLoader
    {
        public List<AstModuleExternal> Modules { get; } = new List<AstModuleExternal>();

        public AstSymbolTable SymbolTable { get; private set; }

        public void Initialize(AstSymbolTable symbolTable)
            => SymbolTable = symbolTable;

        public AstModuleExternal LoadExternal(string moduleName)
        {
            var mod = Modules.FirstOrDefault(m => m.Identifier.Name == moduleName);

            if (mod == null)
            {
                mod = new AstModuleExternal(moduleName, SymbolTable);
                Modules.Add(mod);
            }
            return mod;
        }
    }
}
