using System;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public class AstSymbolTable
    {
        private readonly Dictionary<string, AstSymbolEntry> _table = new();

        public AstSymbolTable(string name = "", AstSymbolTable? parentTable = null)
        {
            Name = name;
            ParentTable = parentTable;
        }

        public string Name { get; private set; }

        public void SetName(string name)
        {
            if (!String.IsNullOrEmpty(Name))
                throw new InternalErrorException(
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
                    return $"{ParentTable.Namespace}.{Name}";
                return Name;
            }
        }

        public AstSymbolEntry AddSymbol(string canonicalName, AstSymbolKind kind, params AstNode?[]? nodes)
            => AddSymbol(AstSymbolName.Parse(canonicalName, AstSymbolNameParseOptions.IsCanonical), kind, nodes);

        public AstSymbolEntry AddSymbol(AstSymbolName symbolName, AstSymbolKind kind, params AstNode?[]? nodes)
        {
            Ast.Guard(symbolName.IsCanonical, "All symbol names must be a canonical name.");
            var exOrImported = FindEntry(symbolName, AstSymbolKind.NotSet);
            var entry = FindEntryLocal(symbolName.FullName, kind);

            if (entry is null)
            {
                entry = new AstSymbolEntry(this, symbolName.FullName, kind);
                if (exOrImported?.SymbolTable == this)
                {
                    Ast.Guard(_table.ContainsValue(exOrImported), "Exported/Imported Symbol is not defined in this SymbolTable.");

                    _table.Remove(exOrImported.Key);
                    Merge(entry, exOrImported);
                }
                _table[entry.Key] = entry;
            }

            if (nodes is not null)
            {
                foreach (var node in nodes)
                {
                    if (node is not null)
                        entry.AddNode(node);
                }
            }
            return entry;
        }

        private static void Merge(AstSymbolEntry targetEntry, AstSymbolEntry sourceEntry)
        {
            // only allow from private up 
            // not from exported to private
            if (targetEntry.SymbolLocality == AstSymbolLocality.Private)
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

        public AstSymbolEntry? ResolveDefinition(AstSymbolEntry symbolEntry)
        {
            if (symbolEntry is null)
                return null;
            if (symbolEntry.HasOverloads || symbolEntry.Definition is not null)
                return symbolEntry;

            var symbolName = AstSymbolName.Parse(symbolEntry.SymbolName, AstSymbolNameParseOptions.IsCanonical);
            var table = this;
            while (table is not null)
            {
                var entry = table.FindEntryLocal(symbolEntry.SymbolName, symbolEntry.SymbolKind);
                if (!HasDefinition(entry) && symbolName.IsDotName)
                    entry = table.FindEntry(symbolName, symbolEntry.SymbolKind);
                if (!HasDefinition(entry))
                    entry = table.FindEntryInModules(symbolEntry.SymbolName, symbolEntry.SymbolKind);

                if (HasDefinition(entry))
                {
                    Merge(entry!, symbolEntry);
                    symbolEntry.Delete();
                    return entry;
                }
                table = table.ParentTable;
            }
            return null;
        }

        private static bool HasDefinition(AstSymbolEntry? entry)
            => entry?.HasDefinition ?? false;

        public T? FindDefinition<T>(string symbolName, AstSymbolKind symbolKind)
            where T : class
        {
            var entry = FindEntryLocal(symbolName, symbolKind);
            var symbolDef = entry?.DefinitionAs<T>();

            if (symbolDef is null)
            {
                entry = FindEntryInModules(symbolName, symbolKind);
                symbolDef = entry?.DefinitionAs<T>();
            }

            if (symbolDef is null)
                symbolDef = ParentTable?.FindDefinition<T>(symbolName, symbolKind);

            return symbolDef;
        }
        public IEnumerable<AstSymbolEntry> FindEntries(AstSymbolKind symbolKind)
            => _table.Values.Where(s => s.SymbolKind == symbolKind);

        public AstSymbolEntry? FindEntry(string name, AstSymbolKind kind = AstSymbolKind.NotSet)
            => FindEntry(AstSymbolName.Parse(name, AstSymbolNameParseOptions.IsCanonical), kind);

        public AstSymbolEntry? FindEntry(AstSymbolName symbolName, AstSymbolKind kind = AstSymbolKind.NotSet)
        {
            Ast.Guard(symbolName.IsCanonical, "All symbol names must be a canonical name.");
            AstSymbolEntry? entry;

            if (symbolName.IsDotName)
                entry = FindEntryByPath(symbolName, kind);
            else
                entry = FindEntryRecursive(symbolName.FullName, kind);

            return entry;
        }

        private AstSymbolEntry? FindEntryRecursive(string name, AstSymbolKind kind)
        {
            var entry = FindEntryLocal(name, kind);

            if (ParentTable is not null &&
                entry is null)
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

        private AstSymbolEntry? FindEntryByPath(AstSymbolName symbolName, AstSymbolKind kind)
        {
            AstSymbolEntry? entry = null;
            var table = this;
            foreach (var namePart in symbolName)
            {
                var isLast = namePart == symbolName.Symbol;

                var kindPart = isLast ? kind : AstSymbolKind.Unknown;
                entry = table.FindEntryRecursive(namePart, kindPart);

                if (entry is null)
                    return null;

                if (!isLast)
                {
                    var tableSite = entry.DefinitionAs<IAstSymbolTableSite>();
                    if (tableSite is null)
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
                if (entry is not null)
                    return entry;
            }
            return null;
        }

        internal void Delete(AstSymbolEntry symbolEntry)
            => _table.Remove(symbolEntry.Key);
    }
}