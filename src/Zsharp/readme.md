# Zsharp

## TODO

- Check unit test coverage
- More tests on syntax errors (non-happy-flow).
- Compiler refactor AstError to info/warning/error
- Expression Operators may need to increase the resulting type (U8 * U8 = U16).
- Emit integer type sizes (un)signed: (U/I8-U/I64)
- Remove parent from TypeReference and share instances (instead of copy).
- grammar: fix how the dot works in module_name.
- grammar: change Variable_def_typed_init so it can reuse Variable_def_typed (as child)?
