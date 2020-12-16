# Zsharp

## TODO

- Check unit test coverage
- More tests on syntax errors (non-happy-flow).
- Expression Operators may need to increase the resulting type (U8 * U8 = U16). 
    RetVal of operator overload determines resulting type.
- Resolve Expression operators to (intrinsic or custom) functions
- Remove parent from TypeReference and share instances (instead of copy).
- Emit integer type sizes (un)signed: (U/I8-U/I64)
- Add (custom) Type Conversion (Type names as function names)
- Add Custom Data Types (and export)
- implement discard as variable: `_ = fn()`
- Emit struct field init
- Can we move ParserRuleContext Context property to AstNode?
- Add SymbolEntrySite to TypeDefinition?
- TypeReferences of built-in types are added to root SymbolTable that contains definition.
    Need to see if this is a problem when multiple files are compiled using the same root symbol table.
