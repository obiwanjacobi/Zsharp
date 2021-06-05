namespace Zsharp.AST
{
    public class AstModuleExternal : AstModule
    {
        public AstModuleExternal(string moduleName, AstSymbolTable? parentTable = null)
            : this(moduleName, moduleName, parentTable)
        { }

        public AstModuleExternal(string ns, string moduleName, AstSymbolTable? parentTable = null)
            : base(AstModuleLocality.External)
        {
            Namespace = ns;
            Symbols = new AstSymbolTable(moduleName, parentTable);
            this.SetIdentifier(new AstIdentifier(moduleName, AstIdentifierType.Module));
        }

        public AstSymbolTable Symbols { get; }

        public string Namespace { get; }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitModuleExternal(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var symbol in Symbols.Entries)
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
            var entry = Symbols.Add(typeDefinition);
            entry.SymbolLocality = AstSymbolLocality.Imported;
        }

        public void AddAlias(AstNode source, string alias)
        {
            var identifier = ((IAstIdentifierSite)source).Identifier!;
            var entry = Symbols.Find(identifier);
            Ast.Guard(entry, $"No symbol for '{identifier!.CanonicalName}' was found in external module {Identifier!.Name}.");

            if (source is AstFunctionDefinition functionDef)
            {
                entry!.TryAddAlias(alias + functionDef.FunctionType!.Identifier!.CanonicalName);
            }
            else
            {
                entry!.TryAddAlias(alias);
            }
        }

        public void AddAlias(string symbol, string alias)
        {
            var entry = Symbols.FindEntry(symbol, AstSymbolKind.Unknown);
            Ast.Guard(entry, $"No symbol for '{symbol}' was found in external module {Identifier!.Name}.");
            entry!.TryAddAlias(alias);
        }

        public void AddFunction(AstFunctionDefinitionExternal function)
        {
            if (function.FunctionType.TypeReference == null)
            {
                var typeRef = AstTypeReference.From(AstTypeDefinitionIntrinsic.Void);
                function.FunctionType.SetTypeReference(typeRef);
            }

            function.CreateSymbols(Symbols);
            function.Symbol!.SymbolLocality = AstSymbolLocality.Imported;
        }
    }
}
