using System.Collections.Generic;
using System.Linq;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFile : AstNode,
        IAstSymbolTableSite, IAstCodeBlockSite
    {
        public AstFile(string scopeName, AstSymbolTable parentTable, FileContext context)
            : base(AstNodeType.File)
        {
            Context = context;
            this.SetCodeBlock(new AstCodeBlock(scopeName, parentTable));
        }

        public FileContext Context { get; }

        public IEnumerable<AstFunctionDefinitionImpl> Functions
            => Symbols.FindEntries(AstSymbolKind.Function)
                .Select(s => s.DefinitionAs<AstFunctionDefinitionImpl>())
                .Where(f => f is not null)!;

        public AstSymbolTable Symbols => CodeBlock!.Symbols;

        private AstCodeBlock? _codeBlock;
        public AstCodeBlock? CodeBlock => _codeBlock;

        public bool HasExports
            => _codeBlock!.Symbols.Entries.Any(e => e.SymbolLocality == AstSymbolLocality.Exported);

        public bool TrySetCodeBlock(AstCodeBlock? codeBlock)
            => this.SafeSetParent(ref _codeBlock, codeBlock);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitFile(this);

        public override void VisitChildren(AstVisitor visitor)
            => CodeBlock?.Accept(visitor);
    }
}