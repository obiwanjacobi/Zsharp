using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstCodeBlock : AstNode, IAstSymbolTableSite
    {
        private readonly List<AstCodeBlockItem> _items = new List<AstCodeBlockItem>();

        public AstCodeBlock(string scopeName, AstSymbolTable parentTable, CodeblockContext? context)
            : base(AstNodeType.CodeBlock)
        {
            Symbols = new AstSymbolTable(scopeName, parentTable);
            Context = context;
        }

        public int Indent { get; set; }

        public CodeblockContext? Context { get; }

        public IEnumerable<AstCodeBlockItem> Items => _items;

        public AstSymbolTable Symbols { get; }

        public T? ItemAt<T>(int index) where T : AstCodeBlockItem
            => _items[index] as T;

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

        public override void Accept(AstVisitor visitor) => visitor.VisitCodeBlock(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var item in Items)
            {
                item.Accept(visitor);
            }
        }
    }
}