using System;
using System.Collections.Generic;
using System.Linq;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstModuleManager
    {
        private readonly Dictionary<string, AstModule> _modules = new();
        private readonly AstSymbolTable _externalSymbolTable;
        private readonly IAstModuleLoader _moduleLoader;

        public AstModuleManager(AstSymbolTable intrinsicSymbols, IAstModuleLoader moduleLoader)
        {
            _moduleLoader = moduleLoader;
            _externalSymbolTable = new AstSymbolTable(String.Empty, intrinsicSymbols);
            _moduleLoader.Initialize(_externalSymbolTable);

            var runtimeModules = _moduleLoader.LoadAll("Zsharp.Runtime");
            foreach (var rtMod in runtimeModules)
            {
                var symbol = intrinsicSymbols.Add(rtMod);
                symbol.SymbolLocality = AstSymbolLocality.Imported;
            }
        }

        public AstSymbolTable SymbolTable => _externalSymbolTable;

        public IEnumerable<AstModule> Modules => _modules.Values;

        public AstModuleImpl AddModule(Statement_moduleContext moduleCtx)
        {
            Ast.Guard(moduleCtx, "Context is null.");
            var moduleName = AstSymbolName.ToCanonical(moduleCtx.module_name().GetText());
            var module = CreateModule(moduleName);
            module.AddModule(moduleCtx);
            return module;
        }

        private AstModuleImpl CreateModule(string moduleName)
        {
            var module = new AstModuleImpl(moduleName);
            _modules.Add(moduleName, module);
            SymbolTable.Add(module);
            return module;
        }

        public AstModule GetOrAddModule(string moduleName)
        {
            if (!_modules.TryGetValue(moduleName, out AstModule? module))
                module = CreateModule(moduleName);

            return module;
        }

        public IEnumerable<T> FindModules<T>(string moduleNamespace)
            where T : AstModule
        {
            return _modules.Keys
                .Where(k => k.StartsWith($"{moduleNamespace}."))
                .Select(k => _modules[k] as T)
                .Where(m => m is not null)
                .ToList()!;
        }

        public T? FindModule<T>(string moduleName)
            where T : AstModule
        {
            _modules.TryGetValue(moduleName, out AstModule? module);
            return module as T;
        }

        public IEnumerable<AstModuleExternal> ImportNamespace(string moduleNamespace)
        {
            var modules = _moduleLoader.LoadNamespace(moduleNamespace);
            foreach (var module in modules)
            {
                var moduleName = module.Identifier!.CanonicalName;
                var astModule = FindModule<AstModuleExternal>(moduleName);
                if (astModule is null)
                {
                    _modules.Add(moduleName, module);
                    _ = _externalSymbolTable.Add(module);
                }
            }
            return modules;
        }

        public AstModuleExternal? Import(string moduleName)
        {
            var module = FindModule<AstModuleExternal>(moduleName);
            if (module is null)
            {
                module = _moduleLoader.LoadExact(moduleName);
                if (module is not null)
                {
                    _modules.Add(moduleName, module);
                    _ = _externalSymbolTable.Add(module);
                }
            }
            return module;
        }
    }
}
