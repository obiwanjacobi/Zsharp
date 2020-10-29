# Zsharp

## TODO

- Check unit test coverage
- change grammar for Variable_def_typed_init so it can reuse Variable_def_typed (as child)?
- Variable/Assignment. Always use assignment for all variable decls/refs? Variable no longer a CodeBlockItem.
- rethink ResolveSymbols/ResolveTypes. Do these need to be separate or integrated?
- Create AstErrors during Semantic processing.
- A pipeline to sequence the steps necessary for a full compile?
- TypeReference resolution scope? We cannot handle reference to external types yet.
- Expression Operators may need to increase the resulting type (U8 * U8 = U16).
- ResolveTypes/Symbols: Remove support for `b = b + 1`. Invalid var reference.
- grammar: fix how the dot works in module_name.
