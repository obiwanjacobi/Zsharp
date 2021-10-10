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
                throw new InternalErrorException(
                    "Name is already set.");
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

        public AstSymbol AddSymbol(string canonicalName, AstSymbolKind kind, AstNode? node = null)
            => AddSymbol(AstSymbolName.Parse(canonicalName, AstSymbolNameParseOptions.IsCanonical), kind, node);

        public AstSymbol AddSymbol(AstSymbolName symbolName, AstSymbolKind kind, AstNode? node = null)
        {
            Ast.Guard(symbolName.IsCanonical, "All symbol names must be a canonical name.");

            // reference to a template parameter is detected as TypeReference
            if (kind == AstSymbolKind.Type &&
                node is AstTypeReferenceType typeRef &&
                typeRef.IsTemplateParameter)
                kind = AstSymbolKind.TemplateParameter;

            var exOrImported = FindSymbol(symbolName.FullName, AstSymbolKind.NotSet);
            var symbol = FindSymbolLocal(symbolName.FullName, kind);

            if (symbol is null)
            {
                symbol = new AstSymbol(this, symbolName.FullName, kind);
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
            var name = identifierSite.Identifier?.SymbolName.ToCanonical()
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

            var symbolName = AstSymbolName.Parse(symbol.SymbolName, AstSymbolNameParseOptions.IsCanonical);
            var childSymbol = symbol;
            var table = this;
            while (table is not null)
            {
                AstSymbol? parentSymbol;
                if (symbolName.IsDotName)
                    parentSymbol = FindSymbolByPath(symbolName, symbol.SymbolKind);
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

        public T? FindDefinition<T>(string symbolName, AstSymbolKind symbolKind)
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

        public AstSymbol? FindSymbol(string name, AstSymbolKind kind)
            => FindSymbol(AstSymbolName.Parse(name, AstSymbolNameParseOptions.IsCanonical), kind);

        public AstSymbol? FindSymbol(AstSymbolName symbolName, AstSymbolKind kind)
        {
            Ast.Guard(symbolName.IsCanonical, "Symbol names must be canonical names.");
            AstSymbol? symbol;

            if (symbolName.IsDotName)
                symbol = FindSymbolByPath(symbolName, kind);
            else
                symbol = FindSymbolRecursive(symbolName.FullName, kind);

            return symbol;
        }

        private AstSymbol? FindSymbolRecursive(string name, AstSymbolKind kind)
            => FindSymbolLocal(name, kind) ?? ParentTable?.FindSymbolRecursive(name, kind);

        private AstSymbol? FindSymbolLocal(string name, AstSymbolKind kind)
        {
            AstSymbol? symbol = null;
            var key = AstSymbol.MakeKey(name, kind);
            if (_table.ContainsKey(key))
            {
                symbol = _table[key];
            }
            else if (kind != AstSymbolKind.Unknown)
            {
                // check aliases
                symbol = _table.Values
                    .SingleOrDefault(e => e.Aliases.Contains(name) && e.SymbolKind == kind);
            }
            else
            {
                // by name only (kind is ignored)
                symbol = _table.Values
                    .SingleOrDefault(e => e.SymbolName == name || e.Aliases.Contains(name));
            }

            return symbol;
        }

        private AstSymbol? FindSymbolByPath(AstSymbolName symbolName, AstSymbolKind kind)
        {
            AstSymbol? symbol = null;
            var table = this;
            foreach (var namePart in symbolName.Parts)
            {
                var isLast = namePart == symbolName.Symbol;

                var kindPart = isLast ? kind : AstSymbolKind.Unknown;
                symbol = table.FindSymbolRecursive(namePart, kindPart);

                if (symbol is null)
                    return null;

                if (!isLast)
                {
                    var tableSite = symbol.DefinitionAs<IAstSymbolTableSite>();
                    if (tableSite is null)
                        return null;

                    table = tableSite.Symbols;
                }
            }
            return symbol;
        }

        private AstSymbol? FindSymbolInModulesLocal(string symbolName, AstSymbolKind symbolKind)
        {
            var moduleSymbolTables = _table.Values
                .Where(e => e.SymbolKind == AstSymbolKind.Module &&
                       e.SymbolLocality == AstSymbolLocality.Imported)
                .Select(e => e.DefinitionAs<AstModuleExternal>()!.Symbols);

            foreach (var symbolTable in moduleSymbolTables)
            {
                var symbol = symbolTable.FindSymbolLocal(symbolName, symbolKind);
                if (symbol is not null)
                    return symbol;
            }

            return null;
        }

        private AstSymbol? FindSymbolInModules(string symbolName, AstSymbolKind symbolKind)
        {
            var symbol = FindSymbolInModulesLocal(symbolName, symbolKind);

            if (symbol is null &&
                ParentTable is not null)
                return ParentTable.FindSymbolInModules(symbolName, symbolKind);

            return symbol;
        }

        internal void Delete(AstSymbol symbolEntry)
            => _table.Remove(symbolEntry.Key);
    }
}