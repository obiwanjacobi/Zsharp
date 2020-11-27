# Zsharp

## TODO

- Check unit test coverage
- More tests on syntax errors (non-happy-flow).
- Expression Operators may need to increase the resulting type (U8 * U8 = U16).
- Compute Constant Value for Expressions at compile time.
- resolve expression operators to (intrinsic or custom) functions
- Remove parent from TypeReference and share instances (instead of copy).
- Emit integer type sizes (un)signed: (U/I8-U/I64)
- Add (custom) Type Conversion (Type names as function names)
- Add Structs, Enums, Custom Data Types (and export)
- implement discard as variable: `_ = fn()`
- Can we move ParserRuleContext Context property to AstNode?
- Add SymbolEntrySite to TypeDefinition?
