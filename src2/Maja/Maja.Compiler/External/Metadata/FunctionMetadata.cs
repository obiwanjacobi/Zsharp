using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;


namespace Maja.Compiler.External.Metadata;

[DebuggerDisplay("{Name}")]
internal class FunctionMetadata
{
    private readonly MethodInfo? _method;
    private readonly ConstructorInfo? _constructor;
    private readonly TypeMetadata _type;

    public FunctionMetadata(MethodInfo method, TypeMetadata type)
    {
        _method = method ?? throw new ArgumentNullException(nameof(method));
        _type = type;
    }

    public FunctionMetadata(ConstructorInfo constructor, TypeMetadata type)
    {
        _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        _type = type;
    }

    public string Name
        => _method?.Name ?? _constructor?.Name ?? throw _panic;

    public bool IsStatic
        => _method?.IsStatic ?? _constructor?.IsStatic ?? throw _panic;

    private TypeMetadata? _retType;
    public TypeMetadata ReturnType
        => _retType ??= new TypeMetadata(
            _method?.ReturnType ?? _constructor?.DeclaringType ?? throw _panic, _type.Assembly);

    private List<ParameterMetadata>? _params;
    public IEnumerable<ParameterMetadata> Parameters
        => _params ??= new(
            (_method?.GetParameters() ?? _constructor?.GetParameters() ?? throw _panic)
            .Select(p => new ParameterMetadata(p)));

    private List<GenericParameterMetadata>? _genTypes;
    public IEnumerable<GenericParameterMetadata> GenericParameters
        => _genTypes ??= new(
            (_method?.GetGenericArguments() ?? Enumerable.Empty<Type>())
            .Select(g => new GenericParameterMetadata(g)));


    public TypeMetadata GetDeclaringType()
    {
        return _type;
    }

    private static readonly /*InternalError*/Exception _panic = new("No method or constructor object set.");
}

[DebuggerDisplay("{Name} ({Operator})")]
internal sealed class OperatorFunctionMetadata : FunctionMetadata
{
    public OperatorFunctionMetadata(MethodInfo method, OperatorAttribute attribute, TypeMetadata type)
        : base(method, type)
    {
        _attribute = attribute;
    }

    public string Operator => _attribute.Symbol;

    private readonly OperatorAttribute _attribute;
    public OperatorAttribute Attribute => _attribute;
}
