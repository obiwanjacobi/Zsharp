using System;

namespace Zsharp.Runtime.Compiler
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public sealed class SymbolAliasAttribute : Attribute
    {
        public SymbolAliasAttribute(string alias)
            => _alias = alias;
        private readonly string _alias;
        public string Alias => _alias;
    }
}
