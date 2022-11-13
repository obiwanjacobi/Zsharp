using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Maja.Compiler.External.Metadata;

namespace Maja.Compiler.External;

internal sealed class AssemblyManager
{
    private readonly ProbeAssemblyResolver _resolver;
    private readonly MetadataLoadContext _context;
    private readonly Dictionary<string, AssemblyMetadata> _assemblies = new();

    public AssemblyManager(params string[] paths)
    {
        _resolver = new ProbeAssemblyResolver(paths);
        _context = new MetadataLoadContext(_resolver);
    }

    public IEnumerable<AssemblyMetadata> Assemblies
        => _assemblies.Values;

    public bool PreloadDependencies { get; set; }

    public AssemblyMetadata? LoadAssembly(AssemblyName assemblyName)
        => LoadAssembly($"{assemblyName.Name}.dll");

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
        if (!TryAddAssembly(assemblyMd))
        {
            assemblyMd = _assemblies[assemblyMd.FullName];
        }
        return assemblyMd;
    }

    private bool TryAddAssembly(AssemblyMetadata? assemblyMetadata)
    {
        if (assemblyMetadata is null) return false;

        if (!_assemblies.ContainsKey(assemblyMetadata.FullName))
        {
            _assemblies.Add(assemblyMetadata.FullName, assemblyMetadata);

            if (PreloadDependencies)
                LoadDependencies(assemblyMetadata);

            return true;
        }

        return false;
    }

    private void LoadDependencies(AssemblyMetadata assemblyMetadata)
    {
        foreach (var assemblyName in assemblyMetadata.GetDependencyNames())
        {
            var assembly = LoadAssembly(assemblyName);
            _ = TryAddAssembly(assembly);
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
            => Resolve(context, $"{assemblyName.Name}.dll");

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