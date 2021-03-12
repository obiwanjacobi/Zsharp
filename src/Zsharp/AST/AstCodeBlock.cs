using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstCodeBlock : AstNode, IAstSymbolTableSite
    {
        private readonly List<IAstCodeBlockItem> _items = new();

        public AstCodeBlock(string scopeName, AstSymbolTable parentTable, CodeblockContext? context = null)
            : base(AstNodeType.CodeBlock)
        {
            Symbols = new AstSymbolTable(scopeName, parentTable);
            Context = context;
        }

        public uint Indent { get; set; }

        public CodeblockContext? Context { get; }

        public IEnumerable<IAstCodeBlockItem> Items => _items;

        public AstSymbolTable Symbols { get; }

        public T? ItemAt<T>(int index) where T : AstNode, IAstCodeBlockItem
            => _items[index] as T;

        public void AddItem<T>(T item)
            where T : AstNode, IAstCodeBlockItem
        {
            _items.Add(item);
            item.Indent = Indent;
            item.SetParent(this);
        }

        public override void Accept(AstVisitor visitor) => visitor.VisitCodeBlock(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (AstNode item in Items)
            {
                item.Accept(visitor);
            }
        }
    }
}