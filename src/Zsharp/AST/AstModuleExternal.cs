using System;

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

        public void AddAlias(string symbol, string alias, AstSymbolKind symbolKind)
        {
            if (!String.IsNullOrEmpty(symbol))
            {
                var entry = Symbols.FindEntry(symbol, symbolKind);
                Ast.Guard(entry, $"No symbol for '{symbol}' was found in external module {Identifier!.Name}.");
                entry!.AddAlias(alias);
            }
            else
            {
                // TODO: Module name alias
                throw new NotSupportedException(
                    "Module Name aliases are not supported yet.");
            }
        }

        public void AddFunction(AstFunctionDefinitionExternal function)
        {
            if (function.TypeReference == null)
            {
                var typeRef = AstTypeReference.From(AstTypeDefinitionIntrinsic.Void);
                function.SetTypeReference(typeRef);
            }

            function.CreateSymbols(Symbols);
            function.Symbol!.SymbolLocality = AstSymbolLocality.Imported;
        }
    }
}
