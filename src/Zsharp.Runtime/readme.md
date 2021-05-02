# Z# Runtime

The Z# Runtime library contains common types used by the Z# compiler.

The Z# Runtime library itself is implemented in C# - 
because the compiler now also emits C#, that seemed a logical choice.

## Conversion

Type Conversion functions are implemented in checked and unchecked versions.

---

> TODO

- Wrapper types: Opt<T>, Err<T>, Imm<T>, Atom<T>, (Mem<T>? Ptr<T>?)
- Operator functions (checked/unchecked?)
