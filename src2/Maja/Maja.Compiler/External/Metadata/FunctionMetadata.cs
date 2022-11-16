using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;


namespace Maja.Compiler.External.Metadata;

[DebuggerDisplay("{Name}")]
internal sealed class FunctionMetadata
{
    private readonly MethodInfo? _method;
    private readonly ConstructorInfo? _constructor;

    public FunctionMetadata(MethodInfo method)
    {
        _method = method ?? throw new ArgumentNullException(nameof(method));
    }

    public FunctionMetadata(ConstructorInfo constructor)
    {
        _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
    }

    public string Name
        => _method?.Name ?? _constructor?.Name ?? throw _panic;

    public bool IsStatic
        => _method?.IsStatic ?? _constructor?.IsStatic ?? throw _panic;

    private TypeMetadata? _retType;
    public TypeMetadata ReturnType
        => _retType ??= new TypeMetadata(
            _method?.ReturnType ?? _constructor?.DeclaringType ?? throw _panic);

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


    private TypeMetadata? _declType;
    public TypeMetadata GetDeclaringType()
    {
        if (_declType is null)
        {
            var declType = _method?.DeclaringType ?? _constructor?.DeclaringType ?? throw _panic;
            if (declType is null)
                throw new InvalidOperationException("MethodMetadata: DeclaringType is null.");
            _declType = new TypeMetadata(declType);
        }
        return _declType;
    }

    private static readonly /*InternalError*/Exception _panic = new("No method or constructor object set.");
}
