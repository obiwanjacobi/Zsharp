using System;

namespace Zsharp.AST
{
    public static class AstSymbolTableExtensions
    {
        public static AstSymbolTable GetRootTable(this AstSymbolTable symbolTable)
        {
            var previousParent = symbolTable.ParentTable;
            var parent = symbolTable;

            if (previousParent == null)
                throw new InternalErrorException(
                    "Don't call GetRootTable on the Intriniscs Table.");

            while (parent != null)
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
            => symbolTable.Find(node, node.NodeType.ToSymbolKind());

        public static AstSymbolEntry? Find(this AstSymbolTable symbolTable, AstIdentifier identifier, AstSymbolKind kind = AstSymbolKind.NotSet)
        {
            if (kind == AstSymbolKind.NotSet)
                kind = identifier!.IdentifierType.ToSymbolKind();

            return symbolTable.FindEntry(identifier.CanonicalName, kind);
        }

        public static AstSymbolEntry? Find(this AstSymbolTable symbolTable, IAstIdentifierSite identifierSite, AstSymbolKind kind = AstSymbolKind.NotSet)
        {
            identifierSite.ThrowIfIdentifierNotSet();

            if (kind == AstSymbolKind.NotSet)
                kind = identifierSite.Identifier!.IdentifierType.ToSymbolKind();

            return symbolTable.FindEntry(identifierSite.Identifier!.CanonicalName, kind);
        }

        public static AstSymbolKind ToSymbolKind(this AstNodeType nodeType)
        {
            return nodeType switch
            {
                AstNodeType.Enum => AstSymbolKind.Type,
                AstNodeType.Function => AstSymbolKind.Function,
                AstNodeType.Struct => AstSymbolKind.Type,
                AstNodeType.Type => AstSymbolKind.Type,
                AstNodeType.Module => AstSymbolKind.Module,
                AstNodeType.FunctionParameter => AstSymbolKind.Variable,
                AstNodeType.Variable => AstSymbolKind.Variable,
                AstNodeType.Field => AstSymbolKind.Field,
                AstNodeType.EnumOption => AstSymbolKind.Field,
                AstNodeType.TemplateParameter => AstSymbolKind.TemplateParameter,
                _ => AstSymbolKind.Unknown
            };
        }

        public static AstSymbolKind ToSymbolKind(this AstIdentifierType identifierType)
        {
            return identifierType switch
            {
                AstIdentifierType.Function => AstSymbolKind.Function,
                AstIdentifierType.Type => AstSymbolKind.Type,
                AstIdentifierType.Module => AstSymbolKind.Module,
                AstIdentifierType.Variable => AstSymbolKind.Variable,
                AstIdentifierType.Field => AstSymbolKind.Field,
                AstIdentifierType.EnumOption => AstSymbolKind.Field,
                AstIdentifierType.TemplateParameter => AstSymbolKind.TemplateParameter,
                _ => AstSymbolKind.Unknown
            };
        }

        public static AstSymbolEntry Add<T>(this AstSymbolTable symbolTable, T node)
            where T : AstNode, IAstIdentifierSite
            => AddSymbol(symbolTable, node, node.NodeType.ToSymbolKind(), node);

        private static AstSymbolEntry AddSymbol(AstSymbolTable symbolTable,
            IAstIdentifierSite identifierSite, AstSymbolKind symbolKind, AstNode node)
        {
            var name = identifierSite.Identifier?.CanonicalName
                ?? throw new ArgumentException("No identifier name.", nameof(identifierSite));

            // reference to a template parameter are detected as TypeReferences
            if (symbolKind == AstSymbolKind.Type &&
                node is AstTypeReference typeRef &&
                typeRef.IsTemplateParameter)
                symbolKind = AstSymbolKind.TemplateParameter;

            var entry = symbolTable.AddSymbol(name, symbolKind, node);

            if (node is IAstSymbolEntrySite symbolSite)
                symbolSite.SetSymbol(entry);

            return entry;
        }
    }
}
