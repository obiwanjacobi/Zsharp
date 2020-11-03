using System;

namespace Zsharp.AST
{
    public class AstModuleExternal : AstModule
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

        public void AddAlias(string symbol, string alias)
        {
            if (!String.IsNullOrEmpty(symbol))
            {
                var entry = Symbols.FindEntry(symbol, AstSymbolKind.NotSet);
                entry.AddAlias(alias);
            }
            else
            {
                // TODO: Module name alias
                throw new NotSupportedException("Module Name aliases are not supported yet.");
            }
        }

        public void AddFunction(AstFunctionExternal function)
        {
            var entry = Symbols.Add(function);
            entry.SymbolLocality = AstSymbolLocality.Imported;
        }
    }
}
