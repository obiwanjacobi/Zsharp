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

        public string Name { get; private set; }

        public void SetName(string name)
        {
            if (!String.IsNullOrEmpty(Name))
                throw new InvalidOperationException(
                    "Name is already set");
            Name = name;
        }

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

        public AstSymbolEntry AddSymbol(string symbolName, AstSymbolKind kind, params AstNode?[]? nodes)
        {
            var exOrImported = FindEntry(symbolName, AstSymbolKind.NotSet);
            var entry = FindEntryLocal(symbolName, kind);

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

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    if (node != null)
                        entry.AddNode(node);
                }
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
            if (symbolEntry.HasOverloads || symbolEntry.Definition != null)
                return symbolEntry;

            var table = this;
            while (table != null)
            {
                var entry = table.FindEntryLocal(symbolEntry.SymbolName, symbolEntry.SymbolKind);
                if (entry == null)
                    entry = table.FindEntryInModules(symbolEntry.SymbolName, symbolEntry.SymbolKind);

                if (entry != null &&
                    (entry.HasOverloads || entry.Definition != null))
                {
                    Merge(entry, symbolEntry);
                    symbolEntry.Delete();
                    return entry;
                }
                table = table.ParentTable;
            }
            return null;
        }

        public AstSymbolEntry? FindEntry(string name, AstSymbolKind kind = AstSymbolKind.NotSet)
        {
            var entry = FindEntryRecursive(name, kind);

            if (entry == null)
            {
                var dotName = new AstDotName(name);
                if (dotName.IsDotName)
                {
                    entry = FindEntryDotName(dotName, kind);
                }
            }
            return entry;
        }

        public AstSymbolEntry? FindEntry(AstIdentifier identifier, AstSymbolKind kind = AstSymbolKind.NotSet)
            => FindEntry(identifier.CanonicalName, kind);

        public AstSymbolEntry? FindEntry(IAstIdentifierSite identifierSite, AstSymbolKind kind = AstSymbolKind.NotSet)
        {
            identifierSite.ThrowIfIdentifierNotSet();
            return FindEntry(identifierSite.Identifier!.CanonicalName, kind);
        }

        internal void Delete(AstSymbolEntry symbolEntry)
        {
            _table.Remove(symbolEntry.Key);
        }

        private AstSymbolEntry? FindEntryRecursive(string name, AstSymbolKind kind)
        {
            var entry = FindEntryLocal(name, kind);

            if (ParentTable != null && entry == null)
            {
                entry = ParentTable.FindEntry(name, kind);
            }

            return entry;
        }

        private AstSymbolEntry? FindEntryLocal(string name, AstSymbolKind kind)
        {
            AstSymbolEntry? entry = null;
            var key = AstSymbolEntry.MakeKey(name, kind);
            if (_table.ContainsKey(key))
            {
                entry = _table[key];
            }
            else if (kind != AstSymbolKind.Unknown)
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

        private AstSymbolEntry? FindEntryDotName(AstDotName dotName, AstSymbolKind kind)
        {
            AstSymbolEntry? entry = null;
            var table = this;
            foreach (var namePart in dotName)
            {
                var isLast = namePart == dotName.Symbol;

                var kindPart = isLast ? kind : AstSymbolKind.Unknown;
                entry = table.FindEntryRecursive(namePart, kindPart);

                if (entry == null)
                    return null;

                if (!isLast)
                {
                    var tableSite = entry.DefinitionAs<IAstSymbolTableSite>();
                    if (tableSite == null)
                        return null;

                    table = tableSite.Symbols;
                }
            }
            return entry;
        }

        private AstSymbolEntry? FindEntryInModules(string symbolName, AstSymbolKind symbolKind)
        {
            var moduleSymbols = _table.Values
                .Where(e => e.SymbolKind == AstSymbolKind.Module &&
                       e.SymbolLocality == AstSymbolLocality.Imported)
                .Select(e => e.DefinitionAs<AstModuleExternal>()!.Symbols);

            foreach (var symbols in moduleSymbols)
            {
                var entry = symbols.FindEntryLocal(symbolName, symbolKind);
                if (entry != null)
                    return entry;
            }
            return null;
        }
    }
}