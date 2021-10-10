using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionReference : AstFunction,
        IAstTemplateSite<AstTemplateParameterReference>
    {
        public AstFunctionReference(Function_callContext context)
        {
            Context = context;
            EnforceReturnValueUse = context.Parent is not Function_call_retval_unusedContext;
            _functionType = new AstTypeReferenceFunction(context);
            _functionType.SetParent(this);
        }

        public bool EnforceReturnValueUse { get; }

        private readonly AstTypeReferenceFunction _functionType;
        public AstTypeReferenceFunction FunctionType => _functionType;

        public AstFunctionDefinition? FunctionDefinition
        {
            get
            {
                this.ThrowIfSymbolEntryNotSet();
                return Symbol!.FindFunctionDefinition(this);
            }
        }

        public bool TryResolveSymbol()
        {
            this.ThrowIfSymbolEntryNotSet();
            return Symbol!.SymbolTable.TryResolveDefinition(Symbol);
        }

        // true when type is a template instantiation
        public bool IsTemplate
            => _templateParameters!.Count > 0;

        private readonly List<AstTemplateParameterReference> _templateParameters = new();
        public IEnumerable<AstTemplateParameterReference> TemplateParameters
            => _templateParameters!;

        public bool TryAddTemplateParameter(AstTemplateParameterReference templateParameter)
        {
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
            => visitor.VisitFunctionReference(this);

        public override void VisitChildren(AstVisitor visitor)
            => FunctionType.Accept(visitor);

        public override void CreateSymbols(AstSymbolTable functionSymbols, AstSymbolTable? parentSymbols = null)
        {
            Ast.Guard(Symbol is null, "Symbol already set. Call CreateSymbols only once.");

            FunctionType.CreateSymbols(functionSymbols, parentSymbols);

            var contextSymbols = parentSymbols ?? functionSymbols;
            contextSymbols.Add(this);
        }

        public override string ToString()
            => $"{Identifier?.CanonicalName}: {FunctionType}";
    }
}
