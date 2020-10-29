using Mono.Cecil;
using System.Collections.Generic;
using System.IO;
using Zsharp.AST;

namespace Zsharp.Semantics
{
    public class ExternalModuleLoader : IAstModuleLoader
    {
        private readonly List<string> _probePaths = new List<string>();

        public void AddProbePath(string path)
        {
            _probePaths.Add(path);
        }

        public AstModuleExternal? LoadExternal(string moduleName)
        {
            foreach (var probePath in _probePaths)
            {
                var path = Path.Combine(probePath, moduleName + ".dll");
                if (File.Exists(path))
                {
                    return LoadAssembly(path);
                }
            }
            return null;
        }

        private AstModuleExternal? LoadAssembly(string path)
        {
            var assemblyDef = AssemblyDefinition.ReadAssembly(path);
            return new AstModuleExternal(assemblyDef);
        }
    }
}
