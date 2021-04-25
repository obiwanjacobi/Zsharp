# Z\# to .NET

> TBD: Should we drop generating IL and simply transpile to C# and use Roslyn (and all the updates it gets) to do the real work?
Are there any features that do not map to C# in any way?

How are the various Z# language constructs represented in .NET.

Z# | .NET (C#) | Parent
--|--|--
project [name] | namespace [name] | Assembly [name]
module [name] | public static class [name] | namespace
\<module w/o exports> | private static class [name] | namespace
export function [name] | public static [name] | module class
function [name] | private static [name] | module class
function [name] (self) | private \<Self> [name] | Self class
export function [name] (self) | public \<Self> [name] | Self class
export struct [name] | public struct [name] | module class
- or - | public record [name] | module class
struct [name] | private struct [name] | module class
- or - | private record [name] | module class
export enum [name] | public enum [name] | module class
enum [name] | private enum [name] | module class
export type [name] | public struct [name] | module class
- or - [name] | public record [name] | module class
type [name] | public struct [name] | module class
 \- or - | Primitive .NET Type |
module variable [name] | private static {Type} [name] | module class
function variable [name] | local {Type} [name] | static method
function self parameter | this parameter | static method

---

> .NET `struct` (heap) vs `ref struct` (stack). Z# does not control the stack/heap choice by type but per case instance... How?

Hidden function prefixes:

- `get_` Property getter.
- `set_` Property setter.
- `init_` Property initializer. Are C# init props translated to `set_`??
- `op_` Operator implementation.

## Enum

.NET only supports integer base types for enums. That means that our `Str` and `F32` or `F64` examples do not translate directly into a .NET enum. Instead they will be generated as a static class with constant field values for these specific (base) types.

## Struct

A value type or struct in .NET cannot be inherited. Z# defined structs with base types can be written out where the base fields are duplicated in the derived type (in order).

For polymorphism in Z#, the compiler has to check if the fields (in order) match those of the requested type. This leans towards duck-typing in that if the fields match - you must be the same type, which is also how Z# handles interfaces: if the functions match, you must be the same interface.

For Custom Data Types like `MyType: U8` where a struct derives from a native Type, we may need to introduce a hidden field (starts with '`_`') with a fixed name (like `_base`).

---

## TODO

First problem is the .NET Reference and Value Types. Although initially Z# aimed to favor structs as value types, it might be a good idea to implement struct as C# records, that is reference types with value semantics.

Related to this is Z#'s notion of the `Ptr<T>` wrapper type. I cannot see a good way to manage object references per variable reference when .NET attaches this semantic to the (reference) type. A variable to a reference type cannot ever not-be a reference (pointer). A value type can be passed by reference though.

The second problem is error handling. Z# has no support for exceptions (as of yet) and translating that to method based error handling on function return values is problematic. We may be forced to support exception and try-catch-finally.

- expressions
- error handling (exceptions/Error/Err\<T>)
- defer
- arrays
- Imm\<T>
- Opt\<T>
- Bit\<n>
- Ptr\<T>
- Range/Iter/Slice (Span\<T>)
- Union Types may be difficult (OneOf library on github?)
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

Cannot reuse `Ptr<T>` type operator `*` because a Z# struct is also a `struct` in .NET (what if we used record?). A pointer to a struct would be a `ref struct`. A pointer to a heap-allocated instance would simply be a .NET managed reference.
