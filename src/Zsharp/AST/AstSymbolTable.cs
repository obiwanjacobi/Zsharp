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
                {
                    return $"{ParentTable.Namespace}.{Name}";
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
            if (symbolEntry == null)
                return null;
            if (symbolEntry.HasOverloads || symbolEntry.Definition != null)
                return symbolEntry;

            var dotName = new AstDotName(symbolEntry.SymbolName);
            var table = this;
            while (table != null)
            {
                var entry = table.FindEntryLocal(symbolEntry.SymbolName, symbolEntry.SymbolKind);
                if (!HasDefinition(entry) && dotName.IsDotName)
                    entry = table.FindEntryDotName(dotName, symbolEntry.SymbolKind);
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

            if (ParentTable != null &&
                symbolDef == null)
                symbolDef = ParentTable.FindDefinition<T>(symbolName, symbolKind);

            return symbolDef;
        }

        public AstSymbolEntry? FindEntry(string name, AstSymbolKind kind = AstSymbolKind.NotSet)
        {
            var dotName = new AstDotName(name);
            AstSymbolEntry? entry;

            if (dotName.IsDotName)
                entry = FindEntryDotName(dotName, kind);
            else
                entry = FindEntryRecursive(name, kind);

            return entry;
        }

        public IEnumerable<AstSymbolEntry> FindEntries(AstSymbolKind symbolKind)
            => _table.Values.Where(s => s.SymbolKind == symbolKind);

        internal void Delete(AstSymbolEntry symbolEntry)
            => _table.Remove(symbolEntry.Key);

        private AstSymbolEntry? FindEntryRecursive(string name, AstSymbolKind kind)
        {
            var entry = FindEntryLocal(name, kind);

            if (ParentTable != null &&
                entry == null)
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