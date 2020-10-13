using static ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeReference : AstType
    {
        protected AstTypeReference(Type_ref_useContext ctx)
            : base(ctx.type_ref().type_name())
        {
            var typeRef = ctx.type_ref();
            IsOptional = typeRef.QUESTION() != null;
            IsError = typeRef.ERROR() != null;
        }

        public AstTypeReference(AstTypeReference inferredFrom)
            : base(inferredFrom.Context!)
        {
            var success = SetInferredFrom(inferredFrom);
            Ast.Guard(success, "InferredFrom Type could not be set.");

            IsOptional = inferredFrom.IsOptional;
            IsError = inferredFrom.IsError;
        }

        public AstTypeReference()
        { }

        private AstTypeDefinition? _typeDefinition;
        public AstTypeDefinition? TypeDefinition => _typeDefinition;

        public bool SetTypeDefinition(AstTypeDefinition typeDefinition)
        {
            if (Ast.SafeSet(ref _typeDefinition, typeDefinition))
            {
                // usually fails - just catches dangling definitions
                typeDefinition.SetParent(this);
                return true;
            }
            return false;
        }

        private AstTypeReference? _inferredFrom;
        public AstTypeReference? InferredFrom => _inferredFrom;

        public bool SetInferredFrom(AstTypeReference type)
        {
            if (Ast.SafeSet(ref _inferredFrom, type))
            {
                // can fail if referring to an existing type ref
                type.SetParent(this);
                return true;
            }
            return false;
        }

        private AstNode? _typeSource;
        public AstNode? TypeSource => _typeSource;

        public bool SetTypeSource(AstNode typeSource)
        {
            return Ast.SafeSet(ref _typeSource, typeSource);
        }

        public bool IsOptional { get; }

        public bool IsError { get; }

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

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitTypeReference(this);
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            if (Identifier != null)
                Identifier.Accept(visitor);
        }

        public static AstTypeReference Create(Type_ref_useContext ctx)
        {
            Ast.Guard(ctx, "AstTypeReference.Create is passed a null");
            var typeRef = new AstTypeReference(ctx);
            AstType.Construct(typeRef, ctx.type_ref().type_name());
            return typeRef;
        }

        public static AstTypeReference Create(AstTypeReference inferredFrom)
        {
            Ast.Guard(inferredFrom?.Identifier, "AstTypeReference.Create on AstTypeReference is passed a null");
            var typeRef = new AstTypeReference(inferredFrom!);
            var identifier = inferredFrom!.Identifier!.Clone();
            typeRef.SetIdentifier(identifier);
            bool success = typeRef.SetTypeDefinition(inferredFrom!.TypeDefinition!);
            Ast.Guard(success, "AstTypeReference.Create SetTypeDefinition (inferred) failed.");

            return typeRef;
        }

        public static AstTypeReference Create(AstNode typeSource, AstTypeDefinition typeDef)
        {
            Ast.Guard(typeDef != null, "AstTypeReference.Create is passed a null for the AstTypeDefinition.");
            Ast.Guard(typeDef!.Identifier != null, "AstTypeReference.Create is passed an AstTypeDefinition that has no Identifier.");

            var typeRef = new AstTypeReference();
            var identifier = typeDef!.Identifier!.Clone();

            bool success = typeRef.SetIdentifier(identifier);
            Ast.Guard(success, "AstTypeReference.Create SetIdentifier failed.");

            success = typeRef.SetTypeDefinition(typeDef);
            Ast.Guard(success, "AstTypeReference.Create SetTypeDefinition failed.");

            success = typeRef.SetTypeSource(typeSource);
            Ast.Guard(success, "AstTypeReference.Create SetTypeSource failed.");

            return typeRef;
        }
    }
}