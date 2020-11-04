using System;
using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFile : AstNode, IAstSymbolTableSite, IAstCodeBlockSite
    {
        private readonly List<AstFunctionDefinitionImpl> _functions = new List<AstFunctionDefinitionImpl>();

        public AstFile(string scopeName, AstSymbolTable parentTable, FileContext context)
            : base(AstNodeType.File)
        {
            Context = context;
            SetCodeBlock(new AstCodeBlock(scopeName, parentTable, null));
        }

        public FileContext Context { get; }

        public IEnumerable<AstFunctionDefinitionImpl> Functions => _functions;

        public AstSymbolTable Symbols => CodeBlock!.Symbols;

        private AstCodeBlock? _codeBlock;
        public AstCodeBlock? CodeBlock => _codeBlock;

        public bool TrySetCodeBlock(AstCodeBlock codeBlock) => this.SafeSetParent(ref _codeBlock, codeBlock);

        public void SetCodeBlock(AstCodeBlock codeBlock)
        {
            if (!TrySetCodeBlock(codeBlock))
                throw new InvalidOperationException(
                    "CodeBlock is already set or null.");
        }

        public override void Accept(AstVisitor visitor) => visitor.VisitFile(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var ci in CodeBlock!.Items)
            {
                ci.Accept(visitor);
            }
        }

        public void AddFunction(AstFunctionDefinitionImpl function)
        {
            CodeBlock!.AddItem(function);
            _functions.Add(function);
        }
    }
}