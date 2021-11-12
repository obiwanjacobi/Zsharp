using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zsharp.AST
{
    public class AstTypeReferenceFunction : AstTypeReference,
        IAstTypeReferenceSite,
        IAstFunctionArguments<AstFunctionParameterArgument>
    {
        public AstTypeReferenceFunction(ParserRuleContext context)
            : base(context)
        {
            this.SetIdentifier(new AstIdentifier(String.Empty, AstIdentifierKind.Type));
        }

        private AstTypeReferenceFunction(AstTypeReferenceFunction typeOrigin)
            : base(typeOrigin)
        { }

        private readonly List<AstFunctionParameterArgument> _arguments = new();
        public IEnumerable<AstFunctionParameterArgument> Arguments => _arguments;

        public bool TryAddArgument(AstFunctionParameterArgument argument)
        {
            if (argument is not null &&
                argument.TrySetParent(this))
            {
                // always make sure 'self' is first param
                if (argument.HasIdentifier && argument.Identifier == AstIdentifierIntrinsic.Self)
                    _arguments.Insert(0, argument);
                else
                    _arguments.Add(argument);
                return true;
            }
            return false;
        }

        public string OverloadKey =>
            String.Join(String.Empty, _arguments
                .Where(p => p.HasTypeReference)
                .Select(p => p.TypeReference.Identifier.SymbolName.CanonicalName.FullName));

        public override AstTypeReferenceFunction MakeCopy()
        {
            var typeRef = new AstTypeReferenceFunction(this);
            Symbol.AddNode(typeRef);
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
            foreach (var param in Arguments)
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
            for (int i = 0; i < Arguments.Count(); i++)
            {
                if (i > 0)
                    txt.Append(", ");

                var p = Arguments.ElementAt(i);
                if (p.HasTypeReference)
                    txt.Append(p.TypeReference.Identifier.NativeFullName);
                else
                    txt.Append('?');
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

            foreach (var parameter in Arguments)
            {
                if (parameter.HasTypeReference)
                    functionSymbols.TryAdd(parameter.TypeReference);
            }
        }

        public bool SetDefinition(AstSymbolTable symbolTable, AstTypeDefinitionFunction functionTypeDef)
        {
            var map = new AstFunctionArgumentMap(functionTypeDef.Parameters, Arguments);
            var name = new StringBuilder();

            for (int i = 0; i < map.Count; i++)
            {
                var p = map.ParameterAt(i);
                var a = map.ArgumentAt(i);

                if (a is null)
                    break;

                var paramTypeRef = p.TypeReference.MakeCopy();
                a.TrySetTypeReference(paramTypeRef);

                if (name.Length > 0)
                    name.Append(',');
                name.Append(paramTypeRef.Identifier.CanonicalFullName);
            }

            name.Insert(0, '(');

            var typeRef = functionTypeDef.TypeReference.MakeCopy();
            this.SetTypeReference(typeRef);

            if (typeRef.Identifier != AstIdentifierIntrinsic.Void)
            {
                name.Append("): ")
                    .Append(typeRef.Identifier.CanonicalFullName);
            }
            else
                name.Append(')');

            Identifier.SymbolName = AstSymbolName.ParseCanonical(name.ToString());
            symbolTable.Add(this);
            return symbolTable.TryResolveDefinition(Symbol);
        }

        public AstTypeReference? ReplaceTypeReference(AstTypeReference typeReference)
        {
            var oldTypeRef = _typeReference;
            _typeReference = typeReference;
            return oldTypeRef;
        }
    }
}
