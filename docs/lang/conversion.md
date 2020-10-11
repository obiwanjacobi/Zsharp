# Type Conversion

## Built-in Types

The allowed conversions are all explicitly represented by a function. The name of the function is the target type. There is no implicit conversion on assignment or anywhere, ever.

```C#
b = 42        // U8
s = b.Text(alloc)   // to string "42"
```

Because `Text` allocates memory to store its content an Allocator object is required for this conversion.

Type conversion from larger to smaller types need some extra help:

```C#
v = 0x4242          // U16
b = v.U8()          // error: loss of data!
b = v.U8([8..16])   // using a Range to extract the bits
```

Alternative Syntax?

```C#
v = 42: U16         // v => U16
b = v: U8           // error: loss of data!
// Nah:
b = v: U8([8..16])  // using a Range to extract the bits
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
if o => v = 0
```
