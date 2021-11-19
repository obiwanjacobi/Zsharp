# Types

There are built-in types that define the basic data storage widths and semantics.

Several flavors of custom types can be defined:

- [Enumeration](./emums.md). A grouped set of named values.
- (Custom) Data Types. Types that restrict the built-in data types further.
- [Structures](./structures.md). A group of data fields always together.
- Related Types. Types that are based on existing types but differ in a small way. For instance a read-only version of a structure or the common fields from multiple other types.

There is a basic syntax that all types declarations follow:

```csharp
TypeName: BaseType
    #meta = value   // data type constraints (#)
    Enum = value    // enum named values
    Field: Type     // struct fields
```

`TypeName`: The name of the type.

`BaseType`: (optional) the name of the type this new type is based on (derives from).

Then each type has its own way of specifying its implementation:

`#meta` refers to a compile-time meta property that can be used in a DataType rule to restrict the value range of the defined type. See 'Custom Data Types' further down.

`Enum` is used to define a named value type. It is optional to specify an explicit value. See also [Enums](enums.md).

`Field` sets the defined type up as a data structure containing one or more fields. See also [Structures](structures.md).

> **_All type names start with a Capital letter_**

> TBD: instead of making a distinction between structs and enums and custom types, have one type that can be a combination of any of these aspects - except custom data types are always a singular value, but can be combined with enum-values.

---

## Built-in Types

The built-in data type form the basic building blocks for creating structures.
There are built-in types for integers, floating point, string and boolean.

Other .NET types are not a native part of Z# and can be used as an external library type.

> .NET has other types in `System.Numerics` that may be of interest to include in the future as native Z# types.

### Integers

The type names have been shortened to an absolute minimum. First:

- U - unsigned integer
- I - signed integer

> TBD: Or should we use 'S' for signed integers?

followed by the width in the number of bits.

- 8
- 16
- 32
- 64

```C#
U8 U16 U32 U64
I8 I16 I32 I64
```

These map to respective .NET types:

- Unsigned: `byte`, `ushort`, `uint` and `ulong`.
- Signed: `sbyte`, `short`, `int` and `long`.

> TBD: Do we want an autoscaling `Int`eger type? (.NET `System.Numerics.BigInteger`)

### Floating Point

The floating point data types are:

```C#
F16, F32, F64, F96
```

These map to respective .NET types: `Half`, `Single`, `Double` and `Decimal`.

### Strings

A 'string' of characters of text (UTF-16).

```C#
Str         // mutable string: StringBuilder
Imm<Str>    // immutable string: String
```

This type maps to the .NET `string` or `StringBuilder` depending on if `Imm<Str>` is used.

---

I am thinking of making each encoding available in a dedicated type which can be converted to one another.

```csharp
StrASCII    // 8-bit
StrUtf7
StrUtf8     // most common
StrUtf16    // explicit .NET string
StrWin1251
StrWin1252
StrIso8859
```

For these specialized string, no character type is available (other than U8/byte), they would basically encapsulate byte buffers and do not derive from `Str`.

The encoding and decoding can be viewed as a conversion between different string types.

```csharp
s = "Hello World"   // Str (UTF16)
s8 = s.StrUtf8()    // convert

loop c in s
    // use c: C16

loop c in s8
    // use c: U8
```

The conversion is done using the .NET Text Encoding types from the BCL.

#### Secure String

> TBD

`System.Security.SecureString`

#### Str-Of-T

> TBD

`StrOf<T>` to allow a described string. `T` describes the structure of the content of the string. `StrOf<EmailAddress>` where `EmailAddress` contains validation (regex?).
Related to custom data types.

---

### Character

A single character used in UTF-16 strings.

```csharp
C16
```

This type maps to the .NET `char`.

---

### Boolean

The boolean data type is defined as:

```C#
Bool
```

It can only have one of two values: `true` or `false`.

This type maps to the .NET `bool`.

---

### Bits

The `Bit` type is parameterized to specify the number of bits the value contains.

Here the example declares a `Bit` type that contains 4 bits (nibble):

```C#
Bit<4>
```

When `Bit`s are stored, the closest fitting data type is used. So a `Bit<6>` would take up a single byte `U8`, while a `Bit<12>` would take up two bytes `U16`. `Bit`s are always interpreted as unsigned and stored in the lower bits of the storage type. The upper unused bits are reset to zero.

This type maps the .NET `System.Collections.BitArray` or `System.Collections.Specialized.BitVector32`. Possibly some of `System.Numerics.BitOperations` will be used for some of the operations.

---

### Date and Time

DateTime, Date(only) and Time(only).

---

### Function Type

The function type `Fn<T>` is used when the type of a function is used in code but no [Function Interface](interfaces.md#Function-Interface) is available.

```csharp
// returns a void function (fn: ())
makeFn(p: U8): Fn
    ...

f = makeFn(42)
f()         // call returned function
```

Note that the type of a function has a specific syntax:

`(parameter-types): return-type`

> Templates?

```csharp
// returns a function that takes one param (U8) and returns a Str
makeFn(p: U8): Fn<(U8): Str>
// returns a function that takes two params (Str and U8) and returns a Bool
makeFn(p: U8): Fn<(Str, U8): Bool>
// returns a function that takes one param (U8) and has no return
makeFn(p: U8): Fn<(U8)>
```

This type will map to the .NET `Func<T>` and `Action<T>` types depending on the return type.

---

### Void

> In light of .NET interop we need to rethink this.

A special type to allow to be explicit when there is no (function return) Type. Acts as the functional `Unit` in that it has only one value: itself and therefor holds no information.

```csharp
VoidFn: ()    // no return type: Void
    ...

v = VoidFn()    // legal: v => Void
// you can't do anything with v, though.

NoParamsFn: (Void): U8    // Error: Void not allowed here
    ...
```

Introducing the `Void` type removes the necessity to distinguish between functions with or without a return value. See the 'Void' topic in [Functions](functions.md) for more info.

---

## Literal Numerical Values

Literal values are commonly use in programs and by default the compiler will assign the smallest data type to fit the literal numerical value. There are times when you want to override that, however.

```csharp
a = 42          // a: U8
b = U16(42)     // b: U16
c: U32 = 42     // c: U32
```

We are simply calling a dedicated constructor function with the literal value.

> Coercing literals to bigger (compatible) types.

```csharp
// some sort of postfix?
a = 42L // U64?
```

---

## Custom Data Types

(Constrained Types? / Type Constraints)

There is an easy way to create data types to differentiate data at a type level. By using different types the purpose of the data become even more clear.

The idea here that you can define the information your application deals with without any context. These information definitions can be combined to form larger concepts (e.g. Person).

> Only simple data types can used as base type. ?Is this still needed? Why?

```csharp
Age: U8 _           // no rules
PersonName: Str
    #length < 100   // rules
    #length > 0
```

```csharp
Age: U8 _
PersonName: Str _

a: Age = 42
name: PersonName = "John"
```

The use of `_` is optional in most cases. It signifies that nothing follows the declaration.

You do have to use the explicit type on the variable declaration, using defaults will yield standard types (U8 and Str).

Here's an example of the use of data types.

```csharp
MetricLength: U16 _
ImperialLength: U16 _

ml: MetricLength = 200      // 200 meter
il: ImperialLength = 200    // 200 yards

loa = ml + il       // error: cannot add different data types

// overloading the + operator would fix this error
Add: (left: MetricLength, right: ImperialLength): MetricLength
    ...
```

```csharp
// adding a convertor method
MetricLength: (l: ImperialLength): MetricLength
    ...
// allows this code
loa = ml + il.MetricLength()
```

Operators of the underlying type can **NOT** be used. _**Data Types are always more specific and restrictive than the underlying type**_. New operator implementations have to be created using the dedicated function names mapped to each operator. It is a compile error if such a function is not found for the operator in use.

> It is strange that a custom data type 'derives' from a base type but is not polymorphic to that base type!? If we make this polymorphic as one would expect, all operators/functions of the underlying base type can be used. If a custom data type needs to be more restrictive it can overload the operator based on its own type.

> I think in the majority (if not all) of cases these functions can be generated by the compiler when rules are defined. But overriding with custom functions must always be possible.

> There may be some operators of the underlying type that can safely be used (compare?). The problem mainly exists for operators that modify the value, not for operators that report on the value. There are also other functions that take the underlying type as a (self) parameter that can be reused (will compile). Here we cross into cross cutting concerns: Printable, Orderable etc...

```csharp
Age: U8 _
a: Age = 42
// use conversion to get to underlying types
u = a.U8()      // u: U8
```

> What result type promotion will there be for arithmetic operators on custom data types? Will simply the return type of the operator-function determine the result type?

Based on the rules of a Custom Data Type, the value range of the result can be determined at compile time and an appropriate (smallest) return type can be chosen.

Note that numerical literals could also be thought of restricted custom data types (in a way). A constant value of 2 only has a small impact on the number of extra bytes needed for the result after an arithmetic operation.

The way data types differ from using aliases is in the use of type-bound functions.

```csharp
Alias = U16
DataType: U16 _

baseFn(self: U16, p: U8)
    ...
aliasFn(self: Alias, p: U8)
    ...
typeFn(self: DataType, p: U8)
    ...

a: U16 = 0x4242
a.baseFn(42)        // U16 = Alias
a.aliasFn(42)       // Alias = U16
a.typeFn(42)        // error! U16 != DataType

d: DataType = 0x4242
d.baseFn(42)        // error! DataType more specific than U16
d.aliasFn(42)       // error! DataType is not Alias (or lower to base??)
d.typeFn(42)        // ok
```

Custom Data types cannot have any fields. A struct can be composed of fields using (only) data types.

```C#
Age: U8
    #value = 0
    #value = [2..101]   // Range syntax
Mode: Str       // this would be a Str-Enum...
    #value = "A"
    #value = "B"
    #value = "C"
Length: I16
    #value > 0
```

> overload/override value setter (assignment operator?) to do custom validation? Can this code be generated at compile-time by the compiler?

> Physical units problem. Can we use Custom Data Types to define a physical units library? Example: `acceleration = speed * time-squared` (composition?). Also `1000m = 1km` (prefixes on dimensions)
Units (m) with value(quantity)-scope (minutes), scaled units (km). Perhaps we call these Semantic Types and will have a value, a scale and a unit...?

```csharp
// these mappings could also be enums?
ScaleKilo:
    #1000 => kilo
    #100 => hecto
    #10 => deca
    #0 => _
    #0.1 => deci
    #0.001 => milli
    #0.000001 => micro
UnitMeter:
    #m => meter
MeterValue: SemanticType<ScaleKilo, UnitMeter>

scale: (val: MeterValue, scale: ScaleKilo): MeterValue
    ...

// ??
```

---

### Literal Values

Make a literal value have a custom data type.

```csharp
MyType: U16 _

a = MyType(42)  // passes all rules
```

```csharp
SmallType: U8
    #value = [0..4]     // 0,1,2,3

a = SmallType(42)       // Error: does not pass rules
```

```csharp
MidType: U8 _

// does not fit into base type
a = MidType(275)        // Error: does not pass rules
```

Again, we're simply calling dedicated construct functions.

---

### Custom Data Constraints

The rules that can be defined after the type declaration narrow the value range based on its base-type - in a declarative manner.

By default the specified rules are ANDed together in that all the specified rules must pass before a value assignment will succeed.

> How to create ORed rules?

```csharp
CustomDataType: U8
    #value > 10 or #value = 0
```

> Support ranges as constraint values.

```csharp
CustomDataType: U8
    #value = [1..43]    // 1 - 42
```

String-rules may need extra features to match different aspects of the value. The goal is NOT to create a Regular Expression engine, but just cover the basic stuff. For complex validation a custom validation function (override) has to be defined.

```csharp
CustomDataType: Str
    #value += "Abc"     // starts with
    #value =+ "Abc"     // ends with with
    #value +=+ "Abc"    // contains
    #value -= "Abc"     // does not start with
    #value -=- "Abc"    // does not contain
    #value <> "Abc"     // not equal to
```

> TBD: do we want a custom error message text to go with the rule?

```csharp
CustomDataType: Str
    #length <= 100, "Too long"
```

---

### Custom Data Type Conversion

Any conversion to or from a custom data type must be hand written. The compiler cannot know how/what to convert when/where.

```csharp
// custom types
Hour: U8 _
Minute: U16 _
Second: U16 _

// conversion functions
Minutes: (self: Hour): Minute => self * 60
Seconds: (self: Minute): Second => self * 60
Seconds: (self: Hour): Second => self * 3600

h: Hour = 2
m = h.Minutes()     // m = 120
s = m.Seconds()     // s = 7200
s = h.Seconds()     // s = 7200
```

---

## Built-in Wrapper Types

Several wrapper types are used to add meaning to other types.

Type | Meaning
--|--
`Err<T>` | T or Error
`Opt<T>` | T or Nothing
`Ptr<T>` | Pointer to T
`Imm<T>` | T is immutable
`Atom<T>` | Atomic access
`Async<T>` | Asynchronous function (return type)
`Mem<T>` | Heap allocated (TBD)

Note that the compiler may generate different code depending on where these types are applied.

---

### Types Operators

Some built-in (decorator) types are use so often, it makes sense to provide a shorter version in the form of an operator.

Type | Operator
--|--
`Err<T>` | !
`Opt<T>` | ?
`Ptr<T>` | *
`Imm<T>` | ^

These operators are always used directly _after_ the type (post-fix)

```csharp
// optionaL parameter and optional or error return
fn: (p: U8?): U8!?
```

The precedence can be set according to the order (left to right) the operators appear in the code. Closest to the type is inner: `U8?*` => `Ptr<Opt<U8>>`.

```csharp
o: U8?^     // Imm<Opt<U8>>
p: U8!*     // Ptr<Err<U8>>
```

The optional and error types can be thought of as constrained variant types.

```csharp
Opt<T>: T or Nothing _
Err<T>: T or Error _
```

`Nothing` is an no-value indication for the compiler and is never available to the program.

`Opt` is discussed in more detail [here](optional.md).

`Err` is discussed in more detail [here](errors.md).

`Ptr` is discussed in more detail [here](pointers.md).

> Can we supply a hash code on Immutable types automatically?

```csharp
o: U8?      // optional U8
e: U8!      // U8 or error
x: U8!?     // optional U8 or Error
p: U8*?     // optional pointer to an U8
i: U8^*?    // pointer to an optional immutable U8
```

`Err<T>` is typically (only) used on function return values.

---

### Immutable Types

Any type can be made immutable wrapping it in a `Imm<T>` type.

```csharp
// any old struct
MyStruct
    fld1: U8
    fld2: Str

// an immutable version of MyStruct
ImmStruct: Imm<MyStruct>
// ImmStruct
//   fld1: Imm<U8>
//   fld2: Imm<Str>
```

The compiler will generate a new Type (struct) based on `MyStruct` making all fields immutable. All references to immutable types are tracked as immutable.

---

### Immutable References

`Imm<T>` Can also be used inline for variable definitions.

```csharp
a: U8   // mutable
a = 42  // ok, a = 42

// immutable has to be initialized on declaration
b: Imm<U8> = 42     // init with 42
b = 101             // error! type is immutable
```

Immutable reference to mutable object.

```csharp
a = 42  // mutable U8

// implicit conversion to immutable
b: Imm<U8> = a
b = 101             // error! type is immutable
```

Immutability in these cases it tracked by the references.

> TBD: 'with' in some other languages. Mutations on an immutable type results in a new instance.

```csharp
s: Imm<Struct>
    ...         //   init struct

// s2 is copy of s with changed field
// what syntax?
s2 = s => { fld1 = 42 }     // object construction
s2 = s.Mut({ fld1 = 42 })   // explicit function call1
s2 = s.Clone({ fld1 = 42 }) // explicit function call2
s2 = s + { fld1 = 42 }      // special operator1
s2 = s & { fld1 = 42 }      // special operator2
s2 = s <= { fld1 = 42 }     // special operator3 (mapping)
s2 = s <- { fld1 = 42 }     // special operator4
s2 = s <+ { fld1 = 42 }     // special operator5
```

> TBD: type validation after construction? This is a general issue...

In all these cases the period of time that the new instance is mutable (when a new instance is created and the old and the new values are copied in) is managed by the compiler and shielded from the developer/program.

A Custom Constructor Function for these immutable types has to take a partial type parameter. By default this constructor is generated by the compiler.

What about using tuples? What if we don't want an explicit optional type variance to exist?

```csharp
MyStruct
    fld1: U8
    fld2: Str

// normal constructor function
MyStruct: (p1: U8, p2: Str): MyStruct
    ...

// all fields optional
MyStructOpt : Opt<MyStruct>

MyStruct: (Imm<MyStruct> self, Imm<MyStructOpt> change): Imm<MyStruct>
    ...
```

If no custom constructor is defined for these immutable object manipulations, the compiler will generate one that performs the merging of `self` and the `change`s into a new instance.

---

## Type Alias

Provides a new name for an existing type. Similar to declaring a new type but without any additions.

```C#
// a real new type (struct)
MyType: OtherType<Complex<U8>, Str> _   // _ to indicate no fields

// another name for the same type (alias)
MyType = OtherType<Complex<U8>, Str>
```

During compilation all references to type aliases are replaced with their original types. Compiler-issues _are_ reported using the original type alias name.

---

## Anonymous Types

Only Structure Types can be implemented as a nameless type.

See also [Anonymous Structures](structures.md#Anonymous-Structures).

> `.NET` C# has three different types of anonymous structures: anonymous types (class), value tuples and tuples (class).
https://docs.microsoft.com/en-us/dotnet/standard/base-types/choosing-between-anonymous-and-tuple#key-differences

---

## Type Constructors

Should this go in 'functions'?

> Any function can be a factory (function). Type constructors are checked specifically by the compiler to make sure they return new instances of a type.

A type constructor is a function with the same name as the type it creates and returns.

> Does the constructor function name has to be the exact same as the original Type definition or do we allow the identifier naming rules to apply?

The number and types of parameters a constructor function takes have no restrictions.

```csharp
MyType
    ...

MyType: (p: U8): MyType
    ...

t = MyType(42)
// t is an instance of MyType initialized with 42
```

Using templates

```csharp
MyType<T>
    ...

MyType: <T>(p: T): MyType<T>
    ...

t = MyType(42)
// t is an instance of MyType initialized with 42 (T=U8)
```

A name-clash can occur when you introduce a new Type with a name that (falsely) matches an existing function as its constructor function. Unfortunately that would mean you would have to alias the existing function to a different name at the locations where it is used and that also reference the new Type. See [Import](../modules/import.md) for more on aliasing.

When construction of the type is not trivial and Errors may occur the type constructor function has to have a return type of `Err<T>`.

```csharp
MyType
    ...
MyType: (p: U8): MyType!

t = try MyType(42)
```

> TBD: Find a different constructor function prototype that does not require copying the returned instance from the constructor function to the call site. For larger structs that is a performance hit.

```csharp
// a call-site created empty instance will be passed by ptr.
// errors can still be returned
MyType: (self: Ptr<MyType>, p: U8): Void!

// calling syntax remain the same?
t = MyType(42)
t.MyType(42)        // bounded type syntax? (not clear)

// which results in this code
t: MyType   // default init-ed struct
[t]         // capture scope for isolation
    x: MyType   // 'x': name guaranteed not to clash
    // pass in the temp instance
    try MyType(x.Ptr(), 42)     // try => error handling
    t <= x      // transfer identity (no copying)

// normal code resumes
t.fld1 ...
```

Need to make sure no half initialized instances are the result of a constructor function erroring-out half way through its function. That is why we use a capture.

This (using an explicit `self` parameter) would also solve the problem of calling base-constructor functions when types are derived.

> Can the `self` parameter also be used to call more derived versions of 'overridden' functions (calling a virtual in ctor)?

```csharp
BaseType        // type definition
    ...
BaseType: (self: BaseType)  // constructor function
    ...

MyType: BaseType    // derived type definition

MyType: (self: MyType)  // derived type constructor fn
    // call base constructor function explicitly
    BaseType(self)      // syntax #1
    self.BaseType()     // syntax #2

// use: construct a new MyType instance
t = MyType()
```

The base class constructor function call must be first.

---

### Type Constructor Overloading

A Type Constructor function can be overloaded - normal functions can only be overloaded based on the self parameter.

At Compile-Time the correct overload will be determined and a compilation error will be generated when no suitable overload was found.

```csharp
MyType
    ...

MyType: (p1: U8): MyType
    ...
MyType: (p1: U8, p2: Str): MyType
    ...
MyType: (p1: U8, p2: Str, p3: U16): MyType
    ...

// uses overload with 3 parameters
t = MyType(42, "42", 0x4242)
```

---

## Constrained Variant

Also known as Discriminated Unions.

```C#
OneOrTheOther: Struct1 or Struct2
OneOfThese: Struct1 or Struct2 or Struct3 or Struct4

s: OneOfThese
    ...

v = match s
    s1: Struct1 => s1.fld1
    s2: Struct2 => s2.val2
    s3: Struct3 => s3.bla
    s4: Struct4 => s4.myfld
```

> The type-id is stored with the instance. Access with `#varId` or something?

> A Ptr should still point to the payload of the variant so perhaps store the var-type in front of the main payload.

A constrained variant instance cannot change type during its lifetime.

> TBD: have other means of determining the type of the contents?

```csharp
OneOfThese: Struct1 or Struct2 or Struct3 or Struct4

s: OneOfThese

// a property that matches T
if s.type = Struct1
    ...
```

This would be basically how the type would be stored...

---

## Type Manipulation

Related variations can be created easily from existing types.

```C#
MyStruct
    fld1: U8
    fld2: U16

// make type optional
MyOptionalStruct: Opt<MyStruct>
MyOptionalStruct: MyStruct?     // language supported
// fld1: U8?
// fld2: U16?

// make type read-only
MyReadOnlyStruct: Imm<MyStruct>
MyReadOnlyStruct: ^MyStruct
// fld1: ^U8
// fld2: ^U16
```

Using `Imm<T>` and `Opt<T>` on the base type applies to all fields.

=> Maybe use different types that indicate a full type transformation?
`Immutable<T>` and `Optional<T>` (as well as `Required<T>`)?

Perhaps allow manipulation like in TypeScript 'for each key'...?

> TBD Inverse of `Opt<T>` (required)? Inverse of `Imm<T>`?

```csharp
MyStruct
    fld1: U8?
    fld2: U16?

// ??
NonOptStruct: Required<MyStruct>
```

Make an instance read-only:

```csharp
s: MyStruct
// using a conversion to make immutable
r = s.Imm()
r.fld1 = 101        // error! field is read-only

//-or-  cast will convert
r: MyReadOnlyStruct = s
```

Make an instance optional:

```csharp
s: MyStruct
// using a conversion to make optional
o = s.Opt()
o.fld1 = _        // field is 'nulled'

//-or-  cast will convert
o: MyOptionalStruct = s
```

There can be special syntax for manipulating instances of (for instance) optional types and their fields?

```csharp
MyStruct
    fld1: U8
    fld2: Str

MyStructOpt : Opt<MyStruct>

s = MyStruct
    fld1 = 42
    fld2 = "101"

o = MyStructOpt
    fld2 = "42"     // partial init

x = s + o
// x => MyStruct
// x.fld1 = 42
// x.fld2 = "42"

p = MyStructOpt
    fld1 = 101     // partial init

y = o + p
// y => MyStructOpt
// y.fld1 = 101
// y.fld2 = "42"
```

---

## TBD

Ideas...

The `|`, `&` and `^` operators act on the memory of a type (sort of).

`and` and `or` operate on the logical type.

> In no case can a type ever be empty.

What about marker interfaces?

---

### Unions

> How are shared fields (locations) initialized when two structs have different default values? Or simply init to zero-always.

> What happens if -part of- a field is accessed through another -incompatible- type? For instance: `Str|U8` write `Str="42"` and read through `U8`. (also a problem in C).
COM-interop (.NET) disallows this.

```C#
MyUnion1: Struct1 | Struct2
MyUnion2: Struct1 | Struct2 | Struct3

MyUnion             // all fields share the same memory
    fld1: U8 |
    fld2: U16 |
    fld3: Str |     // trailing | ok
```

> Because there is no `union` keyword, anonymous or inline unions are not possible.

A union can be used with any type or struct.

```csharp
Union1
    fld1: U8 |
    fld2: U16
Union2
    prop1: Str |
    prop2: Bool

// one big union
Union3
    un1: Union1 |
    un2: Union2

// struct with 2 unions
Struct1
    un1: Union1
    un2: Union2
```

---

### Type Commonality

Common fields in all types.

```C#
MyStruct: Struct1 & Struct2
```

A compiler error is generated if no fields are common - the type defined is empty.

---

### Type difference

Think of this as an inverse union.

```C#
Difference: Struct1 ^ Struct2
```

> Can the `|`, `&` and `^` be combined in one declaration? What is the precedence of these operators?

```csharp
MyStruct: (Struct1 & Struct2) | (Struct3 ^ Struct4)
```

---

### Subtracting from Types

Create a new type based on an existing type minus some fields.

```csharp
// syntax?
MyStruct: BaseStruct - { fld1: Str, fld2: I32 }
```

Adding to types is a simple matter of:

```csharp
MyStruct : BaseStruct
    addFld1: Str
    addFld2: I32
```

But more consistent would be something like:

```csharp
MyStruct: BaseStruct + { fld1: Str, fld2: I32 }
```

Or can we write subtracting like this?

```csharp
MyStruct: BaseStruct - 
    fld1: Str
    fld2: I32
```

Which is kind of confusing...

---

### Multiple Inheritance

Type addition.

```C#
MyStruct: Struct1 and Struct2
```

Laid out in memory in order of definition.

> How to move the 'self' pointer? => All info is available at compile-time.

```csharp
MyStruct: Struct1 and Struct2
structFn(self: Struct2, U8)
    ...

s: MyStruct
    ...

s.structFn(42)      // the 'reference' to the structFn has to go past Struct1
// Also goes for Ptr's to sub-parts

// explicit offset? (verbose!)
structFn(s#offset(Struct2), 42)
```

---

## Dynamic Type

> Should dynamic types be taken into account? How would the syntax look and what semantics are attached?

```csharp
d: Dyn              // dynamic type
d.prop1 = 42        // creates a new field (fixed type)

MyFunction: (self: Dyn, p1: U8): Bool
    // does field exist
    if self?#prop1
        return self.prop1 = 42

    // does function exist
    if self?#getMagicValue
        return self.getMagicValue() = 42
    return false

if d.MyFunc(42)
    ...
```

> use of `#` in prop exists test is NOT at compile time - we should not use it.

> What functions are available to which `Dyn` instances?

Calling non-existent functions should raise an (runtime) Error.
Accessing non-existing fields returns 0? -or- always requires a check in the same scope (like optional)?

> Can fields be removed?

```csharp
d: Dyn
d.prop1 = 42    // now you see it
d.prop1 = _     // now you don't
```

> Can functions be assigned to instances?

```csharp
MyFunction: (self: Dyn, p1: U8): Bool
    ...

d: Dyn
// depend on dot-syntax here!
d.MyFunc = MyFunction

if d.MyFunc(42)
    ...

d.MyFunc = _    // function is removed
```

> Parsing free data structures (json) into a dynamic type.

```csharp
d: Dyn <= "{ 'data':'hello world' }"
if d?#data
    // use d.data
```

---

## The Any Type

Represents the .NET `System.Object` type for reference cases.
(External) Type definitions still use `System.Object` as base type.

```C#
funcAny(): Any
    ...

a = funcAny()   // a => Any
v = match a
    n: U8 => n
    _ => Error("Unsupported")

// v => U8!
```

> TODO: check C++ std::any

---

> TBD

Use meta programming to create types.

```csharp
// special type assignment
MyType =: #Type("MyType", ...)
```

Meta programming needs more work.

---

> Other types that need close integration with the compiler?

- Buffer / RingBuffer
- Stream
