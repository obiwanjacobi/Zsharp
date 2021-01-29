using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    [DebuggerDisplay("{Identifier}")]
    public class AstTypeReference : AstType,
        IAstTemplateSite
    {
        public AstTypeReference(Type_refContext context)
            : base(AstNodeType.Type)
        {
            Context = context;
            IsOptional = context.QUESTION() != null;
            IsError = context.ERROR() != null;
        }

        public AstTypeReference(Enum_base_typeContext context)
            : base(AstNodeType.Type)
        {
            Context = context;
        }

        protected AstTypeReference()
            : base(AstNodeType.Type)
        { }

        private AstTypeReference(AstTypeReference typeOrigin)
            : base(AstNodeType.Type)
        {
            Context = typeOrigin.Context;
            SetIdentifier(typeOrigin.Identifier!);
            TrySetSymbol(typeOrigin.Symbol!);
            _typeOrigin = typeOrigin;
        }

        public AstTypeDefinition? TypeDefinition => Symbol?.DefinitionAs<AstTypeDefinition>();

        public bool IsOptional { get; protected set; }

        public bool IsError { get; protected set; }

        public override bool IsEqual(AstType type)
        {
            if (!base.IsEqual(type))
                return false;

            if (type is not AstTypeReference typedThat)
                return false;

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

        private readonly AstTypeReference? _typeOrigin;
        // points to the origin of this 'proxy'.
        public AstTypeReference? TypeOrigin => _typeOrigin;

        public bool IsProxy => _typeOrigin != null;

        public AstTypeReference MakeProxy()
        {
            if (TypeOrigin != null)
                return new AstTypeReference(TypeOrigin);

            return new AstTypeReference(this);
        }

        public static AstTypeReference Create(AstTypeDefinition typeDef)
        {
            Ast.Guard(typeDef != null, "TypeDefinition is null.");
            Ast.Guard(typeDef!.Identifier != null, "TypeDefinition has no Identifier.");

            var typeRef = new AstTypeReference();
            typeRef.SetIdentifier(typeDef.Identifier!);
            typeRef.TrySetSymbol(typeDef.Symbol!);
            return typeRef;
        }

        // true when type name is actually a template parameter name (T)
        public bool IsTemplateParameter { get; set; }
        // true when type is a template instantiation
        public bool IsTemplate => _parameters.Count > 0;

        private readonly List<AstTemplateParameterReference> _parameters = new List<AstTemplateParameterReference>();
        public IEnumerable<AstTemplateParameter> Parameters => _parameters;

        public bool TryAddTemplateParameter(AstTemplateParameter templateParameter)
        {
            if (templateParameter is AstTemplateParameterReference parameter)
            {
                if (_parameters.SingleOrDefault(p =>
                    p.Identifier?.CanonicalName == parameter.Identifier?.CanonicalName) != null)
                    return false;

                _parameters.Add(parameter);

                Identifier!.AddTemplateParameter(parameter.TypeReference?.Identifier?.Name);
                return true;
            }
            return false;
        }

        public void AddTemplateParameter(AstTemplateParameter templateParameter)
        {
            if (!TryAddTemplateParameter(templateParameter))
                throw new InvalidOperationException(
                    "TemplateParameter is already set or null.");
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeReference(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var param in _parameters)
            {
                param.Accept(visitor);
            }
        }
    }
}