using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionReference : AstFunction
    {
        public AstFunctionReference(Function_callContext context)
        {
            Context = context;
            EnforceReturnValueUse = context.Parent is not Function_call_retval_unusedContext;
            _functionType = new AstTypeReferenceFunction(context);
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
            var entry = Symbol?.SymbolTable.ResolveDefinition(Symbol);
            if (entry is not null)
            {
                Symbol = entry;
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
            Identifier!.SymbolName.TemplatePostfix = FunctionType.Identifier!.SymbolName.TemplatePostfix;

            var contextSymbols = parentSymbols ?? functionSymbols;
            contextSymbols.Add(this);
        }

        public override string ToString()
            => $"{Identifier?.CanonicalName}: {FunctionType}";

        internal void OverrideTypes(AstTypeReference? returnType, IEnumerable<AstTypeReference> parameterTypes)
        {
            FunctionType.OverrideTypes(returnType, parameterTypes);
            Identifier!.SymbolName.TemplatePostfix = FunctionType.Identifier!.CanonicalName;
        }
    }
}
