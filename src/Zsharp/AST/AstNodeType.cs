﻿namespace Zsharp.AST
{
    public enum AstNodeType
    {
        None,
        Module,         // root
        File,           // Module.
        Global,         // File.1
        Function,       // File.
        Struct,         // File.
        Enum,           // File.
        Type,           // File., Variable:[1], Parameter:[1]

        CodeBlock,      // File.
        Assignment,     // CodeBlock.
        Branch,         // CodeBlock.
        Expression,     // CodeBlock., Branch.[1]
        Operand,        // Expression.1|2
        Numeric,        // Expression.1

        Identifier,     // Function.1, Struct.1, Enum.1, Assignment.1

        Variable,
        FunctionParameter,
    }
}