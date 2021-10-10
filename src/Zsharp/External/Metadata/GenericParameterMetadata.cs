using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Zsharp.External.Metadata
{
    [DebuggerDisplay("{Name}")]
    public class GenericParameterMetadata
    {
        private readonly Type _genericType;

        public GenericParameterMetadata(Type genericType)
        {
            _genericType = genericType ?? throw new ArgumentNullException(nameof(genericType));
            if (!genericType.IsGenericParameter)
                throw new ArgumentException($"{genericType.Name} is not a generic parameter.");
        }

        public string Name => _genericType.Name;

        public bool IsCovariant => (_genericType.GenericParameterAttributes & GenericParameterAttributes.Covariant) > 0;
        public bool IsContravariant => (_genericType.GenericParameterAttributes & GenericParameterAttributes.Contravariant) > 0;
        public bool NeedDefaultCtor => (_genericType.GenericParameterAttributes & GenericParameterAttributes.DefaultConstructorConstraint) > 0;
        public bool NeedValueType => (_genericType.GenericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) > 0;
        public bool NeedClass => (_genericType.GenericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) > 0;

        private readonly List<TypeMetadata> _constraintTypes = new();
        public IEnumerable<TypeMetadata> GetConstraintTypes()
        {
            if (_constraintTypes.Count == 0)
            {
                _constraintTypes.AddRange(
                    _genericType.GetGenericParameterConstraints()
                    .Select(t => new TypeMetadata(t))
                );
            }
            return _constraintTypes;
        }
    }
}