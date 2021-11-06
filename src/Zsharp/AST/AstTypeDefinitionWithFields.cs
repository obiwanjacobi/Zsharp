using System.Collections.Generic;

namespace Zsharp.AST
{
    public abstract class AstTypeDefinitionWithFields : AstTypeDefinitionTemplate,
        IAstTypeReferenceSite,
        IAstTypeFields<AstTypeFieldDefinition>
    {
        protected AstTypeDefinitionWithFields(AstNodeKind nodeKind)
            : base(nodeKind)
        { }

        public bool HasBaseType => _baseType is not null;

        private AstTypeReference? _baseType;
        public AstTypeReference BaseType
            => _baseType ?? throw new InternalErrorException("BaseType was not set.");

        public bool TrySetBaseType(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _baseType, typeReference);

        public void SetBaseType(AstTypeReference typeReference)
        {
            if (!TrySetBaseType(typeReference))
                throw new InternalErrorException(
                    "Base Type Reference already set or null.");
        }

        bool IAstTypeReferenceSite.HasTypeReference => HasBaseType;
        AstTypeReference IAstTypeReferenceSite.TypeReference => BaseType;

        bool IAstTypeReferenceSite.TrySetTypeReference(AstTypeReference? typeReference)
            => TrySetBaseType(typeReference);

        private readonly Dictionary<string, AstTypeFieldDefinition> _fields = new();
        public IEnumerable<AstTypeFieldDefinition> Fields => _fields.Values;

        public bool TryAddField(AstTypeFieldDefinition field)
        {
            if (field?.Identifier is null)
                return false;

            if (_fields.TryAdd(field.Identifier.SymbolName.CanonicalName.FullName, field))
            {
                return field.TrySetParent(this);
            }
            return false;
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            if (HasBaseType)
                BaseType.Accept(visitor);

            foreach (var option in Fields)
            {
                option.Accept(visitor);
            }
        }
    }
}
