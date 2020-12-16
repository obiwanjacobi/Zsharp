namespace Zsharp.AST
{
    public enum AstSymbolKind
    {
        NotSet,
        Unknown,
        Module,
        Function,
        Type,               // Struct, Enum
        Variable,
        Field,
        TemplateParameter,  // Type | Variable
    };
}