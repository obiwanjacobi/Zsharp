using Antlr4.Runtime;
using System.Collections.Generic;

namespace Zsharp.AST
{
    public class AstFunctionReference : AstFunction,
        IAstTemplateSite<AstTemplateParameterReference>
    {
        internal AstFunctionReference(ParserRuleContext context, bool enforceReturnValueUse)
        {
            Context = context;
            EnforceReturnValueUse = enforceReturnValueUse;
            //DeferResolveDefinition = _    do not copy this!
            _functionType = new AstTypeReferenceFunction(context);
            _functionType.SetParent(this);
        }

        public bool EnforceReturnValueUse { get; }

        // Do not try to resolve definition until after this is cloned for template instantiation.
        public bool DeferResolveDefinition { get; private set; }

        private readonly AstTypeReferenceFunction _functionType;
        public AstTypeReferenceFunction FunctionType => _functionType;

        public AstFunctionDefinition? FunctionDefinition
            => Symbol.FindFunctionDefinition(this);

        public bool TryResolveSymbol()
        {
            var symbolTable = Symbol.SymbolTable;
            if (symbolTable.TryResolveDefinition(Symbol))
            {
                var funcDef = FunctionDefinition;
                if (funcDef is not null)
                    FunctionType.SetDefinition(symbolTable, funcDef.FunctionType);
                return  true;
            }

            var templateParamSymbol = symbolTable.FindSymbol(Identifier.CanonicalFullName, AstSymbolKind.TemplateParameter);
            if (templateParamSymbol is not null)
            {
                // We found a template parameter with the exact same name as the function reference.
                // This means that this function reference represents a conversion function for a template type (T).
                // Defer resolve-definition untill after the template has been instantiated.
                DeferResolveDefinition = true;
            }
            return false;
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

                Identifier.SymbolName.AddTemplateParameter(parameter.TypeReference.Identifier.NativeFullName);
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
            Ast.Guard(!HasSymbol, "Symbol already set. Call CreateSymbols only once.");
            FunctionType.CreateSymbols(functionSymbols, parentSymbols);

            var contextSymbols = parentSymbols ?? functionSymbols;
            contextSymbols.Add(this);
        }

        public override string ToString()
            => $"{Identifier.SymbolName.CanonicalName.FullName}: {FunctionType}";
    }
}
