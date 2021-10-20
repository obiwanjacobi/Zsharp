using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Zsharp.External.Metadata;

namespace Zsharp.External
{
    public class AssemblyManager
    {
        private readonly ProbeAssemblyResolver _resolver;
        private readonly MetadataLoadContext _context;
        private readonly Dictionary<string, AssemblyMetadata> _assemblies = new();

        public AssemblyManager(params string[] paths)
        {
            _resolver = new ProbeAssemblyResolver(paths);
            _context = new MetadataLoadContext(_resolver);
        }

        public IEnumerable<AssemblyMetadata> Assemblies => _assemblies.Values;

        public AssemblyMetadata? LoadAssembly(AssemblyName assemblyName)
        {
            return LoadAssembly($"{assemblyName.Name}.dll");
        }

        public AssemblyMetadata? LoadAssembly(string assemblyPath)
        {
            if (!assemblyPath.EndsWith(".dll"))
                assemblyPath += ".dll";

            var assembly = _resolver.Resolve(_context, assemblyPath);

            if (assembly is null)
            {
                return null;
            }

            var assemblyMd = new AssemblyMetadata(assembly);
            AddAssembly(assemblyMd);
            return assemblyMd;
        }

        private void AddAssembly(AssemblyMetadata assemblyMetadata)
        {
            if (!_assemblies.ContainsKey(assemblyMetadata.FullName))
            {
                _assemblies.Add(assemblyMetadata.FullName, assemblyMetadata);

                LoadDependencies(assemblyMetadata);
            }
        }

        private void LoadDependencies(AssemblyMetadata assemblyMetadata)
        {
            foreach (var assemblyName in assemblyMetadata.GetDependencyNames())
            {
                var assembly = LoadAssembly(assemblyName);
                if (assembly is not null)
                    AddAssembly(assembly);
            }
        }

        private class ProbeAssemblyResolver : MetadataAssemblyResolver
        {
            private readonly IEnumerable<string> _probePaths;
            public ProbeAssemblyResolver(IEnumerable<string> probePaths)
            {
                _probePaths = probePaths ?? throw new ArgumentNullException(nameof(probePaths));
            }

            public override Assembly? Resolve(MetadataLoadContext context, AssemblyName assemblyName)
            {
                return Resolve(context, $"{assemblyName.Name}.dll");
            }

            public Assembly? Resolve(MetadataLoadContext context, string assemblyName)
            {
                if (File.Exists(assemblyName))
                {
                    return context.LoadFromAssemblyPath(assemblyName);
                }

                foreach (var path in _probePaths)
                {
                    var filePath = Path.Combine(path, assemblyName);

                    if (File.Exists(filePath))
                    {
                        return context.LoadFromAssemblyPath(filePath);
                    }
                }

                return null;
            }
        }

    }
}
