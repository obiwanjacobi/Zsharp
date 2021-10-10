using System;
using System.Diagnostics;
using System.Reflection;

namespace Zsharp.External.Metadata
{
    [DebuggerDisplay("{Name}")]
    public class ParameterMetadata
    {
        private readonly ParameterInfo _parameter;

        public ParameterMetadata(ParameterInfo parameter)
        {
            _parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        public string Name => _parameter.Name ?? String.Empty;

        private TypeMetadata? _paramType;
        public TypeMetadata ParameterType
        {
            get
            {
                if (_paramType is null)
                {
                    _paramType = new TypeMetadata(_parameter.ParameterType);
                }
                return _paramType;
            }
        }
    }
}