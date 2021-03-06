# Type Conversion

In general converting to a type is done by using the target type name to convert to as a function name.

This is very similar to [Type Constructors](types.md$Type-Constructors), which are also functions with the same name as the Type to be created. Conversion functions are seen as a special type of constructor (or factory) function. One thing that is special about them is that the first parameter is a `self` parameter of the source Type to convert from.

```csharp
TargetType: (self: SourceType): TargetType
TargetType: (self: SourceType, bits: Range): TargetType
TargetType: (self: SourceType, other: X): TargetType
```

## Built-in Types

The allowed conversions are all explicitly represented by a function. The name of the function is the target type.

> There is no implicit conversion on assignment anywhere, ever.

```C#
b = 42        // U8
s = b.Text()  // to string "42"
```

Type conversion from larger to smaller types need some extra help:

```C#
v = 0x4242          // U16
b = v.U8()          // error: loss of data!
b = v.U8([8..16])   // using a Range to extract the bits
l = v.U32()         // l: U32
```

Using forward type inference.

```C#
v: U16 = 42         // v => U16
b: U8 = v           // error: loss of data!
l: U32 = v          // ok
```

Unchecked signed to unsigned or visa versa conversions boil down to the number of bits: can the target type contain all the bits of the original value - even though the meaning of those bits may change.

```C#
v = 0xFF        // U8: 255
i = v.I8()      // I8: -1
```

> Use checked functions to do bounds checking and make sure that the actual value has not changed meaning.

> Should we have specific conversion functions that explicitly state the conversion could be unsafe? `v.unsafeI8()`

## Optional

```csharp
v = 42
// can assign a value to an optional
o: Opt<U8> = v

// cannot assign an optional to a value
o: Opt<U8>
v = o       // error: o could be nothing

// without checking first
if o? => v = o
```

## Expressions

```csharp
// error: expression type > U8
v: U8 = 1000 / 200

// ok, converted
v: U8 = U8(1000 / 200)

// TBD: alternate syntax
v: U8 = (1000 / 200).U8()
```

----

## Try Convert

Return type is an `Opt<T>` of the target type.

```csharp
TryTargetType(self: SourceType, ...): TargetType?
```

## Force Convert

Force a conversion even if the meaning of the result changes.

```csharp
ForceTargetType(self: SourceType, ...): TargetType
DoTargetType(self: SourceType, ...): TargetType
```
