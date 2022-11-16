using System;
using System.Diagnostics;
using System.Reflection;

namespace Maja.Compiler.External.Metadata;

[DebuggerDisplay("{Name}")]
internal sealed class ParameterMetadata
{
    private readonly ParameterInfo _parameter;

    public ParameterMetadata(ParameterInfo parameter)
    {
        _parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
    }

    public string Name
        => _parameter.Name ?? String.Empty;

    private TypeMetadata? _paramType;
    public TypeMetadata ParameterType
        => _paramType ??= new TypeMetadata(_parameter.ParameterType);
}