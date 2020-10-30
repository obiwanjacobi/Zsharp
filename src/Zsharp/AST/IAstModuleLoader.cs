namespace Zsharp.AST
{
    public interface IAstModuleLoader
    {
        AstModuleExternal? LoadExternal(string moduleName);
    }
}
