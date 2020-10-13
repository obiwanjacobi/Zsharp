# Z\# Code Generation

Translation from Z# to .NET:

Z# | .NET (C#) | Parent
--|--|--
project [name] | namespace [name] | Assembly [name]
module [name] | public class [name] | namespace
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

---

## TODO

- expressions
- error handling (exceptions)
- defer
- arrays
- Imm\<T>
- Opt\<T>
- Bit\<n>
- Ptr\<T>
- name/identifier matching (case insensitive)
- Memory Heap Allocation. Could be as simple as a wrapper `class HeapAlloc<T> where T : struct` to get a struct on the heap. Look into Boxing.

.NET features not directly supported by Z# but needed for interop.
- delegates/events
- classes/derive from classes
- implement interfaces
-
