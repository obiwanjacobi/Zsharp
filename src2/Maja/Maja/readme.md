﻿# Maja

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
