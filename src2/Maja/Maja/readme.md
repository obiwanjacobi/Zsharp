# Maja

The Maja library contains common types used by the Z# compiler.

The Maja library itself is implemented in C# - 
because the compiler now also emits C#, that seemed a logical choice.

## Conversion

Type Conversion functions are implemented in checked and unchecked versions.

## Types

- `Array<T>` implemented a constructor function using generics.

- `Opt<T>` the optional (maybe) type.

---

> TODO

- Wrapper types: Opt<T>, Err<T>, Mut<T>, Atom<T>, (Mem<T>? Ptr<T>?)
- Operator functions (checked/unchecked?)
- Code generation for Operator and Conversion functions.

---

## Code Generation

Generate the Operator Functions and the Conversion Functions.

- target file name
- target class name
- data types involved (return and parameters)
- operators: saturate,wrap-around, exception and error
- specific impl for each type (cast params to ret-type)
- decide metafile type (xml, json, yml or C#)
- decide text generation tech (reuse EmitCS from compiler?)
