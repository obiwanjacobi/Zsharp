using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Maja.Compiler.External.Metadata;

[DebuggerDisplay("{Name}")]
internal sealed class AssemblyMetadata
{
    private readonly Assembly _assembly;

    public AssemblyMetadata(Assembly assembly)
    {
        _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
    }

    public string? Name
        => _assembly.GetName().Name;
    public string FullName
        => _assembly.FullName!;
    public string Location
        => _assembly.Location;

    private readonly List<TypeMetadata> _types = new();
    public IEnumerable<TypeMetadata> GetPublicTypes()
    {
        if (_types.Count == 0)
        {
            _types.AddRange(_assembly.GetTypes()
                .Where(t => t.IsPublic)
                .Select(t => new TypeMetadata(t))
                );
        }

        return _types;
    }

    public IEnumerable<AssemblyName> GetDependencyNames()
        => _assembly.GetReferencedAssemblies();
}
