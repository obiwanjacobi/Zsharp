namespace Zsharp.AST
{
    public class AstModuleExternal : AstModule
    {
        public AstModuleExternal(AstName moduleName, AstSymbolTable? parentTable = null)
            : base(AstModuleLocality.External)
        {
            var symbolName = new AstSymbolName(moduleName);
            SymbolTable = new AstSymbolTable(symbolName.CanonicalName.FullName, parentTable);
            this.SetIdentifier(new AstIdentifier(symbolName, AstIdentifierKind.Module));
        }

        public AstSymbolTable SymbolTable { get; }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitModuleExternal(this);

        public void AddTypeDefinition(AstTypeDefinitionExternal typeDefinition)
        {
            var symbol = SymbolTable.Add(typeDefinition);
            symbol.SymbolLocality = AstSymbolLocality.Imported;
        }

        public void AddAlias(AstNode source, string alias)
        {
            var identifier = ((IAstIdentifierSite)source).Identifier;
            var symbol = SymbolTable.FindSymbol(identifier);
            Ast.Guard(symbol, $"No symbol for '{identifier.SymbolName.CanonicalName.FullName}' was found in external module {Identifier.NativeFullName}.");

            if (source is AstFunctionDefinition functionDef)
            {
                symbol!.TryAddAlias(alias + functionDef.FunctionType.Identifier.SymbolName.CanonicalName.FullName);
            }
            else
            {
                symbol!.TryAddAlias(alias);
            }
        }

        public void AddAlias(AstName symbolName, string alias)
        {
            var symbol = SymbolTable.FindSymbol(symbolName, AstSymbolKind.Unknown);
            Ast.Guard(symbol, $"No symbol for '{symbol}' was found in external module {Identifier.NativeFullName}.");
            symbol!.TryAddAlias(alias);
        }

        public void AddFunction(AstFunctionDefinitionExternal function)
        {
            if (!function.FunctionType.HasTypeReference)
            {
                var typeRef = new AstTypeReferenceType(AstIdentifierIntrinsic.Void);
                function.FunctionType.SetTypeReference(typeRef);
            }

            function.CreateSymbols(SymbolTable);
            function.Symbol.SymbolLocality = AstSymbolLocality.Imported;
        }
    }
}
