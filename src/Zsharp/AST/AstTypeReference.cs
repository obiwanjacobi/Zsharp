using System.Diagnostics;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    [DebuggerDisplay("{Identifier}")]
    public class AstTypeReference : AstType
    {
        public AstTypeReference(AstTypeReference inferredFrom)
        {
            Context = inferredFrom.Context;
            SetIdentifier(inferredFrom.Identifier!);
            TrySetSymbol(inferredFrom.Symbol!);
            _typeSource = inferredFrom.TypeSource;
        }

        protected AstTypeReference(Type_ref_useContext context)
        {
            Context = context;

            var typeRef = context.type_ref();
            IsOptional = typeRef.QUESTION() != null;
            IsError = typeRef.ERROR() != null;
        }

        protected AstTypeReference()
        { }

        public AstTypeDefinition? TypeDefinition => Symbol?.DefinitionAs<AstTypeDefinition>();

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

        public static AstTypeReference Create(AstTypeDefinition typeDef, AstNode? typeSource = null)
        {
            Ast.Guard(typeDef != null, "TypeDefinition is null.");
            Ast.Guard(typeDef!.Identifier != null, "TypeDefinition has no Identifier.");

            var typeRef = new AstTypeReference();
            typeRef.SetIdentifier(typeDef.Identifier!);
            typeRef.TrySetSymbol(typeDef.Symbol!);
            if (typeSource != null)
                typeRef.TrySetTypeSource(typeSource);
            return typeRef;
        }
    }
}