using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.External.Metadata;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.External;

internal class ExternalTypeFactory
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

    private ExternalDeclaredTypeSymbol CreateDeclaredType(TypeMetadata type)
    {
        var pubFlds = type.GetPublicFields();
        var enums = pubFlds.OfType<FieldEnumMetadata>();
        var fields = pubFlds.OfType<FieldFieldMetadata>();
        var props = pubFlds.OfType<FieldPropertyMetadata>();
        // rules (null reference?)

        var enumSymbols = enums.Select(e => CreateEnumSymbol(e));
        var fieldSymbols = fields.Select(f => CreateFieldSymbol(f))
            .Concat(props.Select(p => CreateFieldSymbol(p)));
        var ruleSymbols = Enumerable.Empty<RuleSymbol>();

        var size = fieldSymbols.Sum(f => f.Type.SizeInBytes);
        var name = new SymbolName(type.Namespace, type.Name);
        return new ExternalDeclaredTypeSymbol(name, enumSymbols, fieldSymbols, ruleSymbols, size);
    }

    private EnumSymbol CreateEnumSymbol(FieldEnumMetadata enumMetadata)
    {
        var name = new SymbolName(enumMetadata.Namespace, enumMetadata.Name);
        //return new EnumSymbol(name, Create(enumMetadata.FieldType));
        var type = new TypeSymbol(enumMetadata.FieldType.Namespace, enumMetadata.FieldType.Name);
        return new EnumSymbol(name, type);
    }

    private FieldSymbol CreateFieldSymbol(FieldFieldMetadata fieldMetadata)
    {
        var name = new SymbolName(fieldMetadata.Namespace, fieldMetadata.Name);
        //return new FieldSymbol(name, Create(fieldMetadata.FieldType));
        var type = new TypeSymbol(fieldMetadata.FieldType.Namespace, fieldMetadata.FieldType.Name);
        return new FieldSymbol(name, type);
    }

    private FieldSymbol CreateFieldSymbol(FieldPropertyMetadata propertyMetadata)
    {
        var name = new SymbolName(propertyMetadata.Namespace, propertyMetadata.Name);
        //return new FieldSymbol(name, Create(propertyMetadata.FieldType));
        var type = new TypeSymbol(propertyMetadata.FieldType.Namespace, propertyMetadata.FieldType.Name);
        return new FieldSymbol(name, type);
    }
}
