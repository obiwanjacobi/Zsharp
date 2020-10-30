using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;

namespace Zsharp.Semantics
{
    public class AssemblyManager
    {
        private readonly List<string> _probePaths = new List<string>();
        private readonly List<AssemblyDefinition> _assemblies = new List<AssemblyDefinition>();

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
                _assemblies.Add(assemblyDef);
                return assemblyDef;
            }

            foreach (var probePath in _probePaths)
            {
                var path = Path.Combine(probePath, assemblyPath);
                if (File.Exists(path))
                {
                    assemblyDef = AssemblyDefinition.ReadAssembly(path);
                    _assemblies.Add(assemblyDef);
                    return assemblyDef;
                }
            }

            throw new ArgumentException($"Assembly '{assemblyPath}' could not be loaded.", nameof(assemblyPath));
        }
    }
}
