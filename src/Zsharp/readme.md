# Zsharp

---

## TODO

- Check unit test coverage
- More tests on syntax errors (non-happy-flow).
- SymbolTables contain nested symbols (e.g. field names of structs) without scope/owner identifier.
- Expression Operators may need to increase the resulting type (U8 * U8 = U16). 
    RetVal of operator overload determines resulting type.
    Type of target (variable) determines resulting type.
- Resolve Expression operators to (intrinsic or custom) function names.
- Emit integer type sizes (un)signed: (U/I8-U/I64)
- Add (custom) Type Conversion (Type names as function names)
    Current intrinsic conversions are hardcoded in grammar (not ideal).
    Conversions are simply functions with same name as (target) type and a self paramter of source type.
    Type Constructor functions are very similar - but without the self parameter.
- Add Custom Data Types (and export)
- implement discard as variable: `_ = fn()`
- Struct field init is always done using a secondary instance - not always necessary (ctor).
- Can we move ParserRuleContext Context property to AstNode?
- Add SymbolEntrySite to TypeDefinition?
- May need to split Function Type ([template] params + retval) from function definition name.
    Ptr to Function needs this type - not the definition name.
- TypeReferences of built-in types are added to root SymbolTable that contains definition.
    Need to see if this is a problem when multiple files are compiled using the same root symbol table.

---
