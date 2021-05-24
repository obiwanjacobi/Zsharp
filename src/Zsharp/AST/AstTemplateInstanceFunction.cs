namespace Zsharp.AST
{
    public class AstTemplateInstanceFunction : AstFunctionDefinition,
        IAstCodeBlockSite, IAstSymbolTableSite
    {
        public AstTemplateInstanceFunction(AstFunctionDefinition templateDefinition)
        {
            TemplateDefinition = templateDefinition;
            TrySetParent(templateDefinition.Parent);
        }

        public override bool IsIntrinsic => TemplateDefinition.IsIntrinsic;

        public AstFunctionDefinition TemplateDefinition { get; }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTemplateInstanceFunction(this);

        public void Instantiate(CompilerContext context, AstFunctionReference function)
        {
            Context = function.Context;
            var cloner = new AstNodeCloner(context, TemplateDefinition.Indent);
            cloner.Clone(function, TemplateDefinition, this);
        }

        private AstCodeBlock? _codeBlock;
        public AstCodeBlock? CodeBlock => _codeBlock;

        public bool TrySetCodeBlock(AstCodeBlock? codeBlock)
        {
            if (this.SafeSetParent(ref _codeBlock, codeBlock))
            {
                _codeBlock!.Indent = Indent + 1;
                AddFunctionSymbols();
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

                var site = ParentAs<IAstSymbolTableSite>() ??
                    throw new InternalErrorException("Function Parent not a SymbolTable Site.");
                return site.Symbols;
            }
        }

        /// <summary>
        /// Deferred registration of function parameter symbols in the codeblock's symbol table.
        /// </summary>
        private void AddFunctionSymbols()
        {
            foreach (var param in Parameters)
            {
                // function parameters are registered as variables
                var entry = Symbols.AddSymbol(param.Identifier!.CanonicalName, AstSymbolKind.Variable, param);
                param.SetSymbol(entry);
            }
        }
    }
}
