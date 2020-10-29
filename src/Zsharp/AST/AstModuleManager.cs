using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstModuleManager
    {
        private readonly Dictionary<string, AstModule> _modules = new Dictionary<string, AstModule>();

        public IEnumerable<AstModule> Modules => _modules.Values;

        public AstModule AddModule(Statement_moduleContext moduleCtx)
        {
            Ast.Guard(moduleCtx, "Context is null.");
            var moduleName = moduleCtx.module_name().GetText();

            if (!_modules.TryGetValue(moduleName, out AstModule? module))
            {
                module = new AstModule(moduleName);
                _modules.Add(moduleName, module);
            }

            module.AddModule(moduleCtx);
            return module;
        }

        public AstModule GetOrAddModule(string moduleName)
        {
            if (!_modules.TryGetValue(moduleName, out AstModule? module))
            {
                module = new AstModule(moduleName);
                _modules.Add(moduleName, module);
            }

            return module;
        }

        public AstModule? FindModule(string moduleName)
        {
            _modules.TryGetValue(moduleName, out AstModule? module);
            return module;
        }

        // TODO: this needs to trigger external lookup/loading of module metadata
        public AstModule Import(Statement_importContext importCtx)
        {
            var moduleName = importCtx.module_name().GetText();
            var module = GetOrAddModule(moduleName);
            return module;
        }
    }
}
