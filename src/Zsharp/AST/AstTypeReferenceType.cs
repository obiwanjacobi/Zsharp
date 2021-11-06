using Antlr4.Runtime;
using System;

namespace Zsharp.AST
{
    public class AstTypeReferenceType : AstTypeReferenceTemplate
    {
        public AstTypeReferenceType(AstIdentifier identifier)
        {
            this.SetIdentifier(identifier);
        }

        internal AstTypeReferenceType(ParserRuleContext? context = null)
            : base(context)
        { }

        protected AstTypeReferenceType(AstTypeReferenceType typeToCopy)
            : base(typeToCopy)
        { }

        public override AstTypeReferenceType MakeCopy()
        {
            var typeRef = new AstTypeReferenceType(this);
            if (HasSymbol)
                Symbol.AddNode(typeRef);
            return typeRef;
        }

        public static AstTypeReferenceType From(AstTypeDefinition typeDef)
        {
            Ast.Guard(typeDef is not null, "TypeDefinition is null.");
            Ast.Guard(typeDef!.HasIdentifier, "TypeDefinition has no Identifier.");

            var typeRef = new AstTypeReferenceType();
            typeRef.SetIdentifier(typeDef.Identifier);
            if (typeDef.Symbol is not null)
            {
                typeRef.TrySetSymbol(typeDef.Symbol);
                typeDef.Symbol.AddNode(typeRef);
            }
            return typeRef;
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeReferenceType(this);
    }
}