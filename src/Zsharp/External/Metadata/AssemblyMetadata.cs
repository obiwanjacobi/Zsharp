using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Zsharp.External.Metadata
{
    public sealed class AssemblyMetadata
    {
        private readonly Assembly _assembly;

        public AssemblyMetadata(Assembly assembly)
        {
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        public string FullName => _assembly.FullName!;

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

        public IEnumerable<AssemblyName> GetDependencyNames() => _assembly.GetReferencedAssemblies();
    }
}
