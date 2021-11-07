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

        public static AstSymbol? FindSymbol(this AstSymbolTable symbolTable, AstIdentifier identifier, AstSymbolKind kind = AstSymbolKind.NotSet)
        {
            if (kind == AstSymbolKind.NotSet)
                kind = identifier.IdentifierKind.ToSymbolKind();

            return symbolTable.FindSymbol(identifier.SymbolName.CanonicalName, kind);
        }

        public static AstSymbol? FindSymbol(this AstSymbolTable symbolTable, IAstIdentifierSite identifierSite, AstSymbolKind kind = AstSymbolKind.NotSet)
        {
            if (kind == AstSymbolKind.NotSet)
                kind = identifierSite.Identifier.IdentifierKind.ToSymbolKind();

            return symbolTable.FindSymbol(identifierSite.Identifier.SymbolName.CanonicalName, kind);
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

        public static AstSymbol? TryAdd<T>(this AstSymbolTable symbolTable, T? node)
            where T : AstNode, IAstIdentifierSite
        {
            if (node is IAstSymbolSite symbolSite &&
                !symbolSite.HasSymbol)
            {
                return Add(symbolTable, node);
            }

            return null;
        }

        public static AstSymbol Add<T>(this AstSymbolTable symbolTable, T node)
            where T : AstNode, IAstIdentifierSite
            => symbolTable.AddSymbol(node, node.NodeKind.ToSymbolKind(), node);
    }
}
