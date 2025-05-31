using System;
using System.Collections.Generic;

namespace Maja.Compiler.EmitCS.CSharp;

internal sealed class Namespace
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
    public IEnumerable<string> Usings
        => _usings;

    public void AddUsing(string namespaceName)
    {
        if (!_usings.Contains(namespaceName))
            _usings.Add(namespaceName);
    }

    private readonly List<Type> _types = new();
    public IEnumerable<Type> Types
        => _types;

    public void AddType(Type type)
        => _types.Add(type);

    private readonly List<Enum> _enums = new();
    public IEnumerable<Enum> Enums => _enums;

    public void AddEnum(Enum @enum)
        => _enums.Add(@enum);

    public Type GetModuleClass()
        => _types[0];
}
