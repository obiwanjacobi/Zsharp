﻿using System;
using System.Collections.Immutable;
using Maja.Compiler.External.Metadata;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.External;

internal sealed record ExternalDeclaredTypeSymbol : DeclaredTypeSymbol
{
    private readonly Lazy<ImmutableArray<EnumSymbol>> _lazyEnums;
    private readonly Lazy<ImmutableArray<FieldSymbol>> _lazyFields;

    public ExternalDeclaredTypeSymbol(SymbolName name, IExternalTypeFactory factory, TypeMetadata typeMetadata)
        : base(name)
    {
        _lazyEnums = new Lazy<ImmutableArray<EnumSymbol>>(
            () => factory.GetEnums(typeMetadata).ToImmutableArray());
        _lazyFields = new Lazy<ImmutableArray<FieldSymbol>>(
            () => factory.GetFields(typeMetadata).ToImmutableArray());
    }

    public override ImmutableArray<EnumSymbol> Enums
        => _lazyEnums.Value;

    public override ImmutableArray<FieldSymbol> Fields
        => _lazyFields.Value;

    public override bool IsExternal
        => true;
}
