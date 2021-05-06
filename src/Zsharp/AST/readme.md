# Abstract Syntax Tree

After the Z# code is parsed using the Antlr generated lexer and parser code,
the parse tree is converted into an Abstract Syntax Tree (AST).

The `AstBuilder`, `AstNodeBuilder` and `AstExpressionBuilder` classes perform the model transformation.

## Symbols

`AstSymbolsEntry` instances are maintained in a `AstSymbolTable` 
and symbol tables are organized in a hierarchy.

Intrinsic Symbols
|
-- File Symbols --> External Module Symbols
    |
    -- Function Symbols
        |
        -- Code Block Symbols

- Intrinsic Symbols contain the built-in compiler types and functions.
- File Symbols contain all the globals of a code file, including imported external symbols.
- Function Symbols contain all the symbols defined inside a function.
- Code Block Symbols contain the symbols defined in a scope of code (inside a function).
- External Module Symbols contain all symbols for imported modules in a file.

`AstSymbolEntry` instances are put in the symbol table where the symbol is defined.
For example a function definition will be placed in the module's symbol table. 
That symbol entry will record any references to the function and/or defined aliases.

The `TryResolve` method on the `IAstSymbolEntrySite` interface is used to resolve the definition for a reference. 
The symbol entries are rearranged (merged) to add the reference to the definition's symbol entry.

### External Symbols

The Z# `import` keyword pulls in an external module.
External can mean any .NET assembly or a Z# (compiled) module.

The symbols of the external module are stored in a separate Symbol Table inside the external module.
In the File-level symbol table (where the import statements are declared) each external module is added as a 'Module' symbol entry.

