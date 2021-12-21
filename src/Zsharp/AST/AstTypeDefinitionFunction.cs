using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstTypeDefinitionFunction : AstTypeDefinition,
        IAstTypeReferenceSite
    {
        internal AstTypeDefinitionFunction()
        {
            this.SetIdentifier(new AstIdentifier(String.Empty, AstIdentifierKind.Type));
        }

        public AstTypeDefinitionFunction(ParserRuleContext context)
            : this()
        {
            Context = context;
        }

        private readonly List<AstTypeReference> _parameterTypes = new();
        public IEnumerable<AstTypeReference> ParameterTypes => _parameterTypes;

        public bool TryAddParameterType(AstTypeReference typeReference)
        {
            if (typeReference is not null &&
                typeReference.TrySetParent(this))
            {
                _parameterTypes.Add(typeReference!);
                return true;
            }
            return false;
        }

        public void AddParameterType(AstTypeReference typeReference)
        {
            if (!TryAddParameterType(typeReference))
                throw new InternalErrorException(
                    "Parameter TypeReference was already set (parent) or null.");
        }

        public string OverloadKey =>
            String.Join(String.Empty, _parameterTypes
                .Select(t => t.Identifier.SymbolName.CanonicalName.FullName));

        public bool HasTypeReference => _typeReference is not null;

        private AstTypeReference? _typeReference;
        public AstTypeReference TypeReference
            => _typeReference ?? throw new InternalErrorException("TypeReference is not set.");

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeReference, typeReference);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeDefinitionFunction(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var param in _parameterTypes)
            {
                param.Accept(visitor);
            }

            if (HasTypeReference)
                TypeReference.Accept(visitor);
        }

        public void CreateSymbols(AstSymbolTable? functionSymbols, AstSymbolTable? parentSymbols)
        {
            Ast.Guard(functionSymbols is not null || parentSymbols is not null, "One of the SymbolTable parameters must be specified.");
            Ast.Guard(!HasSymbol, "Symbol already set. Call CreateSymbols only once.");
            var contextSymbols = parentSymbols ?? functionSymbols;

            var name = new StringBuilder();
            foreach (var parameterType in _parameterTypes)
            {
                if (name.Length > 0)
                    name.Append(',');

                if (!parameterType.HasSymbol)
                    contextSymbols?.TryAdd(parameterType);
                name.Append(parameterType.Identifier.CanonicalFullName);
            }

            name.Insert(0, '(');

            if (HasTypeReference)
            {
                contextSymbols?.TryAdd(TypeReference);

                name.Append("): ")
                    .Append(TypeReference.Identifier.CanonicalFullName);
            }
            else
                name.Append(')');

            var canonical = AstName.CreateUnparsed(name.ToString(), AstNameKind.Canonical);
            Identifier.SymbolName = new AstSymbolName(canonical);

            contextSymbols?.Add(this);
        }

        public override string ToString()
        {
            var txt = new StringBuilder();

            txt.Append('(');
            for (int i = 0; i < _parameterTypes.Count; i++)
            {
                if (i > 0)
                    txt.Append(", ");

                var p = _parameterTypes[i];
                txt.Append(p.Identifier.NativeFullName);
            }
            txt.Append(')');

            if (HasTypeReference)
            {
                txt.Append(": ");
                txt.Append(TypeReference.Identifier.NativeFullName);
            }

            return txt.ToString();
        }
    }
}
