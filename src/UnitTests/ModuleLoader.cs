using System.Collections.Generic;
using System.Linq;
using Zsharp.AST;

namespace UnitTests
{
    public class ModuleLoader : IAstModuleLoader
    {
        public List<AstModuleExternal> Modules { get; } = new List<AstModuleExternal>();

        public AstModuleExternal LoadExternal(string moduleName)
        {
            var mod = Modules.FirstOrDefault(m => m.Name == moduleName);

            if (mod == null)
            {
                mod = new AstModuleExternal(moduleName);
                Modules.Add(mod);
            }
            return mod;
        }
    }
}
