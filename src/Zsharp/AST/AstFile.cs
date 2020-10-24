using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFile : AstNode, IAstSymbolTableSite, IAstCodeBlockSite
    {
        private readonly List<Statement_importContext> _imports = new List<Statement_importContext>();
        private readonly List<Statement_exportContext> _exports = new List<Statement_exportContext>();
        private readonly List<AstFunction> _functions = new List<AstFunction>();

        public AstFile(string scopeName, AstSymbolTable parentTable, FileContext context)
            : base(AstNodeType.File)
        {
            Context = context;
            TrySetCodeBlock(new AstCodeBlock(scopeName, parentTable, null));
        }

        public FileContext Context { get; }

        public IEnumerable<Statement_importContext> Imports => _imports;

        public IEnumerable<Statement_exportContext> Exports => _exports;

        public IEnumerable<AstFunction> Functions => _functions;

        public AstSymbolTable Symbols => CodeBlock!.Symbols;

        private AstCodeBlock? _codeBlock;
        public AstCodeBlock? CodeBlock => _codeBlock;

        public bool TrySetCodeBlock(AstCodeBlock codeBlock) => this.SafeSetParent(ref _codeBlock, codeBlock);

        public override void Accept(AstVisitor visitor) => visitor.VisitFile(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var ci in CodeBlock!.Items)
            {
                ci.Accept(visitor);
            }
        }

        public bool AddImport(Statement_importContext importCtx)
        {
            if (importCtx != null)
            {
                _imports.Add(importCtx);
                return true;
            }
            return false;
        }

        public bool AddExport(Statement_exportContext exportCtx)
        {
            if (exportCtx != null)
            {
                _exports.Add(exportCtx);
                return true;
            }
            return false;
        }

        public bool AddFunction(AstFunction function)
        {
            if (CodeBlock!.AddItem(function))
            {
                _functions.Add(function);
                return true;
            }
            return false;
        }
    }
}