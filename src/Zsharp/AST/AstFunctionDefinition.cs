using Antlr4.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zsharp.AST
{
    public abstract class AstFunctionDefinition : AstFunction,
        IAstTemplateSite<AstTemplateParameterDefinition>
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

        public AstTypeDefinitionFunction FunctionType { get; protected set; }

        public override void CreateSymbols(AstSymbolTable functionSymbols, AstSymbolTable? parentSymbols = null)
        {
            Ast.Guard(Symbol is null, "Symbol already set. Call CreateSymbols only once.");

            FunctionType.CreateSymbols(functionSymbols, parentSymbols);

            var contextSymbols = parentSymbols ?? functionSymbols;
            contextSymbols.Add(this);
        }

        public bool IsTemplate => _templateParameters.Count > 0;

        private readonly List<AstTemplateParameterDefinition> _templateParameters = new();
        public IEnumerable<AstTemplateParameterDefinition> TemplateParameters => _templateParameters;

        public virtual bool TryAddTemplateParameter(AstTemplateParameterDefinition templateParameter)
        {
            if (templateParameter is null)
                return false;

            _templateParameters.Add(templateParameter);

            Ast.Guard(Identifier, "Identifier not set - cannot register template parameter.");
            Identifier!.SymbolName.SetTemplateParameterCount(_templateParameters.Count);

            return true;
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitFunctionDefinition(this);

        public override void VisitChildren(AstVisitor visitor)
            => FunctionType.Accept(visitor);

        public override string ToString()
        {
            var txt = new StringBuilder(Identifier?.CanonicalName);
            if (IsTemplate)
            {
                txt.Append('<');
                for (int i = 0; i < TemplateParameters.Count(); i++)
                {
                    if (i > 0)
                        txt.Append(", ");

                    var p = TemplateParameters.ElementAt(i);
                    txt.Append(p.Identifier!.Name);
                }
                txt.Append('>');
            }

            txt.Append(FunctionType.ToString());
            return txt.ToString();
        }

    }
}
