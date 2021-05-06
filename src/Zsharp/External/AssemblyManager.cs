using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Zsharp.External
{
    public class AssemblyManager
    {
        private readonly List<string> _probePaths = new();
        private readonly List<AssemblyDefinition> _assemblies = new();

        public void AddProbePath(string path)
        {
            _probePaths.Add(path);
        }

        public IEnumerable<AssemblyDefinition> Assemblies => _assemblies;

        public AssemblyDefinition LoadAssembly(string assemblyPath)
        {
            AssemblyDefinition? assemblyDef = null;

            if (File.Exists(assemblyPath))
            {
                assemblyDef = AssemblyDefinition.ReadAssembly(assemblyPath);
                AddAssembly(assemblyDef);
                return assemblyDef;
            }

            foreach (var probePath in _probePaths)
            {
                var path = Path.Combine(probePath, assemblyPath);
                if (File.Exists(path))
                {
                    assemblyDef = AssemblyDefinition.ReadAssembly(path);
                    AddAssembly(assemblyDef);
                    return assemblyDef;
                }
            }

            throw new ArgumentException($"Assembly '{assemblyPath}' could not be loaded.", nameof(assemblyPath));
        }

        private void AddAssembly(AssemblyDefinition assemblyDef)
        {
            if (_assemblies.SingleOrDefault(a => a.FullName == assemblyDef.FullName) != null)
                return;

            _assemblies.Add(assemblyDef);

            foreach (var mod in assemblyDef.Modules)
            {
                foreach (var dep in mod.AssemblyReferences)
                {
                    var depAssembly = mod.AssemblyResolver.Resolve(dep);
                    AddAssembly(depAssembly);
                }
            }
        }
    }
}
