﻿using Antlr4.Runtime;
using System;

namespace Zsharp.AST
{
    public class AstFunctionDefinitionImpl : AstFunctionDefinition,
        IAstCodeBlockSite, IAstSymbolTableSite
    {
        internal AstFunctionDefinitionImpl(ParserRuleContext context)
            : base(context)
        { }

        public bool HasCodeBlock => _codeBlock is not null;

        private AstCodeBlock? _codeBlock;
        public AstCodeBlock CodeBlock
            => _codeBlock ?? throw new InternalErrorException("CodeBlock was not set.");

        public bool TrySetCodeBlock(AstCodeBlock? codeBlock)
        {
            if (this.SafeSetParent(ref _codeBlock, codeBlock))
            {
                _codeBlock!.Indent = Indent + 1;
                return true;
            }
            return false;
        }

        public AstSymbolTable SymbolTable
        {
            get
            {
                if (HasCodeBlock)
                {
                    return CodeBlock.SymbolTable;
                }

                // When the Node Builder is building up the function definition,
                // the CodeBlock that has the symbols of the function impl
                // is not yet created. When the builder gets to the CodeBlock
                // it needs a parent SymbolTable and the function instance
                // on the current-stack is a current SymbolTable site.
                // So we have to implement the fallback here.

                var site = ParentAs<IAstSymbolTableSite>() ??
                    throw new InvalidOperationException("Function Parent not a SymbolTable Site.");
                return site.SymbolTable;
            }
        }

        public override void CreateSymbols(AstSymbolTable functionSymbols, AstSymbolTable? parentSymbols = null)
        {
            base.CreateSymbols(functionSymbols, parentSymbols);

            foreach (var parameter in FunctionType.Parameters)
            {
                functionSymbols.TryAdd(parameter);
            }

            foreach (var templParam in TemplateParameters)
            {
                SymbolTable.TryAdd(templParam);
            }
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            base.VisitChildren(visitor);
            if (HasCodeBlock)
                CodeBlock.Accept(visitor);
        }
    }
}
