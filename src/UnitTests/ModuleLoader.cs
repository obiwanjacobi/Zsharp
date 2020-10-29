using Zsharp.AST;

namespace UnitTests
{
    public class ModuleLoader : IAstModuleLoader
    {
        public AstModuleExternal LoadExternal(string moduleName)
        {
            return new AstModuleExternal(moduleName);
        }
    }
}
