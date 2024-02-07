# Todo

- [ ] Literal array and List syntax is `()` and not `[]`.
- [ ] What is the syntax for static array construction (fixed size)? `[n]` or `Array<T>(n)` or...?
- [ ] Align array arithmetic across operators.md (lexical) and array.md (type). `++` vs `+` etc.
- [ ] Address `Imm<T>` as a type modifier (only for structs?)
- [ ] Talk about (ReadOnly)Span<T> with reference to `Ref<T>`, `Out<T>` and `Mut<T>`.
- [ ] Make structure instantiation consistent with vars `s : MyStruct = ...` (used in nested structs example) or `s := Mystruct ...`? Also take into account the potential structure parameters (see templates).

## Done

- [x] Default to immutability. Replace `Imm<T>` with `Mut<T>` (reverse). `Mut<FunctionType>` is illegal in decls.
- [x] Move template value (non-type) parameters out of the `<>` and into the normal parameters `()` with a `#`.
