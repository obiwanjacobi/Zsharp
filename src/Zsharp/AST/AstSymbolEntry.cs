using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Zsharp.AST
{
    [DebuggerDisplay("{SymbolName} ({SymbolKind})")]
    public class AstSymbolEntry
    {
        private readonly List<AstNode> _references = new List<AstNode>();
        private readonly List<string> _aliases = new List<string>();

        public AstSymbolEntry(AstSymbolTable symbolTable, string symbolName, AstSymbolKind symbolKind)
        {
            SymbolTable = symbolTable;
            SymbolName = symbolName;
            SymbolKind = symbolKind;
        }

        public AstSymbolTable SymbolTable { get; }

        public IEnumerable<AstNode> References => _references;
        public IEnumerable<T> ReferencesOf<T>() where T : AstNode
            => _references.OfType<T>();

        public string Key => MakeKey(SymbolName, SymbolKind);
        public string SymbolName { get; }
        public AstSymbolKind SymbolKind { get; }
        public AstSymbolLocality SymbolLocality { get; set; }

        public AstNode? Definition { get; private set; }
        public T? DefinitionAs<T>() where T : class => Definition as T;

        public void PromoteToDefinition(AstNode definitionNode, AstNode referenceNode)
        {
            Ast.Guard(Definition == null, "Symbol already has a definition.");
            Ast.Guard(_references.IndexOf(referenceNode) != -1, "Specified reference Node was not found in the References.");

            _references.Remove(referenceNode);
            Definition = definitionNode;
        }

        public void AddNode(AstNode node)
        {
            Ast.Guard(node, "Cannot add null.");

            if ((SymbolKind == AstSymbolKind.Module && node is AstModuleExternal) ||
                (SymbolKind == AstSymbolKind.Function && node is AstFunctionDefinitionImpl) ||
                (SymbolKind == AstSymbolKind.Function && node is AstFunctionExternal) ||
                (SymbolKind == AstSymbolKind.Variable && node is AstFunctionParameter) ||
                (SymbolKind == AstSymbolKind.Variable && node is AstVariableDefinition) ||
                (SymbolKind == AstSymbolKind.Type && node is AstTypeDefinition)
                // (SymbolKind == AstSymbolKind.Struct && node is AstStruct) ||
                // (SymbolKind == AstSymbolKind.Enum && node is AstEnum) ||
                // (SymbolKind == AstSymbolKind.Field && node is AstField)
                )
            {
                Ast.Guard(Definition == null, "Definition is already set.");
                Definition = node;
            }
            else
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
                throw new ArgumentException(
                    $"Alias '{alias}' is alread present for symbol {SymbolName}.", nameof(alias));
        }

        internal static string MakeKey(string name, AstSymbolKind kind) => name + kind;
    }
}