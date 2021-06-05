﻿using static Zsharp.Parser.ZsharpParser;

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
                var entry = Symbol!;

                if (entry.HasOverloads)
                {
                    return entry.FindOverloadDefinition(this);
                }

                var functionDef = entry.DefinitionAs<AstFunctionDefinition>();

                if (functionDef?.FunctionType.OverloadKey == FunctionType.OverloadKey)
                    return functionDef;

                return null;
            }
        }

        public bool TryResolveSymbol()
        {
            this.ThrowIfSymbolEntryNotSet();
            var entry = Symbol?.SymbolTable.ResolveDefinition(Symbol);
            if (entry != null)
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

        public override string ToString()
            => $"{Identifier?.CanonicalName}{FunctionType}";
    }
}
