using System;

namespace Zsharp.AST
{
    public class AstModuleExternal : AstModule
    {
        public AstModuleExternal(string moduleName)
            : this(moduleName, moduleName)
        { }

        public AstModuleExternal(string ns, string moduleName)
            : base(AstModuleLocality.External)
        {
            Namespace = ns;
            Symbols = new AstSymbolTable(moduleName);
            SetIdentifier(new AstIdentifier(moduleName, AstIdentifierType.Module));
        }

        public AstSymbolTable Symbols { get; }

        public string Namespace { get; }

        public override void Accept(AstVisitor visitor)
        {
            // no-op
        }

        public void AddTypeDefinition(AstTypeDefinitionExternal typeDefinition)
        {
            var entry = Symbols.AddSymbol(typeDefinition.Identifier.CanonicalName, AstSymbolKind.Type, typeDefinition);
            entry.SymbolLocality = AstSymbolLocality.Imported;
        }

        public void AddAlias(string symbol, string alias)
        {
            if (!String.IsNullOrEmpty(symbol))
            {
                var entry = Symbols.FindEntry(symbol, AstSymbolKind.Unknown);
                Ast.Guard(entry, $"No symbol for '{symbol}' was found in external module {Identifier.Name}.");
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
            var entry = Symbols.Add(function);
            entry.SymbolLocality = AstSymbolLocality.Imported;
        }
    }
}
