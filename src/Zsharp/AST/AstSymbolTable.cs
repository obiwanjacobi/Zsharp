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

        public IEnumerable<AstSymbolEntry> Entries => _table.Values;

        public string Namespace
        {
            get
            {
                if (!String.IsNullOrEmpty(ParentTable?.Name))
                {
                    return ParentTable.Namespace + "." + Name;
                }
                return Name;
            }
        }

        public AstSymbolEntry AddSymbol(string symbolName, AstSymbolKind kind, AstNode? node = null)
        {
            var exOrImported = FindEntry(symbolName, AstSymbolKind.NotSet);
            var entry = FindEntry(symbolName, kind);

            if (entry == null)
            {
                entry = new AstSymbolEntry(this, symbolName, kind);
                if (exOrImported?.SymbolTable == this)
                {
                    Ast.Guard(_table.ContainsValue(exOrImported), "Exported/Imported Symbol is not defined in this SymbolTable.");

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
            foreach (var r in sourceEntry.References)
            {
                targetEntry.AddNode(r);
            }
        }

        public AstSymbolEntry? Resolve(AstSymbolEntry symbolEntry)
        {
            if (symbolEntry == null)
                return null;

            var table = this;
            while (table != null)
            {
                var entry = table.FindEntryLocal(symbolEntry.SymbolName, symbolEntry.SymbolKind);
                if (entry?.Definition != null)
                {
                    if (!Object.ReferenceEquals(entry, symbolEntry))
                        Merge(entry, symbolEntry);
                    return entry;
                }
                table = table.ParentTable;
            }
            return null;
        }

        public AstSymbolEntry? FindEntry(string name, AstSymbolKind kind = AstSymbolKind.NotSet)
        {
            var entry = FindEntryLocal(name, kind);

            if (ParentTable != null && entry == null)
            {
                entry = ParentTable.FindEntry(name, kind);
            }

            return entry;
        }

        public AstSymbolEntry? FindEntry(AstIdentifier identifier, AstSymbolKind kind = AstSymbolKind.NotSet)
        {
            return FindEntry(identifier.Name, kind);
        }

        public AstSymbolEntry? FindEntry(IAstIdentifierSite identifierSite, AstSymbolKind kind = AstSymbolKind.NotSet)
        {
            identifierSite.ThrowIfIdentifierNotSet();
            return FindEntry(identifierSite.Identifier!.Name, kind);
        }

        private AstSymbolEntry? FindEntryLocal(string name, AstSymbolKind kind)
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

            return entry;
        }
    }
}