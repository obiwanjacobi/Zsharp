using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Zsharp.AST
{
    [DebuggerDisplay("{SymbolLocality} {SymbolName} ({SymbolKind})")]
    public class AstSymbol
    {
        private readonly List<AstNode> _definitions = new();
        private readonly List<AstNode> _references = new();
        private readonly List<string> _aliases = new();

        public AstSymbol(AstSymbolTable symbolTable, string symbolName, AstSymbolKind symbolKind)
        {
            SymbolTable = symbolTable;
            SymbolName = symbolName;
            SymbolKind = symbolKind;
        }

        public AstSymbolTable SymbolTable { get; }

        private AstSymbol? _parent;
        public AstSymbol? Parent => _parent;

        public bool TrySetParent(AstSymbol parent)
        {
            Ast.Guard(parent != this, "Cannot add this as parent Symbol.");

            if (_parent is null && parent is not null &&
                parent.TryAddChild(this))
            {
                _parent = parent;
                return true;
            }
            return false;
        }

        public void SetParent(AstSymbol parent)
        {
            if (!TrySetParent(parent))
                throw new InternalErrorException(
                    "Symbol parent is already set or null.");
        }

        private readonly List<AstSymbol> _children = new();
        public IEnumerable<AstSymbol> Children => _children;

        public bool TryAddChild(AstSymbol symbol)
        {
            if (symbol is not null &&
                !_children.Contains(symbol))
            {
                _children.Add(symbol);
                return true;
            }
            return false;
        }

        public void AddChild(AstSymbol symbol)
        {
            if (!TryAddChild(symbol))
                throw new InternalErrorException(
                    "Symbol child is already added or null.");
        }

        public IEnumerable<AstNode> References => _references;

        public IEnumerable<T> ReferencesAs<T>() where T : class
            => _references.OfType<T>();

        public string Key => MakeKey(SymbolName, SymbolKind);

        public string SymbolName { get; }

        public AstSymbolKind SymbolKind { get; }

        public AstSymbolLocality SymbolLocality { get; set; }

        private string? _namespace;
        public string Namespace
        {
            get { return _namespace ?? SymbolTable.Namespace; }
            set { _namespace = value; }
        }

        public string FullName 
            => String.IsNullOrEmpty(Namespace)
                ? SymbolName 
                : $"{Namespace}.{SymbolName}";

        public AstNode? Definition
            => _definitions.SingleOrDefault() ?? Parent?.Definition;

        public T? DefinitionAs<T>() where T : class
            => Definition as T;

        public bool HasDefinition => _definitions.Count > 0;

        public void PromoteToDefinition(AstNode definitionNodeToAdd, AstNode referenceNodeToRemove)
        {
            Ast.Guard(Definition is null, "Symbol already has a definition.");
            Ast.Guard(_references.Contains(referenceNodeToRemove), "Specified reference Node was not found in the Symbol references.");

            _references.Remove(referenceNodeToRemove);
            AddNode(definitionNodeToAdd);
        }

        public bool HasOverloads => _definitions.Count > 1;

        public IEnumerable<AstFunctionDefinition> Overloads
        {
            get
            {
                if (SymbolKind != AstSymbolKind.Function)
                    return Enumerable.Empty<AstFunctionDefinition>();

                return _definitions.Cast<AstFunctionDefinition>();
            }
        }

        public AstFunctionDefinition? FindFunctionDefinition(AstFunctionReference overload)
            => HasDefinition
            ? Overloads.SingleOrDefault(def => def.FunctionType.OverloadKey == overload.FunctionType.OverloadKey)
            : Parent?.FindFunctionDefinition(overload);

        public void AddNode(AstNode node)
        {
            Ast.Guard(node, "Cannot add null.");

            if (node is AstModule ||
                node is AstFunctionParameterDefinition ||
                node is AstVariableDefinition ||
                node is AstTypeDefinition ||
                node is AstTypeFieldDefinition ||
                node is AstTemplateParameterDefinition)
            {
                Ast.Guard(Definition is null, "Definition is already set.");
                _definitions.Add(node);
            }
            else if (SymbolKind == AstSymbolKind.Function && node is AstFunctionDefinition)
            {
                _definitions.Add(node);
            }
            else if (!_references.Contains(node))
            {
                _references.Add(node);
            }
        }

        public IEnumerable<string> Aliases => _aliases;

        public bool TryAddAlias(string alias)
        {
            if (!String.IsNullOrEmpty(alias) &&
                !_aliases.Contains(alias))
            {
                _aliases.Add(alias);
                return true;
            }
            return false;
        }

        public void AddAlias(string alias)
        {
            if (!TryAddAlias(alias))
                throw new InternalErrorException(
                    $"Alias '{alias}' is alread present for symbol {SymbolName}.");
        }

        internal void RemoveReference(AstNode node)
            => _references.Remove(node);

        internal static string MakeKey(string name, AstSymbolKind kind)
            => name + kind;
    }
}