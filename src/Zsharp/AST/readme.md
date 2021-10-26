# Abstract Syntax Tree

After the Z# code is parsed using the Antlr generated lexer and parser code,
the parse tree is converted into an Abstract Syntax Tree (AST).

The `AstBuilder`, `AstNodeBuilder` and `AstExpressionBuilder` classes perform the model transformation.

## Symbols

`AstSymbol` instances are maintained in a `AstSymbolTable` and symbol tables are organized in a hierarchy.

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
- Code Block Symbols contain the symbols defined in a scope of code (at global level or inside a function).
- External Module Symbols contain all symbols for imported modules in a file.

`AstSymbol` instances are stored in the symbol table that represent the scope they were encountered at.
For example a function definition will be placed in the file's symbol table. That symbol entry will be linked to any references to the function and/or defined aliases found elsewhere. If an `AstSymbol` instance does not contain a definition, its parent (recursively) will.

The `TryResolve` method on AstXxxxReference types is used to resolve the symbol entry with the definition for that reference. This is done by searching parent symbol tables for the same symbol name and linking the resulting `AstSymbol` instances.

### TryResolve

After the AST is built, the resolve-definition phase is started. This will walk (visitor) the AST tree and check if each AST node has a symbol definition. The `TryResolve` on an AST node (for a reference) searches the Symbol Table hierarchy for a symbol entry of the same name with a symbol definition set. When found the symbol of the reference is linked to the symbol entry of the definition.

### Resolve Function References

Function References have to be matched to a Function Definition. However, not all type information can always be inferred from the function reference's location.

The introduction of a 'FunctionType' isolates the function parameters and the return type. In order to match a function reference to its definition, assuming the identifiers match, their FunctionTypes have to be compared in an 'open' and flexible way.

Template parameters are on the function, not the function type, but they too have to be matched - although this is much simpler.

The matching algorithm needs to know what function arguments types are concrete and which are inferred. Because of the leave-to-root nature of processing of the AST, a type reference for, say a number literal, is inferred from the literal and set to its smallest possible type. These inferred types can be changed to 'larger' compatible types in order to match a certain function parameter type definition. Types that are not inferred cannot be changed and a compile error will result if no match to a function definition can be made.

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

---

## Symbol Names

- Local Names are parsed out of the source file and converted to canonical format.
- External Names are read from an assembly file and converted to canonical format.

A Symbol (in a Symbol Table) is always stored under its canonical format.

All references to a Symbol are stored with both native (local or external) name and canonical name.
All definitions of a Symbol are stored with both native (local or external) name and canonical name.

An AstIdentifier is basically a combination of a Symbol Name and it's location in the source code (if local). The identifier also tags what type of Symbol the name represents (Enum, Struct, Function, Parameter etc).

Symbol Names all have the namespace of the Symbol Table they are in (hierarchically). Except for module Symbols definitions, they are located in the Symbol Table where they were referenced but maintain their own namespace hierarchy. Module Symbol references are stored in the same Symbol Table as the referenced module is in, but their namespace has to be overridden to the namespace of the module -not the namespace of the symbol table. We want to keep references in the Symbol Table of the scope they were encountered in (not moving them).

External names can be referenced locally without a namespace. By linking up the Symbols between reference and definition (during the resolve names phase) the actual namespace of the reference should become clear.

Native names (local or external) and canonical names both have a namespace and a name.
Nested external symbols use their declaring type name as a namespace. Both native and canonical names can also have name extensions for template and generic parameters. These extensions are built up during AST traversal and need to be mutable.

AstName: Namespace (string), Name (string), prefix (string), extension (counts)
AstSymbolName: Native: (AstName), Canonical: (AstName)

Function (method) names can have prefixes (get_, set_) that are preserved in the canonical name (specifically the '_').

> How are external Methods (Namespace.Type.Function) named?

When emitting code the Symbol Name is in external format, that is the name formatted to the standard .NET naming convention. An external name already has this format (duh). A Code Attribute will hold the local name if available. The canonical format can always be regenerated from either the .NET name or the local name and is not stored.

---

`Namespace` can have zero, one or more '`.`' separators.

Local (from source)

- `Symbol` - simple local names
- `Symbol.Member` - Struct/Enum name navigation
- `Namespace.Symbol` - fully qualified names
- `Namespace.Symbol.Member` - fully qualified name navigation

External (from assembly)

- `Namespace.Type` - default naming
- `Namespace.Type.Member` - member full name
- `Namespace.Type.Nested...` - nested type naming (recursive)
