using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstFunctionDefinition : AstFunction
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
            Identifier?.Append(FunctionType?.Identifier?.CanonicalName);

            var contextSymbols = parentSymbols ?? functionSymbols;

            if (FunctionType.TypeReference != null &&
                FunctionType.TypeReference!.Symbol == null)
            {
                contextSymbols.Add(FunctionType.TypeReference);
            }

            foreach (var parameter in FunctionType.Parameters)
            {
                if (parameter.TypeReference != null &&
                    parameter.TypeReference.Symbol == null)
                {
                    functionSymbols.Add(parameter.TypeReference);
                }
            }

            Ast.Guard(Symbol == null, "Symbol already set. Call CreateSymbols only once.");
            contextSymbols.Add(this);
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitFunctionDefinition(this);

        public override void VisitChildren(AstVisitor visitor)
            => FunctionType.Accept(visitor);

        public override string ToString()
            => $"{Identifier?.CanonicalName}{FunctionType}";
    }
}
