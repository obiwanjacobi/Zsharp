using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Zsharp.AST
{
    [DebuggerDisplay("{SymbolName} ({SymbolKind})")]
    public class AstSymbolEntry
    {
        private readonly List<AstNode> _definitions = new List<AstNode>();
        private readonly List<AstNode> _references = new List<AstNode>();
        private readonly List<string> _aliases = new List<string>();
        private readonly Dictionary<AstFunctionReference, AstFunctionDefinition> _overloads
            = new Dictionary<AstFunctionReference, AstFunctionDefinition>();

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

        public AstNode? Definition => _definitions.SingleOrDefault();

        public T? DefinitionAs<T>() where T : class => Definition as T;

        public bool HasDefinition => _definitions.Count > 0;

        public void PromoteToDefinition(AstNode definitionNode, AstNode referenceNode)
        {
            Ast.Guard(Definition == null, "Symbol already has a definition.");
            Ast.Guard(_references.IndexOf(referenceNode) != -1, "Specified reference Node was not found in the References.");

            _references.Remove(referenceNode);
            _definitions.Add(definitionNode);
        }

        public bool HasOverloads => _definitions.Count > 1;

        public IEnumerable<AstFunctionDefinition> Overloads => _definitions.Cast<AstFunctionDefinition>();

        public AstFunctionDefinition? FindOverloadDefinition(AstFunctionReference overload)
        {
            _overloads.TryGetValue(overload, out AstFunctionDefinition? functionDef);
            return functionDef;
        }

        public void SetOverload(AstFunctionReference functionRef, AstFunctionDefinition functionDef)
        {
            Ast.Guard(functionRef, "Function Reference argument is null");
            Ast.Guard(functionDef, "Function Definition argument is null");
            Ast.Guard(_references.Contains(functionRef), "Function Reference argument does not belong to this symbol.");
            Ast.Guard(_definitions.Contains(functionDef), "Function Definition argument does not belong to this symbol.");

            _overloads.Add(functionRef, functionDef);
        }

        public void AddNode(AstNode node)
        {
            Ast.Guard(node, "Cannot add null.");

            if (node is AstModuleExternal ||
                node is AstFunctionParameterDefinition ||
                node is AstVariableDefinition ||
                node is AstTypeDefinition ||
                node is AstTypeFieldDefinition ||
                node is AstTemplateParameterDefinition)
            {
                Ast.Guard(Definition == null, "Definition is already set.");
                _definitions.Add(node);
            }
            else if (SymbolKind == AstSymbolKind.Function && node is AstFunctionDefinition)
            {
                // TODO: check overloadKey
                _definitions.Add(node);
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

        internal void Delete()
        {
            SymbolTable.Delete(this);
        }

        internal static string MakeKey(string name, AstSymbolKind kind) => name + kind;
    }
}