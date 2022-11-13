using System;
using System.Diagnostics;
using System.Reflection;

namespace Maja.Compiler.External.Metadata;

[DebuggerDisplay("{Name}")]
internal abstract class FieldMetadata
{
    protected FieldMetadata()
    { }

    public abstract bool IsReadOnly { get; }
    public abstract string Name { get; }
    public abstract string Namespace { get; }
    public abstract TypeMetadata FieldType { get; }

    public static FieldMetadata FromEnum(FieldInfo field)
        => new FieldEnumMetadata(field);
    public static FieldMetadata FromField(FieldInfo field)
        => new FieldFieldMetadata(field);
    public static FieldMetadata FromProperty(PropertyInfo property)
        => new FieldPropertyMetadata(property);
}

internal sealed class FieldEnumMetadata : FieldMetadata
{
    private readonly FieldInfo _field;

    public FieldEnumMetadata(FieldInfo field)
    {
        _field = field ?? throw new ArgumentNullException(nameof(field));
    }

    public override bool IsReadOnly
        => _field.IsInitOnly;
    public override string Name
        => _field.Name ?? String.Empty;
    public override string Namespace
        => _field.DeclaringType?.Namespace ?? String.Empty;

    private TypeMetadata? _fieldType;
    public override TypeMetadata FieldType
    {
        get
        {
            _fieldType ??= new TypeMetadata(_field.FieldType);
            return _fieldType;
        }
    }
}

internal sealed class FieldFieldMetadata : FieldMetadata
{
    private readonly FieldInfo _field;

    public FieldFieldMetadata(FieldInfo field)
    {
        _field = field ?? throw new ArgumentNullException(nameof(field));
    }

    public override bool IsReadOnly
        => _field.IsInitOnly;
    public override string Name
        => _field.Name ?? String.Empty;
    public override string Namespace
        => _field.DeclaringType?.Namespace ?? String.Empty;

    private TypeMetadata? _fieldType;
    public override TypeMetadata FieldType
    {
        get
        {
            _fieldType ??= new TypeMetadata(_field.FieldType);
            return _fieldType;
        }
    }
}

internal sealed class FieldPropertyMetadata : FieldMetadata
{
    private readonly PropertyInfo _property;

    public FieldPropertyMetadata(PropertyInfo property)
    {
        _property = property;
    }

    public override bool IsReadOnly
        => _property.GetSetMethod() is null;
    public override string Name
        => _property.Name;
    public override string Namespace
        => _property.DeclaringType?.Namespace ?? String.Empty;

    private TypeMetadata? _fieldType;
    public override TypeMetadata FieldType
    {
        get
        {
            _fieldType ??= new TypeMetadata(_property.PropertyType);
            return _fieldType;
        }
    }
}
