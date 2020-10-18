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

## Bit Fields

A structure can contain bit fields using the `Bit<T>` type.
All fields using the bit data type will be grouped together.

So the following example will only take up 1 byte:

```C#
MyStruct
    field1: Bit<2>      // bit 0-1
    field2: Bit<3>      // bit 2-4
    field3: Bit<3>      // bit 5-7
```

When the total number of bits exceed a byte the rest of the bits is packed into a new byte. Bit fields never cross byte boundaries.

When bit fields are combined with other types, the bit fields are grouped to optimize byte usage.

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

When a larger (than 8-bit-based) bit field type is needed the structure can be derived from an unsigned built-in data type. But the structure can no longer be mixed with non-bit field fields.

```C#
MyStruct: U16
    field1: Bit<2>      // bit 0-1
    field2: Bit<4>      // bit 2-5
    field3: Bit<4>      // bit 6-9
    field4: Bit<4>      // bit 10-13
    field5: Bit<4>      // error! cannot overflow
    other: U8           // error! only bit fields
```

## Nested Declaration

Structures can be declared in a nested fashion:

```csharp
Struct
    nested: NestedStruct
        fld1: U8
        fld2: Str
    name: Str
```

How ever the `NestedStruct` type is not available outside the structure it was declared in.

> TBD: Can the nested type be anonymous?

```csharp
Struct
    nested      // use {} here?
        fld1: U8
        fld2: Str
    name: Str
```

Nested inside a function?

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

## Memory Layout

The fields of a structure are layed out in the order of their definition starting at the base structure type. No alignment or filler bytes are added.

## Tables

> Find a way to allow to easily define tables of data using struct types in an array.

```csharp
MyStruct
    fld1: U8
    fld2: Str

arr: Array<MyStruct> = [
    { fld1 = 42, fld2 = "42" },     // by name
    { 101, "101" },                 // in field order
]
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

## Anonymous Structures

Also known as Tuples.

> Use `{}`

```csharp
a = { Fld1 = 42, Fld2 = "42" }

// a is a tuple with two fields
x = a.Fld1  // U8
y = a.Fld2  // Str

// deconstruct - creates new vars
(fld1, fld2) = a
// build new tuple from vars
b = { fld1, fld2 }
```

Preferred is to use field names for tuples, but even those can be omitted but then deconstruction has to be used to unpack.

```csharp
// no structure type, no field names
x = { 42, "42" }
// C# does this:
x.Item1 // Error: Item1 does not exist

// to use, deconstruct in order
(n, s) = x
// n = 42 (U8)
// s = "42" (Str)
```

> We want to line up the syntax (and semantics) with the parameters of a function call. Adopting a global rule that 'field-lists' (tuples, function params, deconstructs etc) can be build in-order or named or a combination (see function parameters). Array initialization too? `arr = (1, 2, 3, 4)`

> TBD: What is the syntax (type) for an anonymous structure?

```csharp
anoStructFn: (s)        // no type
anoStructFn: (s: Any)   // suggest passing a Str is valid
anoStructFn: (s: Dyn)   // indicates the runtime aspect of discovering fields
anoStructFn: (s: Struct)    // must be a struct
anoStructFn: (s: Record)    // other name for struct?

// this is most correct, albeit somewhat verbose
anoStructFn: (s: {U8, Str}) // tuple like
```

---

This is more a template thing...

```csharp
// template function
PropGetFn: <S>(self: S): Str
    return self.Name

// anonymous struct
s = { Name = "MyName" }
p = PropGetFn(s)
```

## Mapping

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

```csharp
// Transform would be a compiler supported type.
MapS1ToS2: Transform<Struct1, Struct2>
    #Struct2.fld1 = #Struct1.x
    #Struct2.fld2 = #Struct1.y

s2 = MapS1ToS2(s1)
s2 = s1.Transform()

// Transform could support all kinds of hooks (overrides)
afterTransform: (self: MapS1ToS2)
    self.Target.fld3 = self.Source.z.Str()
```
