using System.Collections.Generic;

namespace Zsharp.AST
{
    public interface IAstModuleLoader
    {
        void Initialize(AstSymbolTable symbolTable);

        AstSymbolTable SymbolTable { get; }

        AstModuleExternal? LoadExact(string fullModuleName);

        IEnumerable<AstModuleExternal> LoadAll(string partialModuleName);
    }
}
