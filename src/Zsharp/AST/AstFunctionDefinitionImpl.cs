using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionDefinitionImpl : AstFunctionDefinition,
        IAstCodeBlockSite, IAstSymbolTableSite
    {
        public AstFunctionDefinitionImpl(Function_defContext functionCtx)
        {
            Context = functionCtx;
        }

        private AstCodeBlock? _codeBlock;
        public AstCodeBlock? CodeBlock => _codeBlock;

        public bool TrySetCodeBlock(AstCodeBlock? codeBlock)
        {
            if (this.SafeSetParent(ref _codeBlock, codeBlock))
            {
                _codeBlock!.Indent = Indent + 1;
                return true;
            }
            return false;
        }

        public AstSymbolTable Symbols
        {
            get
            {
                var codeBlock = CodeBlock;
                if (codeBlock != null)
                {
                    return codeBlock.Symbols;
                }

                // When the Node Builder is building up the function definition,
                // the CodeBlock that has the symbols of the function impl
                // is not yet created. When the builder gets to the CodeBlock
                // it needs a parent SymbolTable and the function instance
                // on the current-stack is a current SymbolTable site.
                // So we have to implement the fallback here.

                var site = ParentAs<IAstSymbolTableSite>() ??
                    throw new InvalidOperationException("Function Parent not a SymbolTable Site.");
                return site.Symbols;
            }
        }

        public AstSymbolEntry AddSymbol(string symbolName, AstSymbolKind kind, AstNode? node = null)
        {
            this.ThrowIfSymbolTableNotSet();
            return Symbols!.AddSymbol(symbolName, kind, node);
        }

        public override void CreateSymbols(AstSymbolTable functionSymbols, AstSymbolTable? parentSymbols = null)
        {
            base.CreateSymbols(functionSymbols, parentSymbols);

            foreach (var parameter in Parameters)
            {
                if (parentSymbols != null &&
                    parameter.Symbol == null)
                {
                    functionSymbols.Add(parameter);
                }
            }
        }

        public override bool TryAddTemplateParameter(AstTemplateParameter? templateParameter)
        {
            if (base.TryAddTemplateParameter(templateParameter))
            {
                Symbols.Add((AstTemplateParameterDefinition)templateParameter!);
                return true;
            }
            return false;
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            base.VisitChildren(visitor);
            CodeBlock?.Accept(visitor);
        }
    }
}
