namespace Zsharp.AST
{
    public class AstModuleExternal : AstModule,
        IAstExternalNameSite
    {
        public AstModuleExternal(string moduleName, AstSymbolTable? parentTable = null)
            : this(moduleName, moduleName, parentTable)
        { }

        public AstModuleExternal(string ns, string moduleName, AstSymbolTable? parentTable = null)
            : base(AstModuleLocality.External)
        {
            ExternalName = new AstName(ns, moduleName, AstNameKind.External);
            Symbols = new AstSymbolTable(moduleName, parentTable);
            this.SetIdentifier(new AstIdentifier(moduleName, AstIdentifierKind.Module));
        }

        public AstSymbolTable Symbols { get; }

        public AstName ExternalName { get; }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitModuleExternal(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var symbol in Symbols.Symbols)
            {
                if (symbol.HasDefinition)
                {
                    if (symbol.HasOverloads)
                    {
                        foreach (var overload in symbol.Overloads)
                        {
                            visitor.Visit(overload);
                        }
                    }
                    else
                        visitor.Visit(symbol.Definition!);
                }

                foreach (var reference in symbol.References)
                {
                    visitor.Visit(reference);
                }
            }
        }

        public void AddTypeDefinition(AstTypeDefinitionExternal typeDefinition)
        {
            var symbol = Symbols.Add(typeDefinition);
            symbol.SymbolLocality = AstSymbolLocality.Imported;
        }

        public void AddAlias(AstNode source, string alias)
        {
            var identifier = ((IAstIdentifierSite)source).Identifier!;
            var symbol = Symbols.FindSymbol(identifier);
            Ast.Guard(symbol, $"No symbol for '{identifier!.CanonicalName}' was found in external module {Identifier!.Name}.");

            if (source is AstFunctionDefinition functionDef)
            {
                symbol!.TryAddAlias(alias + functionDef.FunctionType!.Identifier!.CanonicalName);
            }
            else
            {
                symbol!.TryAddAlias(alias);
            }
        }

        public void AddAlias(string symbolName, string alias)
        {
            var symbol = Symbols.FindSymbol(symbolName, AstSymbolKind.Unknown);
            Ast.Guard(symbol, $"No symbol for '{symbol}' was found in external module {Identifier!.Name}.");
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
