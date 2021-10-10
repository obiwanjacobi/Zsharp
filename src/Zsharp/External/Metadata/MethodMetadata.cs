using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Zsharp.External.Metadata
{
    [DebuggerDisplay("{Name}")]
    public class MethodMetadata
    {
        private readonly MethodInfo _method;

        public MethodMetadata(MethodInfo method)
        {
            _method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public string Name => _method.Name;

        public bool IsStatic => _method.IsStatic;

        private TypeMetadata? _retType;
        public TypeMetadata ReturnType
        {
            get
            {
                if (_retType is null)
                {
                    _retType = new TypeMetadata(_method.ReturnType);
                }
                return _retType;
            }
        }

        private readonly List<ParameterMetadata> _params = new();
        public IEnumerable<ParameterMetadata> Parameters
        {
            get
            {
                if (_params.Count == 0)
                {
                    _params.AddRange(_method.GetParameters()
                        .Select(p => new ParameterMetadata(p)));
                }
                return _params;
            }
        }

        private readonly List<GenericParameterMetadata> _genTypes = new();
        public IEnumerable<GenericParameterMetadata> GenericParameters
        {
            get
            {
                if (_genTypes.Count == 0)
                {
                    _genTypes.AddRange(_method.GetGenericArguments()
                        .Select(g => new GenericParameterMetadata(g)));
                }
                return _genTypes;
            }
        }

        private TypeMetadata? _declType;
        public TypeMetadata GetDeclaringType()
        {
            if (_declType is null)
            {
                var declType = _method.DeclaringType;
                if (declType is null)
                    throw new InvalidOperationException("MethodMetadata: DeclaringType is null.");
                _declType = new TypeMetadata(declType);
            }
            return _declType;
        }
    }
}
