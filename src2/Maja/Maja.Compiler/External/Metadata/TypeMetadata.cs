using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Maja.External.Metadata;

[DebuggerDisplay("{Name}")]
public class TypeMetadata
{
    private readonly Type _type;

    public TypeMetadata(Type type)
    {
        _type = type ?? throw new ArgumentNullException(nameof(type));
    }

    public string Namespace
        => _type.Namespace ?? String.Empty;
    public string Name
        => _type.Name;
    public string FullName
        => _type.FullName ?? String.Empty;

    public bool IsArray
        => _type.IsArray;
    public bool IsEnum
        => _type.IsEnum;
    public bool IsValueType
        => _type.IsValueType;
    public bool IsClass
        => _type.IsClass;
    public bool IsInterface
        => _type.IsInterface;

    public bool ContainsGenericParameter
        => _type.ContainsGenericParameters;

    private readonly List<GenericParameterMetadata> _genTypes = new();
    public IEnumerable<GenericParameterMetadata> GenericParameters
    {
        get
        {
            if (_genTypes.Count == 0)
            {
                _genTypes.AddRange(_type.GetGenericArguments()
                    .Select(g => new GenericParameterMetadata(g)));
            }
            return _genTypes;
        }
    }

    public bool IsSealed
        => _type.IsSealed;
    public bool IsAbstract
        => _type.IsAbstract;

    public bool HasBaseType
        => _type.BaseType is not null;

    public TypeMetadata GetBaseType()
    {
        var baseType = _type.BaseType
            ?? throw new InvalidOperationException("TypeMetadata: BaseType is null.");

        return new TypeMetadata(baseType);
    }

    public bool HasElementType
        => _type.GetElementType() is not null;

    public TypeMetadata GetElementType()
    {
        var elemType = _type.GetElementType()
            ?? throw new InvalidOperationException("TypeMetadata: Element Type is null.");

        return new TypeMetadata(elemType);
    }

    private readonly List<MethodMetadata> _methods = new();
    public IEnumerable<MethodMetadata> GetPublicMethods()
    {
        if (_methods.Count == 0)
        {
            _methods.AddRange(_type.GetMethods()
                .Where(m => m.IsPublic)
                .Select(m => new MethodMetadata(m))
                .Concat(
                    _type.GetConstructors()
                    .Where(c => c.IsPublic)
                    .Select(c => new MethodMetadata(c))
                ));
        }
        return _methods;
    }

    public bool HasConstructors
        => _type.GetConstructors().Any(c => c.IsPublic);
}
