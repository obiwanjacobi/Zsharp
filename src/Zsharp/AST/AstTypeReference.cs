using System;
using System.Diagnostics;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    [DebuggerDisplay("{Identifier}")]
    public class AstTypeReference : AstType
    {
        protected AstTypeReference(Type_ref_useContext context)
            : base(context.type_ref().type_name())
        {
            var typeRef = context.type_ref();
            IsOptional = typeRef.QUESTION() != null;
            IsError = typeRef.ERROR() != null;
        }

        public AstTypeReference(AstTypeReference inferredFrom)
            : base(inferredFrom.Context!)
        {
            SetIdentifier(inferredFrom.Identifier!);
            SetTypeDefinition(inferredFrom.TypeDefinition!);
            _typeSource = inferredFrom.TypeSource;
        }

        protected AstTypeReference()
        { }

        private AstTypeDefinition? _typeDefinition;
        public AstTypeDefinition? TypeDefinition => _typeDefinition;

        public bool TrySetTypeDefinition(AstTypeDefinition typeDefinition)
        {
            if (Ast.SafeSet(ref _typeDefinition, typeDefinition))
            {
                // usually fails - just catches dangling definitions
                typeDefinition.TrySetParent(this);
                return true;
            }
            return false;
        }

        public void SetTypeDefinition(AstTypeDefinition typeDefinition)
        {
            if (!TrySetTypeDefinition(typeDefinition))
                throw new InvalidOperationException(
                    "TypeDefinition is already set or null.");
        }

        private AstNode? _typeSource;
        /// <summary>
        /// A reference to a source node that helped determine the type.
        /// </summary>
        public AstNode? TypeSource => _typeSource;

        public bool TrySetTypeSource(AstNode typeSource) => Ast.SafeSet(ref _typeSource, typeSource);

        public bool IsOptional { get; protected set; }

        public bool IsError { get; protected set; }

        public override bool IsEqual(AstType type)
        {
            if (!base.IsEqual(type))
                return false;

            if (!(type is AstTypeReference typedThat))
                return false;

            var typeDef = TypeDefinition;
            var thatTypeDef = typedThat.TypeDefinition;

            if (typeDef != null &&
                thatTypeDef != null)
            {
                return
                    typeDef.IsEqual(thatTypeDef) &&
                    IsError == typedThat.IsError &&
                    IsOptional == typedThat.IsOptional;
            }
            return false;
        }

        public override void Accept(AstVisitor visitor) => visitor.VisitTypeReference(this);

        public static AstTypeReference Create(Type_ref_useContext context)
        {
            Ast.Guard(context, "AstTypeReference.Create is passed a null");
            var typeRef = new AstTypeReference(context);
            AstType.Construct(typeRef, context.type_ref().type_name());
            return typeRef;
        }

        public static AstTypeReference Create(AstNode typeSource, AstTypeDefinition typeDef)
        {
            Ast.Guard(typeDef != null, "TypeDefinition is null.");
            Ast.Guard(typeDef!.Identifier != null, "TypeDefinition has no Identifier.");

            var typeRef = new AstTypeReference();
            typeRef.SetIdentifier(typeDef.Identifier!);
            typeRef.SetTypeDefinition(typeDef);
            typeRef.TrySetTypeSource(typeSource);
            return typeRef;
        }
    }
}