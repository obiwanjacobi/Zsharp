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

        public AstSymbol(AstSymbolTable symbolTable, AstName symbolName, AstSymbolKind symbolKind)
        {
            SymbolTable = symbolTable;
            SymbolName = symbolName;
            SymbolKind = symbolKind;
        }

        public AstSymbolTable SymbolTable { get; }

        private AstSymbol? _parentSymbol;
        public AstSymbol? ParentSymbol => _parentSymbol;

        public bool TrySetParentSymbol(AstSymbol parent)
        {
            Ast.Guard(parent != this, "Cannot add this as parent Symbol.");

            if (_parentSymbol is null && parent is not null &&
                parent.TryAddChildSymbol(this))
            {
                _parentSymbol = parent;
                return true;
            }
            return false;
        }

        public void SetParentSymbol(AstSymbol parent)
        {
            if (!TrySetParentSymbol(parent))
                throw new InternalErrorException(
                    "Symbol parent is already set or null.");
        }

        private readonly List<AstSymbol> _childSymbols = new();
        public IEnumerable<AstSymbol> ChildSymbols => _childSymbols;

        public bool TryAddChildSymbol(AstSymbol symbol)
        {
            if (symbol is not null &&
                !_childSymbols.Contains(symbol))
            {
                _childSymbols.Add(symbol);
                return true;
            }
            return false;
        }

        public void AddChildSymbol(AstSymbol symbol)
        {
            if (!TryAddChildSymbol(symbol))
                throw new InternalErrorException(
                    "Symbol child is already added or null.");
        }

        public IEnumerable<AstNode> References => _references;

        public IEnumerable<T> ReferencesAs<T>() where T : class
            => _references.OfType<T>();

        public string Key => MakeKey(SymbolName.Symbol, SymbolKind, SymbolName.GetArgumentCount());

        public AstName SymbolName { get; }

        public AstSymbolKind SymbolKind { get; }

        public AstSymbolLocality SymbolLocality { get; set; }

        public AstNode? Definition
            => _definitions.SingleOrDefault() ?? ParentSymbol?.Definition;

        public T? DefinitionAs<T>() where T : class
            => _definitions.OfType<T>().SingleOrDefault() ?? ParentSymbol?.DefinitionAs<T>();

        public bool IsDefined => DefinitionSymbol is not null;

        public AstSymbol? DefinitionSymbol
            => HasDefinition ? this : ParentSymbol?.DefinitionSymbol;

        public bool HasDefinition => _definitions.Count > 0;

        public bool HasOverloads => _definitions.Count > 1;

        public IEnumerable<AstFunctionDefinition> FunctionOverloads
        {
            get
            {
                Ast.Guard(SymbolKind == AstSymbolKind.Function, $"Cannot call FunctionOverloads on a {SymbolKind} symbol.");
                return _definitions.Cast<AstFunctionDefinition>();
            }
        }

        public T? TemplateDefinitionAs<T>()
            where T : AstTypeDefinitionTemplate
        {
            if (HasDefinition)
            {
                return _definitions
                    .OfType<T>()
                    .SingleOrDefault(d => d.IsTemplate);
            }

            return ParentSymbol?.TemplateDefinitionAs<T>();
        }

        public T? TemplateInstanceAs<T>(IEnumerable<AstTypeReference> templateArgumentTypes)
            where T : AstNode
        {
            if (HasDefinition)
            {
                return _definitions
                    .OfType<IAstTemplateInstance>()
                    .SingleOrDefault(i => i.TemplateParameterArguments.Arguments
                        .Select(a => a.TypeReference.Identifier.CanonicalFullName)
                        .SequenceEqual(
                            templateArgumentTypes.Select(a => a.Identifier.CanonicalFullName))) as T;
            }

            return ParentSymbol?.TemplateInstanceAs<T>(templateArgumentTypes);
        }

        public AstFunctionDefinition? FindFunctionDefinition(AstFunctionReference overload)
        {
            if (HasDefinition)
            {
                if (overload.IsTemplateOrGeneric)
                {
                    return FunctionOverloads.SingleOrDefault(def => def.Identifier.SymbolName.CanonicalName.GetArgumentCount() == overload.Identifier.SymbolName.CanonicalName.GetArgumentCount());
                }

                return FunctionOverloads.SingleOrDefault(def => def.FunctionType.OverloadKey == overload.FunctionType.OverloadKey);
            }

            return ParentSymbol?.FindFunctionDefinition(overload);
        }

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
                if (node is not AstTypeDefinitionTemplate &&
                    node is not AstTemplateInstanceType &&
                    node is not AstTypeDefinitionFunction)
                {
                    Ast.Guard(Definition is null, "Definition is already set.");
                }
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
            // TODO: call node as IAstSymbolSite to remove symbol
            => _references.Remove(node);
        // if no references are left and no parent or child and no definition - this symbol can be deleted.

        internal static string MakeKey(string name, AstSymbolKind kind, int argCount)
            => $"{name}{kind}{argCount}";
    }
}
