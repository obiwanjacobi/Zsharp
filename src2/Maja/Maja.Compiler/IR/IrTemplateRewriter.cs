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

    protected override IEnumerable<IrDeclarationType> RewriteDeclarationType(IrDeclarationType type)
    {
        if (type.IsTemplate)
        {
            var typeParams = RewriteTypeParameters(type.TypeParameters);
            var enums = RewriteEnums(type.Enums);
            var fields = RewriteFields(type.Fields);
            var rules = RewriteRules(type.Rules);
            var types = RewriteType(type.BaseType);
            var baseType = types.SingleOrDefault();

            if (typeParams == type.TypeParameters &&
                enums == type.Enums &&
                fields == type.Fields &&
                rules == type.Rules &&
                baseType == type.BaseType)
                return [type];

            // TODO: needs deeper replacement for templates!
            var typeSymbol = new TypeTemplateSymbol(
                type.Symbol.Name,
                TypeMap!.Values,
                enums.Select(e => e.Symbol),
                fields.Select(f => f.Symbol),
                rules.Select(r => r.Symbol),
                baseType?.Symbol
            );

            return [new IrDeclarationType(type.Syntax, typeSymbol, typeParams, enums, fields, rules, baseType, type.Scope, type.Locality)];
        }

        return base.RewriteDeclarationType(type);
    }

    [return: NotNullIfNotNull("type")]
    protected override IEnumerable<IrType> RewriteType(IrType? type)
    {
        if (type is null) return [];

        if (TypeMap!.TryGetValue(type.Symbol.Name, out var newTypeSymbol))
        {
            return [new IrType(type.Syntax, newTypeSymbol)];
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

    protected override IrExpression RewriteExpressionTypeInitializer(IrExpressionTypeInitializer initializer)
    {
        if (TypeMap!.TryGetValue(initializer.TypeSymbol.Name, out var newTypeSymbol))
        {
            var fields = RewriteTypeInitializerFields(initializer.Fields);
            return new IrExpressionTypeInitializer(initializer.Syntax, newTypeSymbol, Enumerable.Empty<IrTypeArgument>(), fields);
        }

        return base.RewriteExpressionTypeInitializer(initializer);
    }

    protected override IrTypeInitializerField RewriteTypeInitializerField(IrTypeInitializerField initializer)
    {
        if (TypeMap!.TryGetValue(initializer.Field.Type.Name, out var newFieldTypeSymbol))
        {
            var newFieldSymbol = new FieldSymbol(initializer.Field.Name, newFieldTypeSymbol);
            return new(initializer.Syntax, newFieldSymbol, initializer.Expression);
        }

        return base.RewriteTypeInitializerField(initializer);
    }

    protected override IrTypeMemberField RewriteField(IrTypeMemberField memberField)
    {
        if (TypeMap!.TryGetValue(memberField.Type.Symbol.Name, out var newFieldTypeSymbol))
        {
            var types = RewriteType(memberField.Type);
            var newFieldType = types.Single();
            var newFieldSymbol = new FieldSymbol(memberField.Symbol.Name, newFieldTypeSymbol);
            return new IrTypeMemberField(memberField.Syntax, newFieldSymbol, newFieldType, memberField.DefaultValue);
        }

        return base.RewriteField(memberField);
    }

    protected override IEnumerable<IrParameter> RewriteParameter(IrParameter parameter)
    {
        if (TypeMap!.TryGetValue(parameter.Type.Symbol.Name, out var newParamTypeSymbol))
        {
            var types = RewriteType(parameter.Type);
            var newParamType = types.Single();
            var newParamSymbol = new ParameterSymbol(parameter.Symbol.Name, newParamTypeSymbol);
            return [new IrParameter(parameter.Syntax, newParamSymbol, newParamType)];
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

    public IEnumerable<IrDeclarationFunction> Rewrite(IEnumerable<IrTypeArgument> typeArguments)
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

    public IEnumerable<IrDeclarationType> Rewrite(IEnumerable<IrTypeArgument> typeArguments)
    {
        TypeMap = CreateTypeMap(_template.TypeParameters, typeArguments);
        return RewriteDeclarationType(_template);
    }
}
