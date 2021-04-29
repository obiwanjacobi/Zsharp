using System;
using System.Collections.Generic;

namespace Zsharp.EmitCS.CSharp
{
    internal class Namespace
    {
        public Namespace(string ns, string? name = null)
        {
            if (!String.IsNullOrEmpty(name))
                Name = $"{ns}.{name}";
            else
                Name = ns;
        }

        public string Name { get; }

        private readonly List<string> _usings = new();
        public IEnumerable<string> Usings => _usings;

        public void AddUsing(string namespaceName)
        {
            _usings.Add(namespaceName);
        }

        private readonly List<Class> _classes = new();
        public IEnumerable<Class> Classes => _classes;

        public void AddClass(Class @class)
        {
            _classes.Add(@class);
        }
    }
}
