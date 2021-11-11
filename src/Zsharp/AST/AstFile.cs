using Antlr4.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public class AstFile : AstNode,
        IAstSymbolTableSite, IAstCodeBlockSite
    {
        internal AstFile(string scopeName, AstSymbolTable parentTable, ParserRuleContext context)
            : base(AstNodeKind.File)
        {
            Context = context;
            this.SetCodeBlock(new AstCodeBlock(scopeName, parentTable));
        }

        public ParserRuleContext Context { get; }

        public IEnumerable<AstFunctionDefinitionImpl> Functions
            => SymbolTable.FindSymbols(AstSymbolKind.Function)
                .Select(s => s.DefinitionAs<AstFunctionDefinitionImpl>())
                .Where(f => f is not null)!;

        public AstSymbolTable SymbolTable
            => _codeBlock?.SymbolTable ?? throw new InternalErrorException("CodeBlock was not set. No SymbolTable available.");

        public bool HasCodeBlock => _codeBlock is not null;

        private AstCodeBlock? _codeBlock;
        public AstCodeBlock CodeBlock
            => _codeBlock ?? throw new InternalErrorException("CodeBlock was not set.");

        public bool HasExports
            => CodeBlock.SymbolTable.Symbols.Any(e => e.SymbolLocality == AstSymbolLocality.Exported);

        public bool TrySetCodeBlock(AstCodeBlock? codeBlock)
            => this.SafeSetParent(ref _codeBlock, codeBlock);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitFile(this);

        public override void VisitChildren(AstVisitor visitor)
            => _codeBlock?.Accept(visitor);
    }
}
