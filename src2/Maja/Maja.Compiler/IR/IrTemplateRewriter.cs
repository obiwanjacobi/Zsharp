using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.IR;

internal abstract class IrTemplateRewriter : IrRewriter
{
    // TODO: support partial type-parameter replacement.

    protected Dictionary<TypeSymbol, TypeSymbol>? TypeMap { get; set; }

    [return: NotNullIfNotNull("type")]
    protected override IrType? RewriteType(IrType? type)
    {
        if (type is null) return null;

        if (TypeMap!.TryGetValue(type.Symbol, out var newTypeSymbol))
        {
            return new IrType(type.Syntax, newTypeSymbol);
        }

        return base.RewriteType(type);
    }

    protected override IrExpressionTypeInitializer RewriteTypeInitializer(IrExpressionTypeInitializer initializer)
    {
        if (TypeMap!.TryGetValue(initializer.TypeSymbol, out var newTypeSymbol))
        {
            var fields = RewriteTypeInitializerFields(initializer.Fields);
            return new IrExpressionTypeInitializer(initializer.Syntax, newTypeSymbol, Enumerable.Empty<IrTypeArgument>(), fields);
        }

        return base.RewriteTypeInitializer(initializer);
    }
}

internal sealed class IrTemplateFunctionRewriter : IrTemplateRewriter
{
    private readonly IrDeclarationFunction _template;


    public IrTemplateFunctionRewriter(IrDeclarationFunction template)
    {
        _template = template;
    }

    public IrDeclarationFunction Rewrite(IEnumerable<IrTypeArgument> typeArguments)
    {
        TypeMap = CreateTypeMap(_template.TypeParameters, typeArguments);
        return RewriteDeclarationFunction(_template);
    }

    private static Dictionary<TypeSymbol, TypeSymbol> CreateTypeMap(ImmutableArray<IrTypeParameter> typeParameters, IEnumerable<IrTypeArgument> typeArguments)
    {
        return typeParameters.Zip(typeArguments)
            .ToDictionary(p => (TypeSymbol)p.First.Symbol, p => p.Second.Type.Symbol);
    }
}

internal sealed class IrTemplateTypeRewriter : IrTemplateRewriter
{
}
