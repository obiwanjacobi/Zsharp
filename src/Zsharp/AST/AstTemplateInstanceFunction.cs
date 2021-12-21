namespace Zsharp.AST
{
    public class AstTemplateInstanceFunction : AstFunctionDefinition,
        IAstCodeBlockSite, IAstSymbolTableSite, IAstTemplateInstance
    {
        public AstTemplateInstanceFunction(AstFunctionDefinition templateDefinition)
            : base(new AstTypeDefinitionFunction())
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
            _templateArguments = new AstTemplateArgumentMap(
                TemplateDefinition.ParameterList, function.TemplateArguments);

            var cloner = new AstNodeCloner(context, TemplateDefinition.Indent)
            {
                CloneCodeBlock = TemplateDefinition.IsTemplate
            };
            cloner.Clone(function, TemplateDefinition, this, _templateArguments);

            if (TemplateDefinition.IsGeneric &&
                TemplateDefinition is AstFunctionDefinitionImpl funcImpl)
            {
                _codeBlock = funcImpl.CodeBlock;
            }
        }

        public bool HasCodeBlock => _codeBlock is not null;

        private AstCodeBlock? _codeBlock;
        public AstCodeBlock CodeBlock
            => _codeBlock ?? throw new InternalErrorException("CodeBlock was not set.");

        public bool TrySetCodeBlock(AstCodeBlock? codeBlock)
        {
            if (TemplateDefinition.IsTemplate &&
                this.SafeSetParent(ref _codeBlock, codeBlock))
            {
                _codeBlock!.Indent = Indent + 1;
                AddFunctionSymbols();
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

                var site = ParentAs<IAstSymbolTableSite>() ??
                    throw new InternalErrorException("Function Parent not a SymbolTable Site.");
                return site.SymbolTable;
            }
        }

        private AstTemplateArgumentMap? _templateArguments;
        public AstTemplateArgumentMap TemplateParameterArguments
            => _templateArguments ?? AstTemplateArgumentMap.Empty;

        /// <summary>
        /// Deferred registration of function parameter symbols in the codeblock's symbol table.
        /// </summary>
        private void AddFunctionSymbols()
        {
            foreach (var param in Parameters)
            {
                // function parameters are registered as variables
                SymbolTable.AddSymbol(param.Identifier.SymbolName.CanonicalName.FullName, AstSymbolKind.Variable, param);
            }
        }
    }
}
