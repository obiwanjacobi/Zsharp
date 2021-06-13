using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstCodeBlock : AstNode,
        IAstSymbolTableSite
    {
        public AstCodeBlock(string scopeName, AstSymbolTable parentTable, CodeblockContext? context = null)
            : base(AstNodeKind.CodeBlock)
        {
            Symbols = new AstSymbolTable(scopeName, parentTable);
            Context = context;
        }

        public uint Indent { get; set; }

        public CodeblockContext? Context { get; }

        public AstSymbolTable Symbols { get; }

        private readonly List<IAstCodeBlockLine> _lines = new();
        public IEnumerable<IAstCodeBlockLine> Lines => _lines;

        public T? LineAt<T>(int index) where T : AstNode, IAstCodeBlockLine
            => _lines[index] as T;

        public void AddLine<T>(T item)
            where T : AstNode, IAstCodeBlockLine
        {
            _lines.Add(item);
            item.Indent = Indent;
            item.SetParent(this);
        }

        public override void Accept(AstVisitor visitor) => visitor.VisitCodeBlock(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (AstNode item in Lines)
            {
                item.Accept(visitor);
            }
        }
    }
}