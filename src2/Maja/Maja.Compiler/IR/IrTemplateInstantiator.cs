using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.IR;

internal sealed class IrTemplateInstantiator
{
    public bool TryManifest(IrDeclarationFunction template, IEnumerable<IrTypeArgument> arguments, out IrDeclarationFunction function)
    {
        var success = true;

        // TODO: check for compile-time (non-type) parameters
        if (!template.TypeParameters.Any())
            throw new MajaException($"Can't call {nameof(IrTemplateInstantiator)}: function {template.Symbol.Name} that is not a template.");

        var functionRewriter = new IrTemplateFunctionRewriter(template);
        function = functionRewriter.Rewrite(arguments).Single();

        return success;
    }

    public bool TryManifest(IrDeclarationType template, IEnumerable<IrTypeArgument> arguments, out IrDeclarationType type)
    {
        var success = true;

        // TODO: check for compile-time (non-type) parameters
        if (!template.TypeParameters.Any())
            throw new MajaException($"Can't call {nameof(IrTemplateInstantiator)}: type {template.Symbol.Name} that is not a template.");

        var typeRewriter = new IrTemplateTypeRewriter(template);
        type = typeRewriter.Rewrite(arguments).Single();

        return success;
    }
}
