# Abstract Syntax Tree

After the Z# code is parsed using the Antlr generated lexer and parser code,
the parse tree is converted into an Abstract Syntax Tree (AST).

The `AstBuilder`, `AstNodeBuilder` and `AstExpressionBuilder` classes perform the model transformation.

## Symbols

`AstSymbolsEntry` instances are maintained in a `AstSymbolTable` and symbol tables are organized in a hierarchy.

```
Intrinsic Symbols
|
-- File Symbols --> External Module Symbols
    |
    -- Function Symbols
        |
        -- Code Block Symbols
```

- Intrinsic Symbols contain the built-in compiler types and functions.
- File Symbols contain all the globals of a code file, including imported external symbols.
- Function Symbols contain all the symbols defined inside a function.
- Code Block Symbols contain the symbols defined in a scope of code (inside a function).
- External Module Symbols contain all symbols for imported modules in a file.

`AstSymbolEntry` instances are put in the symbol table where the symbol is defined.
For example a function definition will be placed in the file's symbol table. That symbol entry will record any references to the function and/or defined aliases.

The `TryResolve` method on AstXxxxReference types is used to resolve the symbol entry with the definition for that reference. The symbol entries are rearranged (merged) to add the reference to the definition's symbol entry.

### TryResolve

After the AST is built, the resolve-definition phase is started. This will walk (visitor) the AST tree and check if each AST node has a symbol definition. The `TryResolve` on an AST node (for a reference) searches the Symbol Table hierarchy for a symbol entry of the same name with a symbol definition set. When found the reference is merged into the symbol entry of the definition and deleted.

### Resolve Function References

Function References have to be matched to a Function Definition. However, not all type information can always be inferred from the function reference's location.

The introduction of a 'FunctionType' isolates the template and function parameters as well as the return type. In order to match a function reference to its definition, assuming the identifiers match, only their FunctionTypes have to be compared in an 'open' and flexible way.

When a match is found, the types on the Function Reference may have to be changed in order to match those of the function definition. Then the overload-keys will be equal and the `FunctionDefinition` will show up on the reference object.

## External Symbols

The Z# `import` keyword pulls in an external module.
External can mean any .NET assembly or a Z# (compiled) module.

The symbols of the external module are stored in a separate Symbol Table inside the external module.
In the File-level symbol table (where the import statements are declared) only referenced external modules are added as a 'Module' symbol entry.

When `TryResolve` on a type or function is called and the definition of a symbol cannot be found in the immediate symbol table hierarchy, these module entries are use to try to resolve the definition. If no definition is found a compiler error is generated (undefined type/function).

### TryResolve External Symbols

External modules can have dependencies on other external modules. All of them will be loaded by the ModuleManager. When the resolve-definition phase starts in the compiler, these external module symbols also have to be resolved. They can have references between modules and references to the Z# compiler intrinsic symbols (root symbol table).

```
Module Manager
|
--- SymbolTable
    |
    --- Code Module (Public)
    |
    --- External Module (Module)
        |
        --- SymbolTable
            |
            --- Type/Function Symbol Entries
                |
                --- Symbol Definition
                |
                --- Symbol References
```

By putting all external modules into one Symbol Table the normal process of `TryResolve` can be (re)used.
