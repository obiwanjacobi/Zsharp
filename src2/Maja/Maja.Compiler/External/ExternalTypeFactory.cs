using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.External.Metadata;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.External;

internal interface IExternalTypeFactory
{
    IEnumerable<EnumSymbol> GetEnums(TypeMetadata type);
    IEnumerable<FieldSymbol> GetFields(TypeMetadata type);
}

internal class ExternalTypeFactory : IExternalTypeFactory
{
    private readonly Dictionary<string, TypeSymbol> _types = new();

    private static string BuildKey(TypeMetadata type)
        => type.FullName;

    public TypeSymbol Create(TypeMetadata type)
    {
        var key = BuildKey(type);

        if (_types.TryGetValue(key, out var symbol))
            return symbol;

        var majaType = MajaTypeMapper.MapToMajaType(type.Namespace, type.Name)
            ?? CreateDeclaredType(type);

        _types.Add(key, majaType);

        return majaType;
    }

    private ExternalDeclaredTypeSymbol CreateDeclaredType(TypeMetadata type)
    {
        var name = new SymbolName(type.Namespace, type.Name);
        return new ExternalDeclaredTypeSymbol(name, this, type);
    }

    public IEnumerable<EnumSymbol> GetEnums(TypeMetadata type)
    {
        if (!type.IsEnum)
            return Enumerable.Empty<EnumSymbol>();

        var pubFlds = type.GetEnumFields().Cast<FieldEnumMetadata>();
        return pubFlds.Select(e => CreateEnumSymbol(e, type));
    }

    public IEnumerable<FieldSymbol> GetFields(TypeMetadata type)
    {
        if (type.IsEnum)
            return Enumerable.Empty<FieldSymbol>();

        var pubFlds = type.GetFields();
        return pubFlds.Select(f => CreateFieldSymbol(f, type));
    }

    private EnumSymbol CreateEnumSymbol(FieldEnumMetadata enumMetadata, TypeMetadata typeMetadata)
    {
        var enumType = Create(typeMetadata.GetEnumType());
        var name = new SymbolName(enumMetadata.Name);
        return new EnumSymbol(name, enumType, enumMetadata.Value!);
    }

    private FieldSymbol CreateFieldSymbol(FieldMetadata fieldMetadata, TypeMetadata typeMetadata)
    {
        var name = new SymbolName(fieldMetadata.Name);

        var type = typeMetadata.FullName == fieldMetadata.FieldType.FullName
            ? new TypeSymbol(fieldMetadata.FieldType.Namespace, fieldMetadata.FieldType.Name)
            : Create(fieldMetadata.FieldType);

        return new FieldSymbol(name, type);
    }
}
