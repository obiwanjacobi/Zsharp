using System;

namespace Zsharp.AST
{
    public static class AstSymbolTableExtensions
    {
        public static T? FindDefinition<T>(this AstSymbolTable symbolTable, string symbolName, AstSymbolKind symbolKind)
            where T : class
        {
            var entry = symbolTable.FindEntry(symbolName, symbolKind);
            if (entry != null)
            {
                return entry.DefinitionAs<T>();
            }
            return null;
        }

        public static AstSymbolEntry? Find<T>(this AstSymbolTable symbolTable, T node)
            where T : AstNode, IAstIdentifierSite
            => symbolTable.FindEntry(node, node.NodeType.ToSymbolKind());

        public static AstSymbolKind ToSymbolKind(this AstNodeType nodeType)
        {
            return nodeType switch
            {
                AstNodeType.Enum => AstSymbolKind.Type,
                AstNodeType.Function => AstSymbolKind.Function,
                AstNodeType.Struct => AstSymbolKind.Type,
                AstNodeType.Type => AstSymbolKind.Type,
                AstNodeType.Module => AstSymbolKind.Module,
                AstNodeType.Variable => AstSymbolKind.Variable,
                AstNodeType.Field => AstSymbolKind.Field,
                AstNodeType.EnumOption => AstSymbolKind.Field,
                AstNodeType.TemplateParameter => AstSymbolKind.TemplateParameter,
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
