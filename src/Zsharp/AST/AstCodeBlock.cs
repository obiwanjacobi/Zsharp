using System.Collections.Generic;
using static ZsharpParser;

namespace Zsharp.AST
{
    public class AstCodeBlock : AstNode, IAstSymbolTableSite
    {
        private readonly List<AstCodeBlockItem> _items = new List<AstCodeBlockItem>();

        public AstCodeBlock(string scopeName, AstSymbolTable parentTable, CodeblockContext? ctx)
            : base(AstNodeType.CodeBlock)
        {
            Symbols = new AstSymbolTable(scopeName, parentTable);
            Context = ctx;
        }

        public int Indent { get; set; }

        public CodeblockContext? Context { get; }

        public IEnumerable<AstCodeBlockItem> Items
        {
            get { return _items; }
        }

        public AstSymbolTable Symbols { get; }

        public AstSymbolEntry AddSymbol(string symbolName, AstSymbolKind kind, AstNode node)
        {
            return Symbols.AddSymbol(symbolName, kind, node);
        }

        public T? ItemAt<T>(int index)
            where T : AstCodeBlockItem
        {
            return _items[index] as T;
        }

        public bool AddItem(AstCodeBlockItem item)
        {
            if (item == null)
                return false;

            _items.Add(item);
            item.Indent = Indent;
            var success = item.SetParent(this);
            Ast.Guard(success, "SetParent failed.");
            return true;
        }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitCodeBlock(this);
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var item in Items)
            {
                item.Accept(visitor);
            }
        }
    }
}