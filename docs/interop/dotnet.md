
# Z\# to .NET

How are the various Z# language constructs represented in .NET.

Z# | .NET (C#) | Parent
--|--|--
project [name] | namespace [name] | Assembly [name]
module [name] | public static class [name] | namespace
\<module w/o exports> | private static class [name] | namespace
export function [name] | public static [name] | module class
function [name] | private static [name] | module class
export struct [name] | public struct [name] | module class
struct [name] | private struct [name] | module class
export enum [name] | public enum [name] | module class
enum [name] | private enum [name] | module class
export type [name] | public struct [name] | module class
type [name] | public struct [name] | module class
 \- or - | Primitive .NET Type |
module variable [name] | private static {Type} [name] | module class
function variable [name] | local {Type} [name] | static method
function self parameter | this parameter | static method

---

> .NET `struct` (heap) vs `ref struct` (stack). Z# does not control the stack/heap choice by type but per case instance... How?

## Enum

.NET only supports integer base types for enums. That means that our `Str` and `F32` or `F64` examples do not translate directly into a .NET enum. Instead they will be generated as a static class with constant field values for these specific (base) types.

## Struct

A value type or struct in .NET cannot be inherited. Z# defined structs with base types can be written out where the base fields are duplicated in the derived type (in order).

For polymorphism in Z#, the compiler has to check if the fields (in order) match those of the requested type. This leans towards duck-typing in that if the fields match - you must be the same type, which is also how Z# handles interfaces: if the functions match, you must be the same interface.

For Custom Data Types like `MyType: U8` where a struct derives from a native Type, we may need to introduce a hidden field (starts with '`_`') with a fixed name (like `_base`).

---

## TODO

- expressions
- error handling (exceptions/Error/Err\<T>)
- defer
- arrays
- Imm\<T>
- Opt\<T>
- Bit\<n>
- Ptr\<T>
- Range/Iter/Slice (Span\<T>)
- Union Types may be difficult
- name/identifier matching and representation (case insensitive)
- Memory Heap Allocation. Could be as simple as a wrapper `class HeapAlloc<T> where T : struct` to get a struct on the heap. Look into Boxing. There only need to be a (simple) way to indicate a heap target.

How to indicate in the syntax an object is on the heap?

```csharp
fn: ()
    // stack
    a = 42
    // heap
    b: Mem<U8> = 42     // wrapper type
    c = Mem(42)         // conversion with type infer
    d: U8@ = 42         // Mem<T> type operator?
```

Cannot reuse `Ptr<T>` type operator `*` because a Z# struct is also a `struct` in .NET. A pointer to a struct would be a `ref struct`. A pointer to a heap-allocated instance would simply be a .NET managed reference.
