using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Maja.Compiler.External.Metadata;

[DebuggerDisplay("{Name}")]
internal sealed class TypeMetadata
{
    private readonly AssemblyMetadata? _assembly;
    private readonly Type _type;

    public TypeMetadata(Type type, AssemblyMetadata? assembly = null)
    {
        _assembly = assembly;
        _type = type ?? throw new ArgumentNullException(nameof(type));
    }

    public AssemblyMetadata Assembly => _assembly
        ?? throw new InvalidOperationException("No Assembly was set for this Type.");

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

    private List<GenericParameterMetadata>? _genTypes;
    public IEnumerable<GenericParameterMetadata> GetGenericParameters()
        => _genTypes ??= new(_type.GetGenericArguments()
            .Select(g => new GenericParameterMetadata(g)));

    public bool IsSealed
        => _type.IsSealed;
    public bool IsAbstract
        => _type.IsAbstract;

    public bool HasDeclaringType
        => _type.DeclaringType is not null;

    private TypeMetadata? _declaringType;
    public TypeMetadata GetDeclaringType()
        => _declaringType ??= new TypeMetadata(_type.DeclaringType
            ?? throw new InvalidOperationException("TypeMetadata: DeclaringType is null."), _assembly);

    public bool HasBaseType
        => _type.BaseType is not null;

    private TypeMetadata? _baseType;
    public TypeMetadata GetBaseType()
        => _baseType ??= new TypeMetadata(_type.BaseType
            ?? throw new InvalidOperationException("TypeMetadata: BaseType is null."), _assembly);

    public bool HasElementType
        => _type.GetElementType() is not null;

    private TypeMetadata? _elemType;
    public TypeMetadata GetElementType()
        => _elemType ??= new TypeMetadata(_type.GetElementType()
            ?? throw new InvalidOperationException("TypeMetadata: Element Type is null."), _assembly);

    private TypeMetadata? _enumType;
    public TypeMetadata GetEnumType()
    {
        if (_enumType is null)
        {
            var field = _type.GetField("value__");
            if (field is null)
                throw new InvalidOperationException($"GetEnumType failed. Type {FullName} is not an Enum.");

            _enumType = new TypeMetadata(field.FieldType, _assembly);
        }
        return _enumType;
    }

    private List<FieldMetadata>? _fields;
    public IEnumerable<FieldMetadata> GetEnumFields()
    {
        _fields ??= new(_type.GetFields()
                .Where(fld => fld.IsPublic
                    && fld.Name != "value__")
                .Select(fld => FieldMetadata.FromEnum(fld))
            );

        return _fields;
    }

    public IEnumerable<FieldMetadata> GetFields()
    {
        if (_fields is null)
        {
            _fields = new(_type.GetFields()
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

    private List<FunctionMetadata>? _methods;
    public IEnumerable<FunctionMetadata> GetFunctions()
    {
        _methods ??= new(_type.GetMethods()
            .Where(m => m.IsPublic)
            .Select(m => new FunctionMetadata(m, this))
            .Concat(
                _type.GetConstructors()
                .Where(c => c.IsPublic)
                .Select(c => new FunctionMetadata(c, this))
        ));

        return _methods;
    }

    public bool HasConstructors
        => _type.GetConstructors().Any(c => c.IsPublic);

    private List<TypeMetadata>? _nestedTypes;
    public IEnumerable<TypeMetadata> GetNestedTypes()
    {
        _nestedTypes ??= new(_type.GetNestedTypes()
                .Where(t => t.IsPublic)
                .Select(t => new TypeMetadata(t))
        );

        return _nestedTypes;
    }

    private List<OperatorFunctionMetadata>? _operatorFunctions;
    public IEnumerable<OperatorFunctionMetadata> GetOperatorFunctions()
    {
        if (_operatorFunctions is null)
        {
            _operatorFunctions = new();
            foreach (var method in _type.GetMethods().Where(m => m.IsPublic))
            {
                var attrs = method.GetCustomAttributes(typeof(OperatorAttribute), true);

                if (attrs.Length > 0)
                {
                    var opFn = new OperatorFunctionMetadata(method, (OperatorAttribute)attrs[0], this);
                    _operatorFunctions.Add(opFn);
                }
            }
        }

        return _operatorFunctions;
    }

}
