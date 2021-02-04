using System;
using System.Collections.Generic;

namespace Zsharp.AST
{
    public abstract class AstTypeDefinition : AstType, IAstTypeReferenceSite
    {
        private readonly Dictionary<string, AstTypeFieldDefinition> _fields = new Dictionary<string, AstTypeFieldDefinition>();

        protected AstTypeDefinition(AstIdentifier identifier)
            : base(identifier)
        { }

        protected AstTypeDefinition(AstNodeType nodeType)
            : base(nodeType)
        { }

        private AstTypeReference? _baseType;
        public AstTypeReference? BaseType => _baseType;

        public bool TrySetBaseType(AstTypeReference typeReference)
            => this.SafeSetParent(ref _baseType, typeReference);

        public void SetBaseType(AstTypeReference typeReference)
        {
            if (!TrySetBaseType(typeReference))
                throw new InvalidOperationException(
                    "Base Type Reference already set or null.");
        }

        AstTypeReference? IAstTypeReferenceSite.TypeReference => _baseType;

        bool IAstTypeReferenceSite.TrySetTypeReference(AstTypeReference typeReference)
            => TrySetBaseType(typeReference);

        void IAstTypeReferenceSite.SetTypeReference(AstTypeReference typeReference)
        {
            if (!TrySetBaseType(typeReference))
                throw new InvalidOperationException(
                    "Base Type Reference already set or null.");
        }

        public virtual bool IsIntrinsic => false;

        public virtual bool IsExternal => false;

        public virtual bool IsUnsigned => false;

        public virtual bool IsStruct => false;

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

        public void AddField(AstTypeFieldDefinition field)
        {
            if (!TryAddField(field))
                throw new InvalidOperationException(
                    "Type Field was already added or null.");
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