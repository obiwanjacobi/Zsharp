using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Zsharp.AST
{
    [DebuggerDisplay("{Namespace} ({SymbolCount})")]
    public class AstSymbolTable
    {
        private readonly Dictionary<string, AstSymbol> _table = new();

        public AstSymbolTable(string name = "", AstSymbolTable? parentTable = null)
        {
            Name = name;
            ParentTable = parentTable;
        }

        public string Name { get; private set; }

        public void SetName(string name)
        {
            if (!String.IsNullOrEmpty(Name))
                throw new InternalErrorException("Name is already set.");
            Name = name;
        }

        public AstSymbolTable? ParentTable { get; }

        public int SymbolCount => _table.Count;
        public IEnumerable<AstSymbol> Symbols => _table.Values;

        public string Namespace
        {
            get
            {
                if (!String.IsNullOrEmpty(ParentTable?.Name))
                    return $"{ParentTable.Namespace}.{Name}";
                return Name;
            }
        }

        public AstSymbol AddSymbol(string symbolName, AstSymbolKind kind, AstNode? node = null)
            => AddSymbol(AstName.ParseFullName(symbolName, AstNameKind.Canonical), kind, node);

        public AstSymbol AddSymbol(AstName symbolName, AstSymbolKind kind, AstNode? node = null)
        {
            // reference to a template parameter is detected as TypeReference
            if (kind == AstSymbolKind.Type &&
                node is AstTypeReferenceType typeRef &&
                typeRef.IsTemplateParameter)
                kind = AstSymbolKind.TemplateParameter;

            var symbol = FindSymbolLocal(symbolName, kind);

            if (symbol is null)
            {
                symbol = new AstSymbol(this, symbolName, kind);

                var exOrImported = FindSymbol(symbolName, AstSymbolKind.NotSet);
                if (exOrImported is not null)
                {
                    _table.Remove(exOrImported.Key);
                    Merge(symbol, exOrImported);
                }
                _table[symbol.Key] = symbol;
            }

            if (node is not null)
            {
                symbol.AddNode(node);

                if (node is IAstSymbolSite symbolSite &&
                    symbolSite.Symbol is null)
                    symbolSite.SetSymbol(symbol);

                if (node is IAstExternalNameSite externalName)
                    symbol.Namespace = externalName.ExternalName.Namespace;
            }

            return symbol;
        }

        public AstSymbol AddSymbol(IAstIdentifierSite identifierSite, AstSymbolKind kind, AstNode node)
        {
            var name = identifierSite.Identifier.SymbolName.CanonicalName.FullName
                ?? throw new ArgumentException("No identifier name.", nameof(identifierSite));

            return AddSymbol(name, kind, node);
        }

        private static void Merge(AstSymbol targetEntry, AstSymbol sourceEntry)
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

        public bool TryResolveDefinition(AstSymbol symbol)
        {
            Ast.Guard(symbol, "Symbol is null.");
            if (symbol.HasOverloads || symbol.Definition is not null)
                return true;

            var childSymbol = symbol;
            var table = this;
            while (table is not null)
            {
                AstSymbol? parentSymbol;
                if (symbol.SymbolName.IsDotName)
                    parentSymbol = FindSymbolByPath(symbol.SymbolName, symbol.SymbolKind);
                else
                    parentSymbol = table.FindSymbolLocal(symbol.SymbolName, symbol.SymbolKind);
                // FindSymbolLocal may find itself (symbol)
                if (parentSymbol is null || parentSymbol == symbol)
                    parentSymbol = table.FindSymbolInModulesLocal(symbol.SymbolName, symbol.SymbolKind);

                if (parentSymbol is not null)
                {
                    if (childSymbol.Parent is null)
                        childSymbol.SetParent(parentSymbol!);
                    else
                        return true;

                    if (parentSymbol.HasDefinition)
                        return true;

                    childSymbol = parentSymbol;
                }

                table = table.ParentTable;
            }
            return false;
        }

        public T? FindDefinition<T>(string symbolName, AstSymbolKind symbolKind) where T : class
            => FindDefinition<T>(AstName.ParseFullName(symbolName), symbolKind);

        public T? FindDefinition<T>(AstName symbolName, AstSymbolKind symbolKind)
            where T : class
        {
            var symbol = FindSymbolLocal(symbolName, symbolKind);
            var symbolDef = symbol?.DefinitionAs<T>();

            if (symbolDef is null)
            {
                symbol = FindSymbolInModules(symbolName, symbolKind);
                symbolDef = symbol?.DefinitionAs<T>();
            }

            if (symbolDef is null)
                symbolDef = ParentTable?.FindDefinition<T>(symbolName, symbolKind);

            return symbolDef;
        }

        public IEnumerable<AstSymbol> FindSymbols(AstSymbolKind symbolKind)
            => _table.Values.Where(s => s.SymbolKind == symbolKind);

        public AstSymbol? FindSymbol(string symbolName, AstSymbolKind kind)
            => FindSymbol(AstName.ParseFullName(symbolName, AstNameKind.Canonical), kind);

        public AstSymbol? FindSymbol(AstName symbolName, AstSymbolKind kind)
        {
            Ast.Guard(symbolName.Kind == AstNameKind.Canonical, "Symbol names must be canonical names.");
            AstSymbol? symbol;

            if (symbolName.IsDotName)
                symbol = FindSymbolByPath(symbolName, kind);
            else
                symbol = FindSymbolRecursive(symbolName, kind);

            return symbol;
        }

        private AstSymbol? FindSymbolRecursive(AstName name, AstSymbolKind kind)
            => FindSymbolLocal(name, kind) ?? ParentTable?.FindSymbolRecursive(name, kind);

        private AstSymbol? FindSymbolLocal(AstName name, AstSymbolKind kind)
        {
            AstSymbol? symbol = null;
            var key = AstSymbol.MakeKey(name.Symbol, kind, name.GetArgumentCount());
            if (_table.ContainsKey(key))
            {
                symbol = _table[key];
            }
            else if (kind != AstSymbolKind.Unknown)
            {
                // check aliases
                symbol = _table.Values
                    .SingleOrDefault(e => e.Aliases.Contains(name.Name) && e.SymbolKind == kind);
            }
            else
            {
                // by name only (kind is ignored)
                symbol = _table.Values
                    .SingleOrDefault(e => e.SymbolName.Name == name.Name || e.Aliases.Contains(name.Name));
            }

            return symbol;
        }

        private AstSymbol? FindSymbolByPath(AstName symbolName, AstSymbolKind kind)
        {
            AstSymbol? symbol = null;
            if (Namespace == symbolName.Namespace || String.IsNullOrEmpty(symbolName.Namespace))
                symbol = FindSymbolLocal(symbolName, kind);
            if (symbol is not null)
                return symbol;
            symbol = FindSymbolInModules(symbolName, kind);
            if (symbol is not null)
                return symbol;
            
            var index = 0;
            var parts = symbolName.Parts.ToList();
            var symbolTable = FindParentSymbolTableRecursiveByNamespace(parts[index++]);
            if (symbolTable is null)
            {
                index = 0;
                symbolTable = this;
            }
            for (int i = index; i < parts.Count; i++)
            {
                var part = parts[i];
                var partName = AstName.FromParts(new[] { part }, symbolName.Kind);
                symbol = symbolTable.FindSymbolLocal(partName, i == parts.Count - 1 ? kind : AstSymbolKind.Unknown);
                if (symbol is not null)
                {
                    var tableSite = symbol.DefinitionAs<IAstSymbolTableSite>();
                    if (tableSite is not null)
                        symbolTable = tableSite.Symbols;
                }
                
                var table = FindChildSymbolTableByNamespace(part);
                if (table is not null)
                    symbolTable = table;

                // dead end
                if (symbol is null && table is null)
                    return null;
            }

            return symbol;
        }

        private AstSymbol? FindSymbolInModulesLocal(AstName symbolName, AstSymbolKind symbolKind)
        {
            var moduleSymbolTables = _table.Values
                .Where(e => e.SymbolKind == AstSymbolKind.Module &&
                       e.SymbolLocality == AstSymbolLocality.Imported)
                .Select(e => e.DefinitionAs<AstModuleExternal>()!.Symbols)
                .Where(t => t.Namespace == symbolName.Namespace || String.IsNullOrEmpty(symbolName.Namespace));

            foreach (var symbolTable in moduleSymbolTables)
            {
                var symbol = symbolTable.FindSymbolLocal(symbolName, symbolKind);
                if (symbol is not null)
                    return symbol;
            }

            return null;
        }

        private AstSymbol? FindSymbolInModules(AstName symbolName, AstSymbolKind symbolKind)
        {
            var symbol = FindSymbolInModulesLocal(symbolName, symbolKind);

            if (symbol is null)
                return ParentTable?.FindSymbolInModules(symbolName, symbolKind);

            return symbol;
        }

        private AstSymbolTable? FindParentSymbolTableRecursiveByNamespace(string @namespace)
        {
            var table = this;
            while (table is not null)
            {
                if (table.Namespace == @namespace)
                    return table;

                table = table.ParentTable;
            }
            return null;
        }

        private AstSymbolTable? FindChildSymbolTableByNamespace(string @namespace)
        {
            var tables = _table.Values
                .Select(s => s.DefinitionAs<IAstSymbolTableSite>()?.Symbols)
                .Where(t => t is not null && t.Namespace == @namespace);

            return tables.SingleOrDefault();
        }
    }
}