using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            String.Join(String.Empty, _parameters.Select(p => p.TypeReference?.Identifier?.SymbolName.CanonicalName.FullName));

        private AstTypeReference? _typeReference;
        public AstTypeReference? TypeReference => _typeReference;

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

            TypeReference?.Accept(visitor);
        }

        public void CreateSymbols(AstSymbolTable functionSymbols, AstSymbolTable? parentSymbols = null)
        {
            Ast.Guard(Symbol is null, "Symbol already set. Call CreateSymbols only once.");
            var contextSymbols = parentSymbols ?? functionSymbols;

            contextSymbols.TryAdd(TypeReference);
            foreach (var parameter in Parameters)
            {
                functionSymbols.TryAdd(parameter.TypeReference);
            }

            var symbolName = AstSymbolName.Parse(ToString());
            Identifier!.SymbolName = symbolName;
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
                txt.Append(p.Identifier!.NativeFullName);
                txt.Append(": ");
                txt.Append(p.TypeReference!.Identifier!.NativeFullName);
            }
            txt.Append(')');

            if (TypeReference?.Identifier is not null)
            {
                txt.Append(": ");
                txt.Append(TypeReference.Identifier.NativeFullName);
            }

            return txt.ToString();
        }
    }
}
