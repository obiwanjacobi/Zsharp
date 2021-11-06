namespace Zsharp.AST
{
    public class AstModuleExternal : AstModule,
        IAstExternalNameSite
    {
        public AstModuleExternal(string moduleName, AstSymbolTable? parentTable = null)
            : base(AstModuleLocality.External)
        {
            var symbolName = AstSymbolName.Parse(moduleName, AstNameKind.External);
            Symbols = new AstSymbolTable(symbolName.CanonicalName.FullName, parentTable);
            this.SetIdentifier(new AstIdentifier(symbolName, AstIdentifierKind.Module));
        }

        public AstSymbolTable Symbols { get; }

        public AstName ExternalName => Identifier!.SymbolName.NativeName;

        public override void Accept(AstVisitor visitor)
            => visitor.VisitModuleExternal(this);

        public void AddTypeDefinition(AstTypeDefinitionExternal typeDefinition)
        {
            var symbol = Symbols.Add(typeDefinition);
            symbol.SymbolLocality = AstSymbolLocality.Imported;
        }

        public void AddAlias(AstNode source, string alias)
        {
            var identifier = ((IAstIdentifierSite)source).Identifier!;
            var symbol = Symbols.FindSymbol(identifier);
            Ast.Guard(symbol, $"No symbol for '{identifier!.SymbolName.CanonicalName.FullName}' was found in external module {Identifier!.NativeFullName}.");

            if (source is AstFunctionDefinition functionDef)
            {
                symbol!.TryAddAlias(alias + functionDef.FunctionType!.Identifier!.SymbolName.CanonicalName.FullName);
            }
            else
            {
                symbol!.TryAddAlias(alias);
            }
        }

        public void AddAlias(AstName symbolName, string alias)
        {
            var symbol = Symbols.FindSymbol(symbolName, AstSymbolKind.Unknown);
            Ast.Guard(symbol, $"No symbol for '{symbol}' was found in external module {Identifier!.NativeFullName}.");
            symbol!.TryAddAlias(alias);
        }

        public void AddFunction(AstFunctionDefinitionExternal function)
        {
            if (function.FunctionType.TypeReference is null)
            {
                var typeRef = new AstTypeReferenceType(AstIdentifierIntrinsic.Void);
                function.FunctionType.SetTypeReference(typeRef);
            }

            function.CreateSymbols(Symbols);
            function.Symbol!.SymbolLocality = AstSymbolLocality.Imported;
        }
    }
}
