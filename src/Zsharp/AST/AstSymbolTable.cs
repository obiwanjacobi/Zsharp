using System;
using System.Collections.Generic;
using System.Linq;

namespace Zlang.NET.AST
{
    public enum AstSymbolKind
    {
        NotSet,
        Function,
        Struct,
        Enum,
        Type,
        Parameter,
        Variable,
        Field,
    };

    public enum AstSymbolLocality
    {
        Private,
        Exported,
        Imported,
    };

    public interface IAstSymbolTableSite
    {
        AstSymbolTable Symbols { get; }
        AstSymbolEntry AddSymbol(string symbolName, AstSymbolKind kind, AstNode node)
        {
            return Symbols.AddSymbol(symbolName, kind, node);
        }

        AstSymbolEntry AddSymbol(AstIdentifier identifier, AstSymbolKind kind, AstNode node)
        {
            return Symbols.AddSymbol(identifier.Name, kind, node);
        }

        AstSymbolEntry AddSymbol(IAstIdentifierSite identifierSite, AstSymbolKind kind, AstNode node)
        {
            if (identifierSite?.Identifier == null)
                throw new ArgumentNullException(nameof(identifierSite));
            return Symbols.AddSymbol(identifierSite.Identifier.Name, kind, node);
        }
    }

    public class AstSymbolEntry
    {
        private readonly List<AstNode> _references = new List<AstNode>();

        public AstSymbolEntry(string symbolName, AstSymbolKind symbolKind)
        {
            SymbolName = symbolName;
            SymbolKind = symbolKind;
        }

        public IEnumerable<AstNode> References => _references;
        public IEnumerable<T> ReferencesOf<T>() where T : AstNode => _references.OfType<T>();

        public string Key { get { return MakeKey(SymbolName, SymbolKind); } }
        public string SymbolName { get; }
        public AstSymbolKind SymbolKind { get; }
        public AstSymbolLocality SymbolLocality { get; set; }

        private AstNode? _definition;
        public AstNode? Definition => _definition;
        public T? GetDefinition<T>()
            where T : AstNode
        {
            return _definition as T;
        }

        public void AddNode(AstNode node)
        {
            Ast.Guard(node, "Cannot add null.");

            if ((SymbolKind == AstSymbolKind.Function && node is AstFunction) ||
                (SymbolKind == AstSymbolKind.Parameter && node is AstFunctionParameter) ||
                (SymbolKind == AstSymbolKind.Variable && node is AstVariableDefinition) ||
                (SymbolKind == AstSymbolKind.Type && node is AstTypeDefinition)
                // (SymbolKind == AstSymbolKind.Struct && node is AstStruct) ||
                // (SymbolKind == AstSymbolKind.Enum && node is AstEnum) ||
                // (SymbolKind == AstSymbolKind.Field && node is AstField)
                )
            {
                Ast.Guard(_definition == null, "Definition is already set.");
                _definition = node;
            }
            else
            {
                _references.Add(node);
            }
        }

        internal static string MakeKey(string name, AstSymbolKind kind) => name + kind;
    }

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
                throw new ArgumentException(nameof(name), "Parsing dot-names is not implemented yet.");
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