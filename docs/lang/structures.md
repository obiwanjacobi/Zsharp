# Structures

Structures are data records of fields.

A name and a set of fields is required in order to define a structure. Structures (that do not derive) cannot be empty - have no fields.

```C#
MyStruct
    field1: U8
    field2: Str
```

Note that the name of the structure has to start with a capital letter, because it is a new type. Also note the absence of any specific keywords like 'struct'.

Here is how to create an instance of a structure and assign its fields with values.

```C#
MyStruct
    field1: U8

s = MyStruct            // instantiate an instance
    field1 = 42         // assigning values
```

A structure can have default values. If no value is explicitly specified each type is initialized with its `#default` value:

```C#
MyStruct
    field1: U8 = 42
    field2: U8 = 42

s = MyStruct
    field2 = 101        // overwrites any value

// s.field1 = 42
// s.field2 = 101
```

---

## Composition

New structure types can be made from other structure types by the following methods:

- Inheritance
- Containment

...or a combination of these.

Here is an example of inheritance:

```C#
MyStruct
    field1: U8

MyDerived: MyStruct
    field2: Str
```

`MyDerived` has two fields: `field1` and `field2`. An instance of  the `MyDerived` structure can be treated as an instance of the `MyStruct` structure.

Inheritance makes the structure bigger.

> Multiple inheritance is not supported at this moment.

Here is an example of containment:

```C#
MyStruct
    field1: U8

MyContainer
    cnt: MyStruct
    field2: Str
```

The `MyContainer` structure still has the `field1` from `MyStruct` but it has to be reached by the `cnt` field. Here is how the structure would be initialized:

```C#
s = MyContainer
    cnt.field1 = 42
    field2 = "OK"
```

> Unions in structures are not supported at this moment.

---

## Bit Fields

> This needs to be revised for .NET interop.

A structure can contain bit fields using the `Bit<T>` type.

So the following example will only take up 1 byte:

```C#
MyStruct
    field1: Bit<2>      // bit 0-1
    field2: Bit<3>      // bit 2-4
    field3: Bit<3>      // bit 5-7
```

Bit fields can be combined with other types.

```C#
MyStruct
    field1: Bit<2>
    field2: Bit<4>
    field3: Bit<4>
    other: U8
    field4: Bit<4>
    field5: Bit<4>
```

The resulting structure has 3 bytes worth of bit fields and one byte for the other field.

> What will be the #offset for a bit field? (byte-offset/bit-offset)

> Should this be a union type?

---

## Nested Declaration

Structures can be declared in a nested fashion:

```csharp
Struct
    nested: NestedStruct
        fld1: U8
        fld2: Str
    name: Str
```

However the `NestedStruct` type is not available outside the structure it was declared in.

> TBD: Can the nested type be anonymous?

```csharp
Struct
    nested
        fld1: U8
        fld2: Str
    name: Str
```

Nested inside a function? (yes)

```csharp
someFn: (p: U8)
    LocalStruct     // only visible inside this function
        fld1: U8
        fld2: Str
    
    s = LocalStruct     // use it
        fld1 = 42
        fld2 = "42"

    ...
```

---

## Structure Layout

The fields of a structure are layed out in the order of their definition starting at the base structure type. No alignment or filler bytes are added.

> `.NET` does the actual layout in memory.

---

## Tables

> Find a way to allow to easily define tables of data using struct types in an array.

```csharp
MyStruct
    fld1: U8
    fld2: Str

arr: Array<MyStruct> = (
    { fld1 = 42, fld2 = "42" },     // by name
    { 101, "101" },                 // in field order
)
```

If we allow this, then we can also move towards easy structure instantiation (ala JS/Json).

```csharp
MyStruct1
    fld1: U8
    fld2: Str
MyStruct2
    first: MyStruct1
    second: MyStruct1

// in order
v: MyStruct1 =
    {
        { 42, "42" },
        { 101, "101" }
    }

// by name
v: MyStruct1 =
    {
        first = { fld1 = 42, fld2 = "42" },
        second = { fld1 = 101, fld2 = "101" }
    }

// structured by name
v: MyStruct1 =
    first =
        fld1 = 42
        fld2 = "42"
    second =
        fld1 = 101
        fld2 = "101"

// or any combination of these??
```

---

## Anonymous Structures

Also known as Tuples.

> `.NET` makes a distinction between (two types of) tuples and anonymous types - Z# does not. We do need to choose how Z# is going to leverage these .NET Tuples in the code generation.

> Use `{}` for object creation syntax.

```csharp
a = { Fld1 = 42, Fld2 = "42" }

// a is a tuple with two fields
x = a.Fld1  // U8
y = a.Fld2  // Str

// deconstruct - creates new vars
(fld1, fld2) = a
// build new tuple from vars
b = { fld1, fld2 }

same = (a = b)
// true: compared on value semantics
```

Preferred is to use field names for tuples, but even those can be omitted but then deconstruction has to be used to unpack.

```csharp
// no structure type name, no field names
x = { 42, "42" }
// C# does this (not for ValueTuple though):
x.Item1 // Error: Item1 does not exist

// to use, must deconstruct in order
(n, s) = x
// n = 42 (U8)
// s = "42" (Str)
```

This is how you reference anonymous types in code.

```csharp
// have to repeat the structure of the type
anoStructFn: (s: (number: U8, name: Str))
// if the anonymous type also has no field names
anoStructFn: (s: (U8, Str))
```

> We want to line up the syntax (and semantics) with the parameters of a function call. Adopting a global rule that 'field-lists' (tuples, function params, deconstructs etc) can be build in-order or named or a combination (see function parameters). Array initialization too? `arr = (1, 2, 3, 4)` - has no field names!

---

## Transformation (Mapping)

> TBD

Allow fields of one structure to be mapped easily to fill fields of another structure.

> What operator to use? `<=`?

```csharp
s1: Struct1
// manual
s2: Struct2
    fld1 = s1.x
    fld2 = s1.y

// by convention
s2: Struct2 <= s1
```

Transform using a custom Type and rules/constraints?

Requires some library plumbing code.

```csharp
// marker type for compiler
Transform<T1, T2>
    ...
// these functions are templates where the mapping rules are compiled into
transform: <T1, T2>(self: Transform<T1, T2>, source: T1): T2
    ...
reverseTransform: <T1, T2>(self: Transform<T1, T2>, source: T2): T1
    ...
```

```csharp
// Transform is the compiler supported marker type.
MapS1ToS2: Transform<Struct1, Struct2>
    #Struct2.fld1 = #Struct1.x
    #Struct2.fld2 = #Struct1.y

s2 = MapS1ToS2.transform(s1)
s1 = MapS1ToS2.reverseTransform(s2)
```

```csharp
// Transform could support all kinds of hooks (overrides)
afterTransform: (self: MapS1ToS2)
    self.Target.fld3 = self.Source.z.Str()

// execution of these hooks would not be obvious!
```

Rule base mapping could use specific operators to indicate what rules to use for normal or reverse mapping.

```csharp
// Transform<Source, Destination>
MapStruct1Struct2: Transform<Struct1, Struct2>
    #Struct2.fld1 <= #Struct1.x     // src => dest
    #Struct2.fld2 <=> #Struct1.y    // src => dest and reverse
    #Struct2.fld3 => #Struct1.z     // dest => src
```

> TBD: More complex mappings:

- Splitting or joining fields?
- Conditional mapping/logic?
- External dependencies?
- Calling helpers for transformation?
- Nested objects/object trees?

```csharp
CustomFn1: (Struct1 self): U16
    ...
Deconstruct2:
    x: U16
    z: U16
CustomFn2: (Struct2: self): Deconstruct2
    ...
CustomFn3: (Struct1: self, x: U16, y: U16)
    ...

// Transform<Source, Destination>
MapStruct1Struct2: Transform<Struct1, Struct2>
    // CustomFn1 yields mapping result
    #Struct2.fld1 <= #Struct1.CustomFn1
    // CustomFn2's result is deconstructed onto target fields
    #Struct2.CustomFn2 => #Struct1.x, Struct1.z
    // CustomFn3 receives 2 parameters
    #Struct2.CustomFn3 <= #Struct1.x, Struct1.y
```

```csharp
MapStruct1Struct2: Transform<Struct1, Struct2>
    // match fields by name (2-way)
    #Struct1 <=> #Struct2
```

If no mapping rule structure is defined the field names are matched on a 1:1 basis in both directions using [Identifier](..\lexical\identifiers.md) name matching rules.

---

## Dependent Field Validation

(Dependent Types)

Add validation rules between fields. For example: `#fld1 > fld2`

When/how are these rules enforced?

```csharp
MyStruct
    fld1: U8
    fld2: Str
    // the length of the string must be less than value of fld1
    #fld2.Length < fld1
```

---

> TBD

- Allow YAML/JSON/XAML/XML to be used inline for declaring hierarchical data? Not sure how to separate the YAML/JSON/XAML/XML syntax from the Z# syntax.
