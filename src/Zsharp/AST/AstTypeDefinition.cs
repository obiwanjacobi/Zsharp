using System;
using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeDefinition : AstType
    {
        private readonly Dictionary<string, AstTypeFieldDefinition> _fields = new Dictionary<string, AstTypeFieldDefinition>();

        protected AstTypeDefinition(AstIdentifier identifier)
            : base(identifier)
        { }

        protected AstTypeDefinition(AstNodeType nodeType)
            : base(nodeType)
        { }

        public AstTypeReference? BaseType { get; internal set; }

        public virtual bool IsIntrinsic => false;

        public virtual bool IsExternal => false;

        public virtual bool IsUnsigned => false;

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

        public override void Accept(AstVisitor visitor) => visitor.VisitTypeDefinition(this);

        public static AstTypeDefinition? SelectKnownTypeDefinition(Known_typesContext context)
        {
            if (context == null)
                return null;
            //if (context.type_Bit()) return TypeBit;
            //if (context.type_Ptr()) return TypePtr;

            if (context.BOOL() != null)
                return AstTypeDefinitionIntrinsic.Bool;
            if (context.STR() != null)
                return AstTypeDefinitionIntrinsic.Str;
            if (context.F64() != null)
                return AstTypeDefinitionIntrinsic.F64;
            if (context.F32() != null)
                return AstTypeDefinitionIntrinsic.F32;
            if (context.I8() != null)
                return AstTypeDefinitionIntrinsic.I8;
            if (context.I16() != null)
                return AstTypeDefinitionIntrinsic.I16;
            if (context.I64() != null)
                return AstTypeDefinitionIntrinsic.I64;
            if (context.I32() != null)
                return AstTypeDefinitionIntrinsic.I32;
            if (context.U8() != null)
                return AstTypeDefinitionIntrinsic.U8;
            if (context.U16() != null)
                return AstTypeDefinitionIntrinsic.U16;
            if (context.U64() != null)
                return AstTypeDefinitionIntrinsic.U64;
            if (context.U32() != null)
                return AstTypeDefinitionIntrinsic.U32;

            return null;
        }
    }
}