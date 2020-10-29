# Zsharp

## TODO

- Check unit test coverage
- rethink ResolveSymbols/ResolveTypes. Do these need to be separate or integrated?
- More tests on syntax errors (non-happy-flow).
- TypeReference resolution scope? We cannot handle reference to external types yet.
- Expression Operators may need to increase the resulting type (U8 * U8 = U16).
- ResolveTypes/Symbols: Remove support for `b = b + 1`. Invalid var reference.
- grammar: fix how the dot works in module_name.
- grammar: change Variable_def_typed_init so it can reuse Variable_def_typed (as child)?
