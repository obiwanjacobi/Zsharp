using System;

namespace Zsharp.AST
{
    public class AstExternalName
    {
        public AstExternalName(string @namespace, string symbolName, string? typeName = null)
        {
            Namespace = @namespace;
            TypeName = typeName;
            SymbolName = symbolName;
        }

        public string Namespace { get; }
        public string SymbolName { get; }
        public string? TypeName { get; }

        public string FullName
            => String.IsNullOrEmpty(TypeName)
                ? $"{Namespace}.{SymbolName}"
                : $"{Namespace}.{TypeName}.{SymbolName}";
    }
}
