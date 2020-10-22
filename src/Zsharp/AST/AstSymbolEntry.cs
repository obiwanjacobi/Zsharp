using System;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public class AstSymbolEntry
    {
        private readonly List<AstNode> _references = new List<AstNode>();

        public AstSymbolEntry(string symbolName, AstSymbolKind symbolKind)
        {
            SymbolName = symbolName;
            SymbolKind = symbolKind;
        }

        public IEnumerable<AstNode> References => _references;
        public IEnumerable<T> ReferencesOf<T>() where T : AstNode
            => _references.OfType<T>();

        public string Key => MakeKey(SymbolName, SymbolKind);
        public string SymbolName { get; }
        public AstSymbolKind SymbolKind { get; }
        public AstSymbolLocality SymbolLocality { get; set; }

        private AstNode? _definition;
        public AstNode? Definition => _definition;
        public T? GetDefinition<T>() where T : AstNode
            => _definition as T;
        public bool PromoteToDefinition(AstNode referenceNode)
        {
            if (_definition == null)
            {
                if (_references.IndexOf(referenceNode) == -1)
                {
                    throw new ArgumentException($"Specified node was not found in the References.", nameof(referenceNode));
                }

                _references.Remove(referenceNode);
                _definition = referenceNode;
                return true;
            }
            return false;
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
}