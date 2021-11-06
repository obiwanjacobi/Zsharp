using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zsharp.AST
{
    public class AstTypeReferenceFunction : AstTypeReference,
        IAstTypeReferenceSite,
        IAstFunctionParameters<AstFunctionParameterReference>
    {
        public AstTypeReferenceFunction(ParserRuleContext context)
            : base(context)
        {
            this.SetIdentifier(new AstIdentifier(String.Empty, AstIdentifierKind.Type));
        }

        private AstTypeReferenceFunction(AstTypeReferenceFunction typeOrigin)
            : base(typeOrigin)
        { }

        private readonly List<AstFunctionParameterReference> _parameters = new();
        public IEnumerable<AstFunctionParameterReference> Parameters => _parameters;

        public bool TryAddParameter(AstFunctionParameterReference param)
        {
            if (param is not null &&
                param.TrySetParent(this))
            {
                // always make sure 'self' is first param
                if (param.HasIdentifier && param.Identifier == AstIdentifierIntrinsic.Self)
                    _parameters.Insert(0, param);
                else
                    _parameters.Add(param);
                return true;
            }
            return false;
        }

        public string OverloadKey =>
            String.Join(String.Empty, _parameters
                .Where(p => p.HasTypeReference)
                .Select(p => p.TypeReference.Identifier.SymbolName.CanonicalName.FullName));

        public override AstTypeReferenceFunction MakeCopy()
        {
            var typeRef = new AstTypeReferenceFunction(this);
            Symbol?.AddNode(typeRef);
            return typeRef;
        }

        public bool HasTypeReference => _typeReference is not null;

        private AstTypeReference? _typeReference;
        public AstTypeReference TypeReference
            => _typeReference ?? throw new InternalErrorException("TypeReference was not set.");

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeReference, typeReference);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeReferenceFunction(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var param in Parameters)
            {
                param.Accept(visitor);
            }

            if (HasTypeReference)
                TypeReference.Accept(visitor);
        }

        public override string ToString()
        {
            var txt = new StringBuilder();

            txt.Append('(');
            for (int i = 0; i < Parameters.Count(); i++)
            {
                if (i > 0)
                    txt.Append(", ");

                var p = Parameters.ElementAt(i);
                if (p.HasTypeReference)
                    txt.Append(p.TypeReference.Identifier.NativeFullName);
            }
            txt.Append(')');

            if (HasTypeReference)
            {
                txt.Append(": ");
                txt.Append(TypeReference.Identifier.NativeFullName);
            }

            return txt.ToString();
        }

        public void CreateSymbols(AstSymbolTable functionSymbols, AstSymbolTable? parentSymbols = null)
        {
            Ast.Guard(!HasSymbol, "Symbol already set. Call CreateSymbols only once.");
            var contextSymbols = parentSymbols ?? functionSymbols;

            if (HasTypeReference)
                contextSymbols.TryAdd(TypeReference);

            foreach (var parameter in Parameters)
            {
                if (parameter.HasTypeReference)
                    functionSymbols.TryAdd(parameter.TypeReference);
            }

            var symbolName = AstSymbolName.Parse(ToString());
            Identifier.SymbolName = symbolName;

            contextSymbols.Add(this);
        }

        public AstTypeReference? ReplaceTypeReference(AstTypeReference typeReference)
        {
            var oldTypeRef = _typeReference;
            _typeReference = typeReference;
            return oldTypeRef;
        }
    }
}
