using Antlr4.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public abstract class AstTypeReferenceTemplate : AstTypeReference,
        IAstTemplateSite<AstTemplateParameterReference>,
        IAstGenericSite<AstGenericParameterReference>
    {
        protected AstTypeReferenceTemplate(ParserRuleContext? context = null)
            : base(context)
        { }

        protected AstTypeReferenceTemplate(AstTypeReferenceTemplate typeToCopy)
            : base(typeToCopy)
        {
            foreach (var templateParameter in typeToCopy.TemplateParameters)
            {
                var newTemplateParam = new AstTemplateParameterReference(templateParameter);
                this.AddTemplateParameter(newTemplateParam);
            }

            IsTemplateParameter = typeToCopy.IsTemplateParameter;
        }

        // true when type name is actually a template parameter name (T)
        public bool IsTemplateParameter { get; set; }

        // true when type is a template instantiation
        public bool IsTemplate => _templateParameters.Count > 0;

        private readonly List<AstTemplateParameterReference> _templateParameters = new();
        public IEnumerable<AstTemplateParameterReference> TemplateParameters => _templateParameters;

        public bool TryAddTemplateParameter(AstTemplateParameterReference templateParameter)
        {
            Ast.Guard(Identifier, "Identifier not set - cannot register template parameter.");
            if (templateParameter is null)
                return false;

            _templateParameters.Add(templateParameter);
            templateParameter.SetParent(this);
            if (templateParameter.HasTypeReference)
                Identifier.SymbolName.AddTemplateParameter(templateParameter.TypeReference.Identifier.NativeFullName);

            return true;
        }

        // true when type name is actually a generic parameter name (T)
        public bool IsGenericParameter { get; set; }

        // true when type is a generic instantiation
        public bool IsGeneric => _genericParameters.Count > 0;

        private readonly List<AstGenericParameterReference> _genericParameters = new();
        public IEnumerable<AstGenericParameterReference> GenericParameters => _genericParameters;

        public bool TryAddGenericParameter(AstGenericParameterReference genericParameter)
        {
            Ast.Guard(Identifier, "Identifier not set - cannot register generic parameter.");
            if (genericParameter is null)
                return false;

            _genericParameters.Add(genericParameter);
            genericParameter.SetParent(this);
            if (genericParameter.HasTypeReference)
                Identifier.SymbolName.AddTemplateParameter(genericParameter.TypeReference.Identifier.NativeFullName);

            return true;
        }

        public override AstTypeDefinition? TypeDefinition 
            => IsTemplate 
            ? Symbol.TemplateInstanceAs<AstTypeDefinition>(TemplateParameters.Select(p => p.TypeReference)) 
            : base.TypeDefinition;

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var param in _templateParameters)
            {
                param.Accept(visitor);
            }

            foreach (var param in _genericParameters)
            {
                param.Accept(visitor);
            }
        }
    }
}
