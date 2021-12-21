using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstFunctionDefinition : AstFunction,
        IAstFunctionParameters<AstFunctionParameterDefinition>,
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

        private readonly List<AstFunctionParameterDefinition> _parameters = new();
        public IEnumerable<AstFunctionParameterDefinition> Parameters => _parameters;

        public bool TryAddParameter(AstFunctionParameterDefinition param)
        {
            if (param is not null &&
                param.TrySetParent(this))
            {
                FunctionType.AddParameterType(param.TypeReference.MakeCopy());

                // always make sure 'self' is first param
                if (param.HasIdentifier && param.Identifier == AstIdentifierIntrinsic.Self)
                    _parameters.Insert(0, param);
                else
                    _parameters.Add(param);
                return true;
            }
            return false;
        }

        public override void CreateSymbols(AstSymbolTable? functionSymbols, AstSymbolTable? parentSymbols)
        {
            Ast.Guard(functionSymbols is not null || parentSymbols is not null, "One of the SymbolTable parameters must be specified.");
            Ast.Guard(!HasSymbol, "Symbol already set. Call CreateSymbols only once.");

            FunctionType.CreateSymbols(functionSymbols, parentSymbols);

            foreach (var param in _parameters)
            {
                functionSymbols?.Add(param);
                if (param.HasTypeReference && !param.TypeReference.HasSymbol)
                    parentSymbols?.Add(param.TypeReference);
            }

            foreach (var templParam in TemplateParameters)
            {
                functionSymbols?.TryAdd(templParam);
            }

            parentSymbols?.Add(this);
        }

        public override bool TrySetIdentifier(AstIdentifier identifier)
        {
            if (base.TrySetIdentifier(identifier))
            {
                Identifier.SymbolName.SetParameterCount(_parameterList.Count);
                return true;
            }
            return false;
        }

        private readonly List<AstTemplateParameter> _parameterList = new();
        public IEnumerable<AstTemplateParameter> ParameterList => _parameterList;

        public T TemplateParameterAt<T>(int index) where T : AstTemplateParameter
            => (T)_parameterList[index];

        public bool IsTemplate => _templateParameters.Count > 0;

        private readonly List<AstTemplateParameterDefinition> _templateParameters = new();
        public IEnumerable<AstTemplateParameterDefinition> TemplateParameters => _templateParameters;

        public virtual bool TryAddTemplateParameter(AstTemplateParameterDefinition templateParameter)
        {
            if (templateParameter is null)
                return false;

            _parameterList.Add(templateParameter);
            _templateParameters.Add(templateParameter);
            templateParameter.SetParent(this);
            if (HasIdentifier)
                Identifier.SymbolName.SetParameterCount(_parameterList.Count);

            return true;
        }

        public bool IsGeneric => _genericParameters.Count > 0;

        private readonly List<AstGenericParameterDefinition> _genericParameters = new();
        public IEnumerable<AstGenericParameterDefinition> GenericParameters => _genericParameters;

        public virtual bool TryAddGenericParameter(AstGenericParameterDefinition genericParameter)
        {
            if (genericParameter is null)
                return false;

            _parameterList.Add(genericParameter);
            _genericParameters.Add(genericParameter);
            genericParameter.SetParent(this);
            if (HasIdentifier)
                Identifier.SymbolName.SetParameterCount(_parameterList.Count);

            return true;
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitFunctionDefinition(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            FunctionType.Accept(visitor);

            foreach (var param in _parameterList)
            {
                param.Accept(visitor);
            }

            foreach (var param in Parameters)
            {
                param.Accept(visitor);
            }
        }

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
