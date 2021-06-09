﻿using Antlr4.Runtime;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeReferenceType : AstTypeReferenceTemplate
    {
        public AstTypeReferenceType(Type_refContext context)
            : base(context)
        { }

        protected AstTypeReferenceType(ParserRuleContext? context = null)
            : base(context)
        { }

        protected AstTypeReferenceType(AstTypeReferenceType typeOrigin)
            : base(typeOrigin)
        { }

        public new AstTypeReferenceType? TypeOrigin
            => (AstTypeReferenceType?)base.TypeOrigin;

        public override AstTypeReferenceType MakeProxy()
        {
            return (TypeOrigin is not null)
                ? new AstTypeReferenceType(TypeOrigin)
                : new AstTypeReferenceType(this);
        }

        public static AstTypeReferenceType From(AstTypeDefinition typeDef)
        {
            Ast.Guard(typeDef is not null, "TypeDefinition is null.");
            Ast.Guard(typeDef!.Identifier is not null, "TypeDefinition has no Identifier.");

            var typeRef = new AstTypeReferenceType();
            typeRef.SetIdentifier(typeDef.Identifier!);
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