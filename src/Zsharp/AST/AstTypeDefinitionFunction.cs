﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstTypeDefinitionFunction : AstTypeDefinition,
        IAstTypeReferenceSite,
        IAstFunctionParameters<AstFunctionParameterDefinition>
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

        private readonly List<AstFunctionParameterDefinition> _parameters = new();
        public IEnumerable<AstFunctionParameterDefinition> Parameters => _parameters;

        public bool TryAddParameter(AstFunctionParameterDefinition param)
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
            foreach (var param in Parameters)
            {
                param.Accept(visitor);
            }

            if (HasTypeReference)
                TypeReference.Accept(visitor);
        }

        public void CreateSymbols(AstSymbolTable functionSymbols, AstSymbolTable? parentSymbols = null)
        {
            Ast.Guard(!HasSymbol, "Symbol already set. Call CreateSymbols only once.");
            var contextSymbols = parentSymbols ?? functionSymbols;

            var name = new StringBuilder();
            foreach (var parameter in Parameters)
            {
                if (name.Length > 0)
                    name.Append(',');

                if (parameter.HasTypeReference)
                {
                    functionSymbols.TryAdd(parameter.TypeReference);
                    name.Append(parameter.TypeReference.Identifier.CanonicalFullName);
                }
                else
                    name.Append('?');
            }

            name.Insert(0, '(');

            if (HasTypeReference)
            {
                contextSymbols.TryAdd(TypeReference);

                name.Append("): ")
                    .Append(TypeReference.Identifier.CanonicalFullName);
            }
            else
                name.Append(')');

            var canonical = AstName.CreateUnparsed(name.ToString(), AstNameKind.Canonical);
            Identifier.SymbolName = new AstSymbolName(canonical);
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
                txt.Append(p.Identifier.NativeFullName);
                txt.Append(": ");
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
    }
}
