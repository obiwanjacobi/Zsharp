# .NET Interop

How to do 2-way interop with .NET (IL)?

Z# assemblies can be used in another .NET project (Z# to .NET).

.NET assemblies can be used in a Z# project (.NET to Z#).

## Z# to .NET

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

---
---

## .NET to Z#

How are referenced (imported) .NET code constructs translated to the Z# language constructs.

### Identifiers

> How are Identifier in .NET represented in Z#?

We only need to make certain that types start with an upper case letter and functions with a lower case letter. We may have to keep track of this mapping during compilation.

Matching Identities follows the Z# rules.

> How are .NET namespaces represented?

Each namespace in .NET is a module in Z#.

> Type Identification

Z# has a simplified type identification system that needs to be mapped from .NET.

.NET | Z# | Notes
--|--|--
`typeof()` | `#typeinfo` |
`TypeCode` | `#typeid` | Higher values are custom types.

At runtime Z# code can always use the `Object.GetType()` method to gain access to type info.

### Types

How to map the primitive .NET type to Z# types?

.NET | Z# | Notes
--|--|--
Boolean | Bool |
Byte | U8 |
SByte | I8 |
Char | U16 |
Decimal | ?? | 96 bits!
Double | F64 |
Single | F32 |
Int32 | I32 |
UInt32 | U32 |
Int64 | I64 |
UInt64 | U64 |
Int16 | I16 |
UInt16 | U16 |
Object | ?? | `Any` type?
String | Str | StrUtf16?
DateTime | ?? |
DbNull | ?? |
dynamic/Object | Dyn |
void | Void | ?

> Casting and Conversion

The standard Z# cast functions (use the Type name as cast function name) will map nicely.

> The C# `is` and `as` keywords

The `is` keyword is a type test. In Z# the `#typeid` can be used to test for type equality. However this does not include interfaces.

The `as` keyword is a dynamic type cast (at runtime).

There are several options in Z# to do this.
See [Test for Interface Implementation](./lang/interfaces.md#Test-for-Interface-Implementation) for more details.

### Null vs Optional

The `null` keyword is not be available in Z#. Using the nullable reference attributes types will be represented as `Opt<T>` as indicated by the attributes. .NET methods that do not have these attributes are assumed to be nullable and always translate to `Opt<T>`.

Nullable value types will also be represented with `Opt<T>`.

### Loops and Enumerators

The .NET `IEnumerable<T>` interface is mapped to one of the `GetIter` functions.

The .NET `IEnumerator<T>` interface is mapped to the `Iter<T>` type, which basically performs the same function and has the semantics.

How the loops are generated in IL does not really matter.

### IDisposable

.NET Types that implement IDisposable can be used as is. Z# can call the Dispose method - no problem.

The C# `using` construct (which is a try-finally block) has to be build with `defer` to 'schedule' the Dispose being called when the scope is exited. This would do: `defer myobj.Dispose()`.

Note: `defer` has not received a whole lot of thought yet, so things may change.

### Exceptions vs Errors

In principle all .NET Exceptions are to be translated to `Error` object representations.

All .NET constructors, methods, properties and events will be marked as `Err<T>` identifying that a potential exception could be thrown there. That probably will make using imported .NET code very verbose to work with.

Z# does not have any constructs for catching exceptions specifically - based on type - but it does have a `catch` keyword that will work.

We could translate a `match` on an `Err<T>` return into a typed catch clause.

The `try` keyword is on a per function-call basis. That may not be optimal for working with exceptions.

The `defer` keyword can be used as a finally handler, although this also is on a per call basis.

Throwing an exception from Z# code is passing an instance in the `Error` functions which will see its an `Exception` and throw it. The code _should_ to be generated in such a way that the stack trace represent the position where the exception was passed into Error - not Error itself.

### Classes

How to deal with Classes/Objects and inheritance?
(calling and implementing)

How do you deal with a Reference-Type when Z# is mainly Value-Type focussed.

> Classes are represented as structures with functions.

Public fields are represented as a struct with the same name as the class (with a capital first letter).

Public instance methods are represented as (`self`) bound-functions.

Public static methods are normal functions with the name of the class prefixed to the method name. (How do we parse back a `ConsoleWriteLine` to `Console.WriteLine`?) The import statement can contain the static class name and will expose all its static members (`import System.Console` allows `WriteLine("")`). Using an import alias is also supported on a per function (method) basis (`import ConsoleWriteLine = System.Console.WriteLine`).

Public extension methods are represented as self-bound (this) functions (if we can detect those).

> How do you instantiate a new instance of a .NET class?

In Z# you would need an allocator to place an object on the heap.
Then there has to be a .NET (implicit) allocator. `GC`, to make it clear it is a Garbage Collected heap, which does not clash with the static methods on the (System) `GC` class.

```csharp
import System
...
o = Object(GC)  // call parameter-less ctor
```

Any constructor parameters are specified after the allocator.

> How do you derive from a .NET (abstract) base class.

Define a new type that derives from the .NET base class

```csharp
import System

MyType: Object
    MyExtraField: U8

// we could even leave out the GC allocator to cut down on the noise and ceremony.
MyType: (self: MyType, [GC: GCAllocator,] p: U8)
    Object(self, GC)        // call base class constructor
    t = GC.MyType
```

Refer to [Type Constructors](/lang/types.md#Type-Constructors) to see how to call base class constructors. For .NET classes the call to the base class constructor function must come first.

> Polymorphic Class

For each .NET class that has public (protected) members an implicit interface is available. This allows the Z# code to approach the class in a polymorphic manner. If the class has a base class its interface is also 'derived' from the base class' interface.

- There is one issue of what to name that interface not to cause name clashes with other interfaces the class may implement. Perhaps prefix it with a specific symbol?

.NET class properties are represented as `get` and `set` functions on the class interfaces.

- Z# does not allow fields on interfaces.

> Overriding virtual / new Methods

A Z# function bound to the derived class `self`, with the exact same name (Identifier) and parameters will override the base implementation.

```csharp
import System:

MyStruct: Object

ToString(self: MyStruct): Str
    // cast self to base type to call overridden fn
    return ToString(self.Object())

o: Object = MyStruct
    ...

s = o.ToString()    // calls ToString on MyStruct
```

- what if the overridden method is not virtual (error?)
- what if calling base method is not available or not the direct base class (but deeper)? (error?)

Implementing a (C#) `new` function to replace an old function on a class is not supported. Simply give the new function a (slightly) different name.

> .NET class method overloads.

- what type of method resolving is required?
- can it all be done at compile time?
- should we give overloaded methods a special name to differentiate between them (method, method1, method2)?

### Interfaces

Interfaces (querying, calling and implementing)

Interface methods are translated to self-bound template interface functions sharing the name of the .NET interface.

### Delegates and Events

A delegate is a wrapper around an instance pointer and a function pointer.

A `Delegate` type will be introduced to represent the C# delegate constructs. This `Delegate` type contains a function pointer for a specific (`self`-bound) function prototype and an optional instance/self pointer.

Multi-cast delegates are a linked list of delegates that will all fire when the delegate is invoked.

Events are a standardized delegate signature (sender, args) and a 'property' mechanism for adding and removing delegates. Similar to Properties, Events will be exposed as `add` and `remove` functions.

### Lambdas

...

---

- Linq / Expressions
- Threading/Tasks/async/await
- Arrays (build a custom stack array? do the array IL instructions still work?)
- Pointers (normal Ptr, strong/weak, ref-counted, garbage collected...)