using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.IR;

internal abstract class IrTemplateRewriter : IrRewriter
{
    // TODO: support partial type-parameter replacement.

    protected Dictionary<SymbolName, TypeSymbol>? TypeMap { get; set; }

    [return: NotNullIfNotNull("type")]
    protected override IrType? RewriteType(IrType? type)
    {
        if (type is null) return null;

        if (TypeMap!.TryGetValue(type.Symbol.Name, out var newTypeSymbol))
        {
            return new IrType(type.Syntax, newTypeSymbol);
        }

        return base.RewriteType(type);
    }

    protected override IrTypeParameter RewriteTypeParameterTemplate(IrTypeParameterTemplate parameter)
    {
        if (TypeMap!.TryGetValue(parameter.Symbol.Name, out var newTypeSymbol))
        {
            return new IrTypeParameterTemplateResolved(parameter.Syntax, newTypeSymbol, parameter.Symbol);
        }

        return parameter;
    }

    protected override IrExpressionTypeInitializer RewriteTypeInitializer(IrExpressionTypeInitializer initializer)
    {
        if (TypeMap!.TryGetValue(initializer.TypeSymbol.Name, out var newTypeSymbol))
        {
            var fields = RewriteTypeInitializerFields(initializer.Fields);
            return new IrExpressionTypeInitializer(initializer.Syntax, newTypeSymbol, Enumerable.Empty<IrTypeArgument>(), fields);
        }

        return base.RewriteTypeInitializer(initializer);
    }

    protected override IrExpressionIdentifier RewriteExpressionIdentifier(IrExpressionIdentifier expression)
    {
        if (TypeMap!.TryGetValue(expression.TypeSymbol.Name, out var newTypeSymbol))
        {
            return new IrExpressionIdentifier(expression.Syntax, expression.Symbol, newTypeSymbol);
        }

        return expression;
    }

    protected static Dictionary<SymbolName, TypeSymbol> CreateTypeMap(ImmutableArray<IrTypeParameter> typeParameters, IEnumerable<IrTypeArgument> typeArguments)
    {
        return typeParameters.Zip(typeArguments)
            .ToDictionary(p => p.First.Symbol.Name, p => p.Second.Type.Symbol);
    }
}

internal sealed class IrTemplateFunctionRewriter : IrTemplateRewriter
{
    private readonly IrDeclarationFunction _template;

    public IrTemplateFunctionRewriter(IrDeclarationFunction template)
    {
        Debug.Assert(template.IsTemplate);
        _template = template;
    }

    public IrDeclarationFunction Rewrite(IEnumerable<IrTypeArgument> typeArguments)
    {
        TypeMap = CreateTypeMap(_template.TypeParameters, typeArguments);
        return RewriteDeclarationFunction(_template);
    }
}

internal sealed class IrTemplateTypeRewriter : IrTemplateRewriter
{
    private readonly IrDeclarationType _template;

    public IrTemplateTypeRewriter(IrDeclarationType template)
    {
        Debug.Assert(template.IsTemplate);
        _template = template;
    }

    public IrDeclarationType Rewrite(IEnumerable<IrTypeArgument> typeArguments)
    {
        TypeMap = CreateTypeMap(_template.TypeParameters, typeArguments);
        return RewriteDeclarationType(_template);
    }
}
