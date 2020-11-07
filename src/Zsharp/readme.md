# Zsharp

## TODO

- Check unit test coverage
- rethink ResolveSymbols/ResolveTypes. Do these need to be separate or integrated?
- Function overloading: need function parameter types for unique symbol name - not available during node building.
- AstFunctionParameterDefinition: add IAstSymbolEntrySite impl.
- More tests on syntax errors (non-happy-flow).
- Expression Operators may need to increase the resulting type (U8 * U8 = U16).
- Remove parent from TypeReference and share instances (instead of copy).
- grammar: fix how the dot works in module_name.
- grammar: change Variable_def_typed_init so it can reuse Variable_def_typed (as child)?
