using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public abstract class AstFunction<ParamT, TemplateParamT> : AstNode,
        IAstCodeBlockItem, IAstIdentifierSite, IAstTypeReferenceSite,
        IAstSymbolEntrySite, IAstTemplateSite
        where ParamT : AstFunctionParameter
        where TemplateParamT : AstTemplateParameter
    {
        private readonly List<ParamT> _parameters = new();

        protected AstFunction()
            : base(AstNodeType.Function)
        { }

        public ParserRuleContext? Context { get; protected set; }

        public uint Indent { get; set; }

        public IEnumerable<ParamT> Parameters => _parameters;

        public bool TryAddParameter(ParamT param)
        {
            if (param != null &&
                param.TrySetParent(this))
            {
                // always make sure 'self' is first param
                if (param.Identifier == AstIdentifierIntrinsic.Self)
                    _parameters.Insert(0, param);
                else
                    _parameters.Add(param);
                return true;
            }
            return false;
        }

        public void AddParameter(ParamT param)
        {
            if (!TryAddParameter(param))
                throw new InternalErrorException("Parameter was already set or null.");
        }

        // true when type is a template instantiation
        public bool IsTemplate => _templateParameters.Count > 0;

        private readonly List<TemplateParamT> _templateParameters = new();
        public IEnumerable<AstTemplateParameter> TemplateParameters => _templateParameters;

        public virtual bool TryAddTemplateParameter(AstTemplateParameter? templateParameter)
        {
            if (templateParameter is TemplateParamT parameter)
            {
                _templateParameters.Add(parameter);
                return true;
            }

            return false;
        }

        public string OverloadKey =>
            String.Join(String.Empty, _parameters.Select(p => p.TypeReference?.Identifier?.CanonicalName));

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier? identifier)
            => Ast.SafeSet(ref _identifier, identifier);

        private AstTypeReference? _typeRef;
        public AstTypeReference? TypeReference => _typeRef;

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeRef, typeReference);

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public virtual bool TrySetSymbol(AstSymbolEntry? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);

        public bool TryResolve()
        {
            var entry = _symbol?.SymbolTable.ResolveDefinition(_symbol);
            if (entry != null)
            {
                _symbol = entry;
                return true;
            }
            return false;
        }

        public virtual void CreateSymbols(AstSymbolTable functionSymbols, AstSymbolTable? parentSymbols = null)
        {
            var contextSymbols = parentSymbols ?? functionSymbols;

            if (TypeReference != null &&
                TypeReference!.Symbol == null)
            {
                contextSymbols.Add(TypeReference);
            }

            foreach (var parameter in Parameters)
            {
                if (parameter.TypeReference != null &&
                    parameter.TypeReference.Symbol == null)
                {
                    functionSymbols.Add(parameter.TypeReference);
                }
            }

            Ast.Guard(Symbol == null, "Symbol already set. Call CreateSymbols only once.");
            contextSymbols.Add(this);
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var param in Parameters)
            {
                param.Accept(visitor);
            }

            TypeReference?.Accept(visitor);
        }
    }
}