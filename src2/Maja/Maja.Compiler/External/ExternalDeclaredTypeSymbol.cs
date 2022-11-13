using System.Collections.Generic;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.External;

internal sealed record ExternalDeclaredTypeSymbol : DeclaredTypeSymbol
{
    public ExternalDeclaredTypeSymbol(SymbolName name,
        IEnumerable<EnumSymbol> enums, IEnumerable<FieldSymbol> fields,
        IEnumerable<RuleSymbol> rules, int sizeInBytes)
        : base(name, enums, fields, rules, sizeInBytes)
    { }

    public override bool IsExternal
        => true;
}
