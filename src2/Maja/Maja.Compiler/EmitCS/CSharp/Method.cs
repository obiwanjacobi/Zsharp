using System.Collections.Generic;

namespace Maja.Compiler.EmitCS.CSharp;

internal sealed class Method
{
    public Method(string name, string typeName)
    {
        Name = name;
        TypeName = typeName;
    }

    public AccessModifiers AccessModifiers { get; set; }

    public MethodModifiers MethodModifiers { get; set; }

    public string Name { get; }

    public string TypeName { get; }

    private readonly List<Parameter> _parameters = new();
    public IEnumerable<Parameter> Parameters => _parameters;

    public void AddParameter(Parameter parameter)
        => _parameters.Add(parameter);
}

internal sealed class Parameter
{
    public Parameter(string name, string typeName)
    {
        Name = name;
        TypeName = typeName;
    }

    public string Name { get; }
    public string TypeName { get; }
}
