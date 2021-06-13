using System;
using System.Collections.Generic;

namespace Zsharp.AST
{
    public static class AstSymbolTableExtensions
    {
        public static AstSymbolTable GetRootTable(this AstSymbolTable symbolTable)
        {
            var previousParent = symbolTable.ParentTable;
            var parent = symbolTable;

            if (previousParent is null)
                throw new InternalErrorException(
                    "Don't call GetRootTable on the Intriniscs Table.");

            while (parent is not null)
            {
                previousParent = parent;
                parent = parent.ParentTable;
            }

            // The real root contains the intrinsics
            // we return the module/file symbol table
            return previousParent;
        }

        public static AstSymbolEntry? Find<T>(this AstSymbolTable symbolTable, T node)
            where T : AstNode, IAstIdentifierSite
            => symbolTable.Find(node, node.NodeKind.ToSymbolKind());

        public static AstSymbolEntry? Find(this AstSymbolTable symbolTable, AstIdentifier identifier, AstSymbolKind kind = AstSymbolKind.NotSet)
        {
            if (kind == AstSymbolKind.NotSet)
                kind = identifier!.IdentifierKind.ToSymbolKind();

            return symbolTable.FindEntry(identifier.SymbolName.ToCanonical(), kind);
        }

        public static AstSymbolEntry? Find(this AstSymbolTable symbolTable, IAstIdentifierSite identifierSite, AstSymbolKind kind = AstSymbolKind.NotSet)
        {
            identifierSite.ThrowIfIdentifierNotSet();

            if (kind == AstSymbolKind.NotSet)
                kind = identifierSite.Identifier!.IdentifierKind.ToSymbolKind();

            return symbolTable.FindEntry(identifierSite.Identifier!.SymbolName.ToCanonical(), kind);
        }

        public static AstSymbolKind ToSymbolKind(this AstNodeKind nodeKind)
        {
            return nodeKind switch
            {
                AstNodeKind.Enum => AstSymbolKind.Type,
                AstNodeKind.Function => AstSymbolKind.Function,
                AstNodeKind.Struct => AstSymbolKind.Type,
                AstNodeKind.Type => AstSymbolKind.Type,
                AstNodeKind.Module => AstSymbolKind.Module,
                AstNodeKind.FunctionParameter => AstSymbolKind.Variable,
                AstNodeKind.Variable => AstSymbolKind.Variable,
                AstNodeKind.Field => AstSymbolKind.Field,
                AstNodeKind.EnumOption => AstSymbolKind.Field,
                AstNodeKind.TemplateParameter => AstSymbolKind.TemplateParameter,
                _ => AstSymbolKind.Unknown
            };
        }

        public static AstSymbolKind ToSymbolKind(this AstIdentifierKind identifierKind)
        {
            return identifierKind switch
            {
                AstIdentifierKind.Function => AstSymbolKind.Function,
                AstIdentifierKind.Type => AstSymbolKind.Type,
                AstIdentifierKind.Module => AstSymbolKind.Module,
                AstIdentifierKind.Variable => AstSymbolKind.Variable,
                AstIdentifierKind.Field => AstSymbolKind.Field,
                AstIdentifierKind.EnumOption => AstSymbolKind.Field,
                AstIdentifierKind.TemplateParameter => AstSymbolKind.TemplateParameter,
                _ => AstSymbolKind.Unknown
            };
        }

        public static AstSymbolEntry? TryAdd<T>(this AstSymbolTable symbolTable, T? node)
            where T : AstNode, IAstIdentifierSite
        {
            if (node is not null &&
                (node as IAstSymbolEntrySite)?.Symbol is null)
            {
                return Add(symbolTable, node);
            }

            return null;
        }

        public static AstSymbolEntry Add<T>(this AstSymbolTable symbolTable, T node)
            where T : AstNode, IAstIdentifierSite
            => AddSymbol(symbolTable, node, node.NodeKind.ToSymbolKind(), node);

        private static AstSymbolEntry AddSymbol(AstSymbolTable symbolTable,
            IAstIdentifierSite identifierSite, AstSymbolKind symbolKind, AstNode node)
        {
            var name = identifierSite.Identifier?.CanonicalName
                ?? throw new ArgumentException("No identifier name.", nameof(identifierSite));

            // reference to a template parameter is detected as TypeReference
            if (symbolKind == AstSymbolKind.Type &&
                node is AstTypeReferenceType typeRef &&
                typeRef.IsTemplateParameter)
                symbolKind = AstSymbolKind.TemplateParameter;

            var entry = symbolTable.AddSymbol(name, symbolKind, node);

            if (node is IAstSymbolEntrySite symbolSite &&
                symbolSite.Symbol is null)
                symbolSite.SetSymbol(entry);

            return entry;
        }

        internal static void RemoveReferences<T>(this AstSymbolTable symbolTable, IEnumerable<T> nodesToRemove)
            where T : AstNode, IAstIdentifierSite
        {
            foreach (var node in nodesToRemove)
            {
                var entry = symbolTable.Find(node);
                if (entry is not null)
                {
                    entry.RemoveReference(node);
                }
            }
        }
    }
}
