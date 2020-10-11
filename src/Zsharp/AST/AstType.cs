using System;
using static ZsharpParser;

namespace Zsharp.AST
{
    public interface IAstTypeReferenceSite
    {
        AstTypeReference? TypeReference { get; }
        bool SetTypeReference(AstTypeReference typeRef);
    }

    // See AstIdentifier.cs
    public partial class AstIdentifierIntrinsic
    {
        public static readonly AstIdentifierIntrinsic U8 = new AstIdentifierIntrinsic("U8", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic U16 = new AstIdentifierIntrinsic("U16", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic U24 = new AstIdentifierIntrinsic("U24", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic U32 = new AstIdentifierIntrinsic("U32", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I8 = new AstIdentifierIntrinsic("I8", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I16 = new AstIdentifierIntrinsic("I16", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I24 = new AstIdentifierIntrinsic("I24", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I32 = new AstIdentifierIntrinsic("I32", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic F16 = new AstIdentifierIntrinsic("F16", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic F32 = new AstIdentifierIntrinsic("F32", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic Str = new AstIdentifierIntrinsic("Str", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic Bool = new AstIdentifierIntrinsic("Bool", AstIdentifierType.Type);
    }

    public class AstTypeDefinition : AstType
    {
        public AstTypeDefinition(Type_defContext ctx)
            : base(ctx.type_ref_use().type_ref().type_name())
        {
            Context = ctx;
        }
        protected AstTypeDefinition(AstIdentifier identifier)
            : base(identifier)
        { }

        public new Type_defContext? Context { get; }

        public AstTypeReference? BaseType { get; internal set; }

        public static AstTypeDefinition Create(Type_defContext ctx)
        {
            var typeDef = new AstTypeDefinition(ctx);
            typeDef.BaseType = AstTypeReference.Create(ctx.type_ref_use());

            var identifier = new AstIdentifier(ctx.identifier_type());
            typeDef.SetIdentifier(identifier);

            // TODO: type parameters: ctx.type_param_list()
            return typeDef;
        }
        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitTypeDefinition(this);
        }

        public static AstTypeDefinition? SelectKnownTypeDefinition(Known_typesContext ctx)
        {
            if (ctx == null) return null;
            //if (ctx.type_Bit()) return TypeBit;
            //if (ctx.type_Ptr()) return TypePtr;

            if (ctx.BOOL() != null)
                return  AstTypeIntrinsic.Bool;
            if (ctx.STR() != null)
                return AstTypeIntrinsic.Str;
            if (ctx.F16() != null)
                return AstTypeIntrinsic.F16;
            if (ctx.F32() != null)
                return AstTypeIntrinsic.F32;
            if (ctx.I8() != null)
                return AstTypeIntrinsic.I8;
            if (ctx.I16() != null)
                return AstTypeIntrinsic.I16;
            // if (ctx.I24() != null)
            //     return AstTypeIntrinsic.I24;
            if (ctx.I32() != null)
                return AstTypeIntrinsic.I32;
            if (ctx.U8() != null)
                return AstTypeIntrinsic.U8;
            if (ctx.U16() != null)
                return AstTypeIntrinsic.U16;
            // if (ctx.U24() != null)
            //     return AstTypeIntrinsic.U24;
            if (ctx.U32() != null)
                return AstTypeIntrinsic.U32;

            return null;
        }
    }



    public abstract class AstType : AstNode, IAstIdentifierSite
    {
        protected AstType()
            : base(AstNodeType.Type)
        { }
        protected AstType(Type_nameContext ctx)
            : base(AstNodeType.Type)
        {
            Context = ctx;
        }
        protected AstType(AstIdentifier identifier)
            : base(AstNodeType.Type)
        {
            SetIdentifier(identifier);
        }

        public Type_nameContext? Context { get; }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;
        public bool SetIdentifier(AstIdentifier identifier)
        {
            return this.SafeSetParent(ref _identifier, identifier);
        }

        public virtual bool IsEqual(AstType type)
        {
            if (type == null) return false;
            if (Identifier == null) return false;
            if (type.Identifier == null) return false;

            return Identifier.IsEqual(type.Identifier);
        }

        public static void Construct(AstType instance, Type_nameContext ctx)
        {
            AstIdentifier? identifier;

            var idCtx = ctx.identifier_type();
            if (idCtx != null)
            {
                identifier = new AstIdentifier(idCtx);
                // TODO: type parameters ctx.type_param_list()
            }
            else
            {
                var knownCtx = ctx.known_types();
                identifier = SelectKnownIdentifier(knownCtx);
            }

            Ast.Guard(identifier, "Identifier failed.");
            bool success = instance.SetIdentifier(identifier!);
            Ast.Guard(success, "SetIdentifier() failed");
        }

        private static AstIdentifier? SelectKnownIdentifier(Known_typesContext ctx)
        {
            if (ctx == null) return null;
            //if (ctx.type_Bit()) return Bit;
            //if (ctx.type_Ptr()) return Ptr;

            AstIdentifier? identifier = null;

            if (ctx.BOOL() != null)
                identifier = AstIdentifierIntrinsic.Bool;
            if (ctx.STR() != null)
                identifier = AstIdentifierIntrinsic.Str;
            if (ctx.F16() != null)
                identifier = AstIdentifierIntrinsic.F16;
            if (ctx.F32() != null)
                identifier = AstIdentifierIntrinsic.F32;
            if (ctx.I8() != null)
                identifier = AstIdentifierIntrinsic.I8;
            if (ctx.I16() != null)
                identifier = AstIdentifierIntrinsic.I16;
            // if (ctx.I24() != null)
            //     identifier = AstIdentifierIntrinsic.I24;
            if (ctx.I32() != null)
                identifier = AstIdentifierIntrinsic.I32;
            if (ctx.U8() != null)
                identifier = AstIdentifierIntrinsic.U8;
            if (ctx.U16() != null)
                identifier = AstIdentifierIntrinsic.U16;
            // if (ctx.U24() != null)
            //     identifier = AstIdentifierIntrinsic.U24;
            if (ctx.U32() != null)
                identifier = AstIdentifierIntrinsic.U32;

            if (identifier != null)
            {
                return identifier.Clone();
            }

            return null;
        }
    }



    public class AstTypeReference : AstType
    {
        protected AstTypeReference(Type_ref_useContext ctx)
            : base(ctx.type_ref().type_name())
        { }

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


        public override bool IsEqual(AstType that)
        {
            if (!base.IsEqual(that)) return false;

            var typedThat = that as AstTypeReference;
            if (typedThat == null) return false;

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

            success = typeRef.SetTypeSource((AstNode)typeSource);
            Ast.Guard(success, "AstTypeReference.Create SetTypeSource failed.");

            return typeRef;
        }
    }

    public class AstTypeIntrinsic : AstTypeDefinition
    {
        public AstTypeIntrinsic(AstIdentifier identifier)
            : base(identifier)
        {}

        public static readonly AstTypeIntrinsic U8 = new AstTypeIntrinsic(AstIdentifierIntrinsic.U8);
        public static readonly AstTypeIntrinsic U16 = new AstTypeIntrinsic(AstIdentifierIntrinsic.U16);
        public static readonly AstTypeIntrinsic U24 = new AstTypeIntrinsic(AstIdentifierIntrinsic.U24);
        public static readonly AstTypeIntrinsic U32 = new AstTypeIntrinsic(AstIdentifierIntrinsic.U32);
        public static readonly AstTypeIntrinsic I8 = new AstTypeIntrinsic(AstIdentifierIntrinsic.I8);
        public static readonly AstTypeIntrinsic I16 = new AstTypeIntrinsic(AstIdentifierIntrinsic.I16);
        public static readonly AstTypeIntrinsic I24 = new AstTypeIntrinsic(AstIdentifierIntrinsic.I24);
        public static readonly AstTypeIntrinsic I32 = new AstTypeIntrinsic(AstIdentifierIntrinsic.I32);
        public static readonly AstTypeIntrinsic F16 = new AstTypeIntrinsic(AstIdentifierIntrinsic.F16);
        public static readonly AstTypeIntrinsic F32 = new AstTypeIntrinsic(AstIdentifierIntrinsic.F32);
        public static readonly AstTypeIntrinsic Str = new AstTypeIntrinsic(AstIdentifierIntrinsic.Str);
        public static readonly AstTypeIntrinsic Bool = new AstTypeIntrinsic(AstIdentifierIntrinsic.Bool);

        private static void AddIntrinsicSymbol(AstSymbolTable symbols, AstTypeIntrinsic type)
        {
            if (type?.Identifier == null) throw new ArgumentNullException(nameof(type));
            symbols.AddSymbol(type.Identifier.Name, AstSymbolKind.Type, type);
        }

        public static void AddAll(AstSymbolTable symbols)
        {
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.Bool);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.F16);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.F32);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.I16);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.I24);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.I32);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.I8);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.Str);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.U16);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.U24);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.U32);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.U8);
        }
    }
}