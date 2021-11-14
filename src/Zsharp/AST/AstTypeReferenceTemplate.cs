using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstTypeReferenceTemplate : AstTypeReference,
        IAstTemplateUseSite<AstTemplateParameterArgument>
    {
        protected AstTypeReferenceTemplate(ParserRuleContext? context = null)
            : base(context)
        { }

        protected AstTypeReferenceTemplate(AstTypeReferenceTemplate typeToCopy)
            : base(typeToCopy)
        {
            foreach (var templateArgument in typeToCopy.TemplateArguments)
            {
                var newTemplateArg = new AstTemplateParameterArgument(templateArgument);
                TryAddTemplateArgument(newTemplateArg, modifyName: false);
            }

            IsTemplateParameter = typeToCopy.IsTemplateParameter;
            IsGenericParameter = typeToCopy.IsGenericParameter;
        }

        // true when type name is actually a template or generic parameter name (T)
        public bool IsTemplateParameter { get; set; }
        public bool IsGenericParameter { get; set; }

        // true when type is a template instantiation
        public override bool IsTemplateOrGeneric => _templateArguments.Count > 0;

        private readonly List<AstTemplateParameterArgument> _templateArguments = new();
        public IEnumerable<AstTemplateParameterArgument> TemplateArguments => _templateArguments;

        public bool TryAddTemplateArgument(AstTemplateParameterArgument templateArgument)
            => TryAddTemplateArgument(templateArgument, modifyName: true);

        private bool TryAddTemplateArgument(AstTemplateParameterArgument templateArgument, bool modifyName)
        {
            Ast.Guard(Identifier, "Identifier not set - cannot register template argument.");
            if (templateArgument is null)
                return false;

            templateArgument.OrderIndex = _templateArguments.Count;
            _templateArguments.Add(templateArgument);
            templateArgument.SetParent(this);
            if (templateArgument.HasTypeReference && modifyName)
                Identifier.SymbolName.AddTemplateArgument(templateArgument.TypeReference.Identifier.NativeFullName);

            return true;
        }

        public AstTypeDefinitionTemplate? TemplateDefinition
            => Symbol.TemplateDefinitionAs<AstTypeDefinitionTemplate>();

        public override AstTypeDefinition? TypeDefinition
            => IsTemplateOrGeneric
                ? Symbol.TemplateInstanceAs<AstTypeDefinition>(TemplateArguments.Select(p => p.TypeReference))
                : base.TypeDefinition;

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var param in _templateArguments)
            {
                param.Accept(visitor);
            }
        }
    }
}
