using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.Diagnostics;

namespace Maja.Compiler.IR;

internal sealed class IrTemplateInstantiator
{
    private readonly DiagnosticList _diagnostics = new();

    public bool Manifest(IrDeclarationFunction template, IEnumerable<IrTypeArgument> arguments, out IrDeclarationFunction function)
    {
        var success = true;

        // TODO: check for compile-time (non-type) parameters
        if (!template.TypeParameters.Any())
            throw new MajaException($"Can't call {nameof(IrTemplateInstantiator)}: function {template.Symbol.Name} that is not a template.");

        var functionRewriter = new IrTemplateFunctionRewriter(template);
        function = functionRewriter.Rewrite(arguments);

        return success;
    }

    public bool Manifest(IrDeclarationType template, IEnumerable<IrTypeArgument> arguments, out IrDeclarationType type)
    {
        var success = true;

        // TODO: check for compile-time (non-type) parameters
        if (!template.TypeParameters.Any())
            throw new MajaException($"Can't call {nameof(IrTemplateInstantiator)}: type {template.Symbol.Name} that is not a template.");

        var typeRewriter = new IrTemplateTypeRewriter(template);
        type = typeRewriter.Rewrite(arguments);

        return success;
    }
}
