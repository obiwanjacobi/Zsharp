using System;
using System.Collections.Generic;
using System.Diagnostics;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    [DebuggerDisplay("{Identifier}")]
    public class AstTypeReference : AstType,
        IAstTemplateSite
    {
        public AstTypeReference(Type_refContext context)
            : this()
        {
            Context = context;
            IsOptional = context.QUESTION() != null;
            IsError = context.ERROR() != null;
        }

        protected AstTypeReference()
            : base(AstNodeType.Type)
        {
            _templateParameters = new();
        }

        private AstTypeReference(AstTypeReference typeOrigin)
            : base(AstNodeType.Type)
        {
            Context = typeOrigin.Context;
            SetIdentifier(typeOrigin.Identifier!);
            TrySetSymbol(typeOrigin.Symbol!);
            _typeOrigin = typeOrigin;
        }

        public AstTypeDefinition? TypeDefinition
            => Symbol?.DefinitionAs<AstTypeDefinition>();

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
            AstTypeReference typeRef;

            if (TypeOrigin != null)
                typeRef = new AstTypeReference(TypeOrigin);
            else
                typeRef = new AstTypeReference(this);

            typeRef.IsTemplateParameter = IsTemplateParameter;

            return typeRef;
        }

        public static AstTypeReference From(AstTypeDefinition typeDef)
        {
            Ast.Guard(typeDef != null, "TypeDefinition is null.");
            Ast.Guard(typeDef!.Identifier != null, "TypeDefinition has no Identifier.");

            var typeRef = new AstTypeReference();
            typeRef.SetIdentifier(typeDef.Identifier!);
            if (typeDef.Symbol != null)
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
        public IEnumerable<AstTemplateParameter> TemplateParameters
            => TypeOrigin?.TemplateParameters ?? _templateParameters!;

        public bool TryAddTemplateParameter(AstTemplateParameter templateParameter)
        {
            if (TypeOrigin != null || _templateParameters == null)
                throw new InvalidOperationException(
                    "Cannot add Template Parameter onto a TypeReference Proxy.");

            if (templateParameter is AstTemplateParameterReference parameter)
            {
                _templateParameters.Add(parameter);

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
            if (_templateParameters != null)
            {
                foreach (var param in _templateParameters)
                {
                    param.Accept(visitor);
                }
            }
        }
    }
}