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

    private string BuildKey(TypeMetadata type)
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

    public IEnumerable<EnumSymbol> GetEnums(TypeMetadata type)
    {
        if (type.IsEnum)
        {
            var pubFlds = type.GetEnumFields().Cast<FieldEnumMetadata>();
            var enumType = Create(type.GetEnumType());
            return pubFlds.Select(e => CreateEnumSymbol(e, enumType));
        }

        return Enumerable.Empty<EnumSymbol>();
    }

    public IEnumerable<FieldSymbol> GetFields(TypeMetadata type)
    {
        if (!type.IsEnum)
        {
            var pubFlds = type.GetPublicFields();
            return pubFlds.Select(f => CreateFieldSymbol(f, type));
        }

        return Enumerable.Empty<FieldSymbol>();
    }

    private ExternalDeclaredTypeSymbol CreateDeclaredType(TypeMetadata type)
    {
        //var size = fieldSymbols.Sum(f => f.Type.SizeInBytes);
        var name = new SymbolName(type.Namespace, type.Name);
        return new ExternalDeclaredTypeSymbol(name, this, type);
    }

    private EnumSymbol CreateEnumSymbol(FieldEnumMetadata enumMetadata, TypeSymbol enumType)
    {
        var name = new SymbolName(enumMetadata.Namespace, enumMetadata.Name);
        return new EnumSymbol(name, enumType, enumMetadata.Value!);
    }

    private FieldSymbol CreateFieldSymbol(FieldMetadata fieldMetadata, TypeMetadata typeMetadata)
    {
        var name = new SymbolName(fieldMetadata.Namespace, fieldMetadata.Name);

        if (typeMetadata.FullName == fieldMetadata.FieldType.FullName)
        {
            var type = new TypeSymbol(fieldMetadata.FieldType.Namespace, fieldMetadata.FieldType.Name);
            return new FieldSymbol(name, type);
        }

        return new FieldSymbol(name, Create(fieldMetadata.FieldType));
    }
}
