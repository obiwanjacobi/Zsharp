using System;
using System.Collections.Generic;

namespace Zsharp.AST
{
    public class AstSymbolTable
    {
        private readonly Dictionary<string, AstSymbolEntry> _table = new Dictionary<string, AstSymbolEntry>();
        private readonly List<AstSymbolTable> _symbolTables = new List<AstSymbolTable>();

        public AstSymbolTable(string name = "")
        {
            Name = name;
        }

        public AstSymbolTable(string name, AstSymbolTable parentTable)
        {
            Ast.Guard(parentTable, "Parent SymbolTable is null.");

            Name = name;
            ParentTable = parentTable;
        }

        public string Name { get; }

        public AstSymbolTable? ParentTable { get; }

        public AstSymbolEntry AddSymbol(string symbolName, AstSymbolKind kind, AstNode? node = null)
        {
            var exOrImported = FindEntry(symbolName, AstSymbolKind.NotSet);
            var entry = FindEntry(symbolName, kind);

            if (entry == null)
            {
                entry = new AstSymbolEntry(symbolName, kind);
                if (exOrImported != null)
                {
                    _table.Remove(exOrImported.Key);
                    entry.SymbolLocality = exOrImported.SymbolLocality;
                }
                _table[entry.Key] = entry;
            }

            if (node != null)
            {
                entry.AddNode(node);
            }

            if (node is AstModuleExternal externalModule)
            {
                Ast.Guard(kind == AstSymbolKind.Module, $"Adding an External Module as a {kind}.");
                // register external table at this 'level'
                _symbolTables.Add(externalModule.Symbols);
            }

            return entry;
        }

        public AstSymbolEntry? FindEntry(string name, AstSymbolKind kind)
        {
            if (name.Contains('.'))
            {
                throw new ArgumentException("Parsing dot-names is not implemented yet.", nameof(name));
            }

            AstSymbolEntry? entry = null;
            var key = AstSymbolEntry.MakeKey(name, kind);
            if (_table.ContainsKey(key))
            {
                entry = _table[key];
            }
            else
            {
                foreach (var table in _symbolTables)
                {
                    entry = table.FindEntry(name, kind);
                    if (entry != null)
                        break;
                }
            }

            if (ParentTable != null && entry == null)
            {
                entry = ParentTable.FindEntry(name, kind);
            }

            return entry;
        }

        public AstSymbolEntry? FindEntry(AstIdentifier identifier, AstSymbolKind kind)
        {
            return FindEntry(identifier.Name, kind);
        }

        public AstSymbolEntry? FindEntry(IAstIdentifierSite identifierSite, AstSymbolKind kind)
        {
            identifierSite.ThrowIfIdentifierNotSet();
            return FindEntry(identifierSite.Identifier!.Name, kind);
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