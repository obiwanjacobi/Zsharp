using System.Collections.Generic;

namespace Zsharp.AST
{
    public abstract class AstTypeDefinitionWithFields : AstTypeDefinition,
        IAstTypeReferenceSite,
        IAstTypeFields<AstTypeFieldDefinition>
    {
        protected AstTypeDefinitionWithFields(AstNodeType nodeType)
            : base(nodeType)
        { }

        private AstTypeReference? _baseType;
        public AstTypeReference? BaseType => _baseType;

        public bool TrySetBaseType(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _baseType, typeReference);

        public void SetBaseType(AstTypeReference typeReference)
        {
            if (!TrySetBaseType(typeReference))
                throw new InternalErrorException(
                    "Base Type Reference already set or null.");
        }

        AstTypeReference? IAstTypeReferenceSite.TypeReference => _baseType;

        bool IAstTypeReferenceSite.TrySetTypeReference(AstTypeReference? typeReference)
            => TrySetBaseType(typeReference);

        private readonly Dictionary<string, AstTypeFieldDefinition> _fields = new();
        public IEnumerable<AstTypeFieldDefinition> Fields => _fields.Values;

        public bool TryAddField(AstTypeFieldDefinition field)
        {
            if (field?.Identifier == null)
                return false;

            if (_fields.TryAdd(field.Identifier.CanonicalName, field))
            {
                return field.TrySetParent(this);
            }
            return false;
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            BaseType?.Accept(visitor);
            foreach (var option in Fields)
            {
                option.Accept(visitor);
            }
        }
    }
}
