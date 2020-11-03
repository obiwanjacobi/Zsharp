using System;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public class AstSymbolTable
    {
        private readonly Dictionary<string, AstSymbolEntry> _table = new Dictionary<string, AstSymbolEntry>();

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
                    Merge(entry, exOrImported);
                }
                _table[entry.Key] = entry;
            }

            if (node != null)
            {
                entry.AddNode(node);
            }

            return entry;
        }

        private void Merge(AstSymbolEntry targetEntry, AstSymbolEntry sourceEntry)
        {
            targetEntry.SymbolLocality = sourceEntry.SymbolLocality;
            foreach (var alias in sourceEntry.Aliases)
            {
                targetEntry.TryAddAlias(alias);
            }
        }

        public AstSymbolEntry? FindEntry(string name, AstSymbolKind kind = AstSymbolKind.NotSet)
        {
            AstSymbolEntry? entry = null;
            var key = AstSymbolEntry.MakeKey(name, kind);
            if (_table.ContainsKey(key))
            {
                entry = _table[key];
            }
            else if (kind != AstSymbolKind.NotSet)
            {
                // check aliases
                entry = _table.Values
                    .SingleOrDefault(e => e.Aliases.Contains(name) && e.SymbolKind == kind);
            }
            else
            {
                // by name only (kind is ignored)
                entry = _table.Values
                    .SingleOrDefault(e => e.SymbolName == name || e.Aliases.Contains(name));
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
                if (ParentTable != null &&
                    !String.IsNullOrEmpty(ParentTable.Name))
                {
                    return ParentTable.Namespace + "." + Name;
                }
                return Name;
            }
        }
    }
}