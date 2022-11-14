using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Maja.Compiler.External.Metadata;

[DebuggerDisplay("{Name}")]
internal sealed class TypeMetadata
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

    private TypeMetadata? _enumType;
    public TypeMetadata GetEnumType()
    {
        if (_enumType is null)
        {
            var field = _type.GetField("value__");
            if (field is null)
                throw new InvalidOperationException($"GetEnumType failed. Type {FullName} is not an Enum.");

            _enumType = new TypeMetadata(field.FieldType);
        }
        return _enumType;
    }

    private List<FieldMetadata> _fields = new();
    public IEnumerable<FieldMetadata> GetEnumFields()
    {
        if (_fields.Count == 0)
        {
            _fields.AddRange(_type.GetFields()
                .Where(fld => fld.IsPublic
                    && fld.Name != "value__")
                .Select(fld => FieldMetadata.FromEnum(fld))
                );
        }

        return _fields;
    }

    public IEnumerable<FieldMetadata> GetPublicFields()
    {
        if (_fields.Count == 0)
        {
            _fields.AddRange(_type.GetFields()
                .Where(fld => fld.IsPublic)
                .Select(fld => FieldMetadata.FromField(fld))
                );

            _fields.AddRange(_type.GetProperties()
                .Where(prop => prop.GetGetMethod()?.IsPublic ?? false)
                .Select(prop => FieldMetadata.FromProperty(prop))
                );
        }

        return _fields;
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

    private readonly List<TypeMetadata> _nestedTypes = new();
    public IEnumerable<TypeMetadata> GetNestedTypes()
    {
        if (_nestedTypes.Count == 0)
        {
            _nestedTypes.AddRange(_type.GetNestedTypes()
                .Where(t => t.IsPublic)
                .Select(t => new TypeMetadata(t))
            );
        }
        return _nestedTypes;
    }
}
