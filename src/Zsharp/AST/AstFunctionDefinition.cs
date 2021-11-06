using Antlr4.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zsharp.AST
{
    public abstract class AstFunctionDefinition : AstFunction,
        IAstTemplateSite<AstTemplateParameterDefinition>,
        IAstGenericSite<AstGenericParameterDefinition>
    {
        protected AstFunctionDefinition(AstTypeDefinitionFunction functionType)
        {
            FunctionType = functionType;
        }

        protected AstFunctionDefinition(ParserRuleContext context)
        {
            Context = context;
            FunctionType = new AstTypeDefinitionFunction(context);
        }

        public virtual bool IsIntrinsic => false;

        public virtual bool IsExternal => false;

        private AstTypeDefinitionFunction? _functionType;
        public AstTypeDefinitionFunction FunctionType
        {
            get { return _functionType ?? throw new InternalErrorException("FunctionType was not set."); }
            protected set
            {
                _functionType = value ?? throw new InternalErrorException("FunctionType value is null.");
                _functionType?.SetParent(this);
            }
        }

        public override void CreateSymbols(AstSymbolTable functionSymbols, AstSymbolTable? parentSymbols = null)
        {
            Ast.Guard(!HasSymbol, "Symbol already set. Call CreateSymbols only once.");

            FunctionType.CreateSymbols(functionSymbols, parentSymbols);

            var contextSymbols = parentSymbols ?? functionSymbols;
            contextSymbols.Add(this);
        }

        public bool IsTemplate => _templateParameters.Count > 0;

        private readonly List<AstTemplateParameterDefinition> _templateParameters = new();
        public IEnumerable<AstTemplateParameterDefinition> TemplateParameters => _templateParameters;

        public virtual bool TryAddTemplateParameter(AstTemplateParameterDefinition templateParameter)
        {
            Ast.Guard(HasIdentifier, "Identifier not set - cannot register template parameter.");
            if (templateParameter is null)
                return false;

            _templateParameters.Add(templateParameter);
            templateParameter.SetParent(this);
            Identifier.SymbolName.SetParameterCounts(_templateParameters.Count, _genericParameters.Count);

            return true;
        }

        public bool IsGeneric => _genericParameters.Count > 0;

        private readonly List<AstGenericParameterDefinition> _genericParameters = new();
        public IEnumerable<AstGenericParameterDefinition> GenericParameters => _genericParameters;

        public virtual bool TryAddGenericParameter(AstGenericParameterDefinition genericParameter)
        {
            Ast.Guard(HasIdentifier, "Identifier not set - cannot register generic parameter.");
            if (genericParameter is null)
                return false;

            _genericParameters.Add(genericParameter);
            genericParameter.SetParent(this);
            Identifier.SymbolName.SetParameterCounts(_templateParameters.Count, _genericParameters.Count);

            return true;
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitFunctionDefinition(this);

        public override void VisitChildren(AstVisitor visitor)
            => FunctionType.Accept(visitor);

        public override string ToString()
        {
            var txt = new StringBuilder(Identifier.SymbolName.CanonicalName.FullName);
            if (IsTemplate)
            {
                txt.Append('<');
                for (int i = 0; i < TemplateParameters.Count(); i++)
                {
                    if (i > 0)
                        txt.Append(", ");

                    var p = TemplateParameters.ElementAt(i);
                    txt.Append(p.Identifier.SymbolName.CanonicalName.FullName);
                }
                txt.Append('>');
            }

            txt.Append(FunctionType.ToString());
            return txt.ToString();
        }

    }
}
