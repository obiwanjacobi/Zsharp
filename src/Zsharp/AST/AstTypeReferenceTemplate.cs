using Antlr4.Runtime;
using System.Collections.Generic;

namespace Zsharp.AST
{
    public abstract class AstTypeReferenceTemplate : AstTypeReference,
        IAstTemplateSite<AstTemplateParameterReference>
    {
        protected AstTypeReferenceTemplate(ParserRuleContext? context = null)
            : base(context)
        { }

        protected AstTypeReferenceTemplate(AstTypeReferenceTemplate typeToCopy)
            : base(typeToCopy)
        { }

        // true when type name is actually a template parameter name (T)
        public bool IsTemplateParameter { get; set; }

        // true when type is a template instantiation
        public bool IsTemplate
            => _templateParameters is not null && _templateParameters.Count > 0;

        private readonly List<AstTemplateParameterReference> _templateParameters = new();
        public IEnumerable<AstTemplateParameterReference> TemplateParameters
            => _templateParameters!;

        public bool TryAddTemplateParameter(AstTemplateParameterReference templateParameter)
        {
            if (templateParameter is null)
                return false;

            _templateParameters.Add(templateParameter);
            templateParameter.SetParent(this);

            Ast.Guard(Identifier, "Identifier not set - cannot register template parameter.");
            Identifier!.SymbolName.AddTemplateParameter(templateParameter.TypeReference?.Identifier?.Name);

            return true;
        }

        protected void CopyTemplateParametersTo(AstTypeReferenceTemplate typeRef)
        {
            foreach (var templateParameter in _templateParameters)
            {
                var newTemplateParam = new AstTemplateParameterReference(templateParameter);
                typeRef.AddTemplateParameter(newTemplateParam);
            }
        }

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