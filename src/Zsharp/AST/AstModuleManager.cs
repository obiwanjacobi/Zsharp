using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstModuleManager
    {
        private readonly Dictionary<string, AstModule> _modules = new Dictionary<string, AstModule>();
        private readonly IAstModuleLoader _moduleLoader;

        public AstModuleManager(IAstModuleLoader moduleLoader)
        {
            _moduleLoader = moduleLoader;
        }

        public IEnumerable<AstModule> Modules => _modules.Values;

        public AstModulePublic AddModule(Statement_moduleContext moduleCtx)
        {
            Ast.Guard(moduleCtx, "Context is null.");
            var moduleName = moduleCtx.module_name().GetText();

            var module = FindModule<AstModulePublic>(moduleName);
            if (module == null)
            {
                module = new AstModulePublic(moduleName);
                _modules.Add(moduleName, module);
            }

            module.AddModule(moduleCtx);
            return module;
        }

        public AstModule GetOrAddModule(string moduleName)
        {
            if (!_modules.TryGetValue(moduleName, out AstModule? module))
            {
                module = new AstModulePublic(moduleName);
                _modules.Add(moduleName, module);
            }

            return module;
        }

        public T? FindModule<T>(string moduleName)
            where T : AstModule
        {
            _modules.TryGetValue(moduleName, out AstModule? module);
            return module as T;
        }

        public AstModuleExternal? Import(string moduleName)
        {
            var module = FindModule<AstModuleExternal>(moduleName);
            if (module == null)
            {
                module = _moduleLoader.LoadExternal(moduleName);
                if (module != null)
                    _modules.Add(moduleName, module);
            }
            return module;
        }
    }
}
