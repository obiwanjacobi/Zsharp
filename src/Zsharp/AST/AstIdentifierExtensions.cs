using System;

namespace Zsharp.AST
{
    public static class AstIdentifierExtensions
    {
        public static AstSymbolEntry AddSymbol(this AstIdentifier identifier)
        {
            var symbols = identifier.GetParentRecursive<IAstSymbolTableSite>() ??
                throw new InvalidOperationException("No SymbolTable Site could be found.");

            return symbols.AddSymbol(identifier.Name,
                ToSymbolKind(identifier.IdentifierType), identifier);
        }

        private static AstSymbolKind ToSymbolKind(AstIdentifierType idType)
        {
            switch (idType)
            {
                case AstIdentifierType.Variable:
                    return AstSymbolKind.Variable;
                case AstIdentifierType.EnumOption:
                case AstIdentifierType.Field:
                    return AstSymbolKind.Field;
                case AstIdentifierType.Function:
                    return AstSymbolKind.Function;
                case AstIdentifierType.Parameter:
                    return AstSymbolKind.Parameter;
                case AstIdentifierType.Type:
                    return AstSymbolKind.Type;
                default:
                    return AstSymbolKind.NotSet;
            }
        }
    }
}