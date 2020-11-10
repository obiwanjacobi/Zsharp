
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
 - or - | Primitive .NET Type |
module variable [name] | private static {Type} [name] | module class
function variable [name] | local {Type} [name] | static method
function self parameter | this parameter | static method

> For a Console application the Main function will be implemented by a compiler-generated `Main` function that calls the entry point of the program.

---

## TODO

- expressions
- error handling (exceptions/Error/Err<T>)
- defer
- arrays
- Imm\<T>
- Opt\<T>
- Bit\<n>
- Ptr\<T>
- Range/Iter/Slice (Span<T>)
- name/identifier matching and representation (case insensitive)
- Memory Heap Allocation. Could be as simple as a wrapper `class HeapAlloc<T> where T : struct` to get a struct on the heap. Look into Boxing.
