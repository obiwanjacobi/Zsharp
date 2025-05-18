using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.IR;

internal abstract class IrTemplateRewriter : IrRewriter
{
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

    protected override IrTypeMemberField RewriteField(IrTypeMemberField memberField)
    {
        if (TypeMap!.TryGetValue(memberField.Type.Symbol.Name, out var newFieldTypeSymbol))
        {
            var newFieldType = RewriteType(memberField.Type);
            var newFieldSymbol = new FieldSymbol(memberField.Symbol.Name, newFieldTypeSymbol);
            return new IrTypeMemberField(memberField.Syntax, newFieldSymbol, newFieldType, memberField.DefaultValue);
        }

        return base.RewriteField(memberField);
    }

    protected override IrParameter RewriteParameter(IrParameter parameter)
    {
        if (TypeMap!.TryGetValue(parameter.Type.Symbol.Name, out var newParamTypeSymbol))
        {
            var newParamType = RewriteType(parameter.Type);
            var newParamSymbol = new ParameterSymbol(parameter.Symbol.Name, newParamTypeSymbol);
            return new IrParameter(parameter.Syntax, newParamSymbol, newParamType);
        }

        return base.RewriteParameter(parameter);
    }

    protected override IrExpressionIdentifier RewriteExpressionIdentifier(IrExpressionIdentifier expression)
    {
        if (TypeMap!.TryGetValue(expression.TypeSymbol.Name, out var newTypeSymbol))
        {
            return new IrExpressionIdentifier(expression.Syntax, expression.Symbol, newTypeSymbol);
        }

        return expression;
    }

    // TODO: support partial type-parameter replacement.
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
