using System;
using System.Collections.Generic;

namespace Zsharp.AST
{
    public class AstSymbolTable
    {
        private readonly Dictionary<string, AstSymbolEntry> _table = new Dictionary<string, AstSymbolEntry>();

        public AstSymbolTable()
        {
            Name = String.Empty;
        }

        public AstSymbolTable(string name)
        {
            Name = name;
        }

        public AstSymbolTable(AstSymbolTable parentTable)
        {
            Name = String.Empty;
            ParentTable = parentTable;
        }

        public AstSymbolTable(string name, AstSymbolTable parentTable)
        {
            Name = name;
            ParentTable = parentTable;
        }

        public string Name { get; }

        public AstSymbolTable? ParentTable { get; }

        public AstSymbolEntry AddSymbol(string symbolName, AstSymbolKind kind, AstNode? node)
        {
            var entry = GetEntry(symbolName, kind);
            if (entry == null)
            {
                entry = new AstSymbolEntry(symbolName, kind);
                _table[entry.Key] = entry;
            }

            if (node != null)
            {
                entry.AddNode(node);
            }
            return entry;
        }

        public AstSymbolEntry? GetEntry(string name, AstSymbolKind kind)
        {
            if (name.Contains('.'))
            {
                throw new ArgumentException("Parsing dot-names is not implemented yet.", nameof(name));
            }

            var key = AstSymbolEntry.MakeKey(name, kind);
            if (_table.ContainsKey(key))
            {
                return _table[key];
            }
            if (ParentTable != null)
            {
                return ParentTable.GetEntry(name, kind);
            }
            return null;
        }

        public IEnumerable<AstSymbolEntry> Entries => _table.Values;

        public string Namespace
        {
            get
            {
                if (ParentTable != null)
                {
                    return ParentTable.Namespace + "." + Name;
                }
                return Name;
            }
        }
    }
}