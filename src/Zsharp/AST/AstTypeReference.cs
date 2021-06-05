using Antlr4.Runtime;
using System.Collections.Generic;
using System.Diagnostics;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    [DebuggerDisplay("{Identifier}")]
    public class AstTypeReference : AstType,
        IAstTemplateSite<AstTemplateParameterReference>
    {
        public AstTypeReference(Type_refContext context)
            : base(AstNodeType.Type)
        {
            Context = context;
            _templateParameters = new();
        }

        protected AstTypeReference(ParserRuleContext? context = null)
            : base(AstNodeType.Type)
        {
            Context = context;
            _templateParameters = new();
        }

        protected AstTypeReference(AstTypeReference typeOrigin)
            : base(AstNodeType.Type)
        {
            Context = typeOrigin.Context;
            this.SetIdentifier(typeOrigin.Identifier!);
            TrySetSymbol(typeOrigin.Symbol!);
            _typeOrigin = typeOrigin;
        }

        public AstTypeDefinition? TypeDefinition
            => Symbol?.DefinitionAs<AstTypeDefinition>();

        public virtual bool TryResolveSymbol()
        {
            this.ThrowIfSymbolEntryNotSet();
            var entry = Symbol?.SymbolTable.ResolveDefinition(Symbol);
            if (entry is not null)
            {
                Symbol = entry;
                return true;
            }
            return false;
        }

        public virtual bool IsExternal => false;

        public override bool IsEqual(AstType type)
        {
            if (!base.IsEqual(type))
                return false;

            if (type is not AstTypeReference typedThat)
                return false;

            var typeDef = TypeDefinition;
            var thatTypeDef = typedThat.TypeDefinition;

            if (typeDef is not null &&
                thatTypeDef is not null)
            {
                return typeDef.IsEqual(thatTypeDef);
            }
            return false;
        }

        private readonly AstTypeReference? _typeOrigin;
        // points to the origin of this 'proxy'.
        public AstTypeReference? TypeOrigin => _typeOrigin;

        public bool IsProxy => _typeOrigin is not null;

        public virtual AstTypeReference MakeProxy()
        {
            AstTypeReference typeRef;

            if (TypeOrigin is not null)
                typeRef = new AstTypeReference(TypeOrigin);
            else
                typeRef = new AstTypeReference(this);

            typeRef.IsTemplateParameter = IsTemplateParameter;

            return typeRef;
        }

        public static AstTypeReference From(AstTypeDefinition typeDef)
        {
            Ast.Guard(typeDef is not null, "TypeDefinition is null.");
            Ast.Guard(typeDef!.Identifier is not null, "TypeDefinition has no Identifier.");

            var typeRef = new AstTypeReference();
            typeRef.SetIdentifier(typeDef.Identifier!);
            if (typeDef.Symbol is not null)
            {
                typeRef.TrySetSymbol(typeDef.Symbol);
                typeDef.Symbol.AddNode(typeRef);
            }
            return typeRef;
        }

        // true when type name is actually a template parameter name (T)
        public bool IsTemplateParameter { get; set; }
        // true when type is a template instantiation
        public bool IsTemplate
            => TypeOrigin?.IsTemplate ?? _templateParameters!.Count > 0;

        private readonly List<AstTemplateParameterReference>? _templateParameters;
        public IEnumerable<AstTemplateParameterReference> TemplateParameters
            => TypeOrigin?.TemplateParameters ?? _templateParameters!;

        public bool TryAddTemplateParameter(AstTemplateParameterReference templateParameter)
        {
            if (TypeOrigin is not null || _templateParameters is null)
                throw new InternalErrorException(
                    "Cannot add Template Parameter onto a TypeReference Proxy.");

            if (templateParameter is AstTemplateParameterReference parameter)
            {
                _templateParameters.Add(parameter);

                if (Identifier is not null)
                    Identifier.SymbolName.AddTemplateParameter(parameter.TypeReference?.Identifier?.Name);
                return true;
            }
            return false;
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeReference(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            if (_templateParameters is not null)
            {
                foreach (var param in _templateParameters)
                {
                    param.Accept(visitor);
                }
            }
        }
    }
}