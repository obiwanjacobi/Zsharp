namespace Zsharp.AST
{
    public interface IAstModuleLoader
    {
        void Initialize(AstSymbolTable symbolTable);

        AstSymbolTable SymbolTable { get; }

        AstModuleExternal? LoadExternal(string moduleName);
    }
}
