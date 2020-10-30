namespace Zsharp.AST
{
    public class AstModuleExternal : AstModule, IAstSymbolTableSite
    {
        public AstModuleExternal(string moduleName)
            : base(moduleName, AstModuleLocality.External)
        {
            Symbols = new AstSymbolTable(moduleName);
        }

        public AstSymbolTable Symbols { get; }

        public override void Accept(AstVisitor visitor)
        {
            // no-op
        }

        public void AddTypeDefinition(AstTypeDefinitionExternal typeDefinition)
        {
            var entry = Symbols.AddSymbol(typeDefinition.Identifier.Name, AstSymbolKind.Type, typeDefinition);
            entry.SymbolLocality = AstSymbolLocality.Imported;
        }
    }
}
