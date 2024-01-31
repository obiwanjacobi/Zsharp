using System.Collections.Generic;

namespace Maja.Compiler.EmitCS.CSharp;

internal sealed class Method
{
    public Method(string name, string typeName)
    {
        Name = name;
        TypeName = typeName;
        Body = new CSharpWriter();
    }

    public AccessModifiers AccessModifiers { get; set; }

    public MethodModifiers MethodModifiers { get; set; }

    public string Name { get; }

    public string TypeName { get; }

    private readonly List<TypeParameter> _typeParameters = new();
    public IEnumerable<TypeParameter> TypeParameters => _typeParameters;

    public void AddTypeParameter(TypeParameter parameter)
        => _typeParameters.Add(parameter);

    private readonly List<Parameter> _parameters = new();
    public IEnumerable<Parameter> Parameters => _parameters;

    public void AddParameter(Parameter parameter)
        => _parameters.Add(parameter);

    public CSharpWriter Body { get; }
}

internal sealed class TypeParameter
{
    public TypeParameter(string typeName)
    {
        TypeName = typeName;
    }

    public string TypeName { get; }
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
