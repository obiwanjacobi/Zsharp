using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Zsharp.AST;

namespace Zsharp.External.Metadata
{
    [DebuggerDisplay("{Name}")]
    public class MethodMetadata
    {
        private readonly MethodInfo? _method;
        private readonly ConstructorInfo? _constructor;

        public MethodMetadata(MethodInfo method)
        {
            _method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public MethodMetadata(ConstructorInfo constructor)
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        }

        public string Name
            => _method?.Name
            ?? _constructor?.Name
            ?? throw _panic;

        public bool IsStatic
            => _method?.IsStatic
            ?? _constructor?.IsStatic
            ?? throw _panic;

        private TypeMetadata? _retType;
        public TypeMetadata ReturnType
        {
            get
            {
                if (_retType is null)
                {
                    _retType = new TypeMetadata(
                        _method?.ReturnType ?? _constructor?.DeclaringType ?? throw _panic);
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
                    _params.AddRange(
                        (_method?.GetParameters() ?? _constructor?.GetParameters() ?? throw _panic)
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
                    _genTypes.AddRange(
                        (_method?.GetGenericArguments() ?? Enumerable.Empty<Type>())
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
                var declType = _method?.DeclaringType ?? _constructor?.DeclaringType ?? throw _panic;
                if (declType is null)
                    throw new InvalidOperationException("MethodMetadata: DeclaringType is null.");
                _declType = new TypeMetadata(declType);
            }
            return _declType;
        }

        private static readonly InternalErrorException _panic = new("No method or constructor object set.");
    }
}
