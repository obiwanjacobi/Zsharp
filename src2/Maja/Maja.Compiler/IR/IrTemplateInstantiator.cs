using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.IR;

internal sealed class IrTemplateInstantiator
{
    private readonly DiagnosticList _diagnostics = new();

    public bool Resolve(IrDeclarationFunction template, IEnumerable<IrTypeArgument> arguments, out IrDeclarationFunction function)
    {
        var success = true;

        // TODO: check for compile-time (non-type) parameters
        if (!template.TypeParameters.Any())
            throw new MajaException($"Can't call {nameof(IrTemplateInstantiator)} function {template.Symbol.Name} that is not a template.");

        var functionRewriter = new IrTemplateFunctionRewriter(template);
        function = functionRewriter.Rewrite(arguments);

        return success;
    }

    private static List<IrTemplateArgumentMatchInfo> CreateArgumentInfos(
        IEnumerable<TypeParameterSymbol> typeParameters, IEnumerable<IrTypeArgument> typeArguments)
    {
        // We do not support named (out-of-order) type arguments yet.
        return typeParameters
            .Zip(typeArguments)
            .Select(typeParamArg => new IrTemplateArgumentMatchInfo(typeParamArg.First, typeParamArg.Second))
            .ToList();
    }
}

internal sealed class IrTemplateArgumentMatchInfo
{
    public IrTemplateArgumentMatchInfo(TypeParameterSymbol parameter, IrTypeArgument argument)
    {
        TypeParameter = parameter;
        TypeArgument = argument;
    }

    public TypeParameterSymbol? TypeParameter { get; set; }
    public IrTypeArgument? TypeArgument { get; set; }
}