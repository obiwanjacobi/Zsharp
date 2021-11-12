using Antlr4.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public abstract class AstTypeReferenceTemplate : AstTypeReference,
        IAstTemplateUseSite<AstTemplateParameterArgument>,
        IAstGenericUseSite<AstGenericParameterArgument>
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
                this.AddTemplateArgument(newTemplateArg);
            }

            IsTemplateParameter = typeToCopy.IsTemplateParameter;
        }

        // true when type name is actually a template parameter name (T)
        public bool IsTemplateParameter { get; set; }

        // true when type is a template instantiation
        public bool IsTemplate => _templateArguments.Count > 0;

        private readonly List<AstTemplateParameterArgument> _templateArguments = new();
        public IEnumerable<AstTemplateParameterArgument> TemplateArguments => _templateArguments;

        public bool TryAddTemplateArgument(AstTemplateParameterArgument templateArgument)
        {
            Ast.Guard(Identifier, "Identifier not set - cannot register template argument.");
            if (templateArgument is null)
                return false;

            _templateArguments.Add(templateArgument);
            templateArgument.SetParent(this);
            if (templateArgument.HasTypeReference)
                Identifier.SymbolName.AddTemplateArgument(templateArgument.TypeReference.Identifier.NativeFullName);

            return true;
        }

        // true when type name is actually a generic parameter name (T)
        public bool IsGenericParameter { get; set; }

        // true when type is a generic instantiation
        public bool IsGeneric => _genericArguments.Count > 0;

        private readonly List<AstGenericParameterArgument> _genericArguments = new();
        public IEnumerable<AstGenericParameterArgument> GenericArguments => _genericArguments;

        public bool TryAddGenericArgument(AstGenericParameterArgument genericArgument)
        {
            Ast.Guard(Identifier, "Identifier not set - cannot register generic argument.");
            if (genericArgument is null)
                return false;

            _genericArguments.Add(genericArgument);
            genericArgument.SetParent(this);
            if (genericArgument.HasTypeReference)
                Identifier.SymbolName.AddTemplateArgument(genericArgument.TypeReference.Identifier.NativeFullName);

            return true;
        }

        public override AstTypeDefinition? TypeDefinition 
            => IsTemplate 
            ? Symbol.TemplateInstanceAs<AstTypeDefinition>(TemplateArguments.Select(p => p.TypeReference)) 
            : base.TypeDefinition;

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var param in _templateArguments)
            {
                param.Accept(visitor);
            }

            foreach (var param in _genericArguments)
            {
                param.Accept(visitor);
            }
        }
    }
}
