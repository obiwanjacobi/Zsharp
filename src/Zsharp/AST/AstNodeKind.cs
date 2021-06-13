namespace Zsharp.AST
{
    public enum AstNodeKind
    {
        None,
        Module,         // root
        File,           // Module.
        Function,       // File.
        Struct,         // File.
        Enum,           // File.
        Type,           // File., Variable:[1], Parameter:[1]

        CodeBlock,      // File.
        Assignment,     // CodeBlock.
        Branch,         // CodeBlock.
        Expression,     // CodeBlock., Branch.[1]
        Operand,        // Expression.1|2
        Literal,        // Expression.1

        Variable,
        FunctionParameter,
        TemplateParameter,
        Field,
        EnumOption,
    }
}