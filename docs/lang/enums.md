# Enums

Enums are groups of values that belong together.
An enum is basically a value type that has its possible values predefined.

> These values all have to be available at compile time, they cannot be calculated at runtime.

As with all type names, the name of the enum type has to start with a capital letter. The enum declaration looks very much like the declaration of a structure, but the use of the equals sign (and the absence of data types) make it an enum.

Here is an intuitive declaration of an enum type.

```C#
MyEnum
    opt1, opt2, opt3
```

> TBD: Comma Operator

Because the values were not specifically listed, they are assigned by the compiler and start at 0 (zero) in order of declaration.
So `opt1` would be 0, `opt2` would be 1 and `opt3` would be 2. You can assign these values specifically.

```C#
MyEnum
    opt1 = 0
    opt2 = 10
    opt3 = 20
```

The literal values can be specified using any valid format. See [Literals](../lexical/literals.md)

The type of the enum in the previous example is defaulted to `U8`. But you can be explicit about it and / or choose a different type.

Here is how that would look:

```C#
MyEnum: I8
    opt1 = -1      // can have negative values
    opt2 = 0
    opt3 = 1
```

Here is an example how to do flags:

```C#
MyBigEnum: U16
    flag = 0x8000
    mask = 0x00F0
```

> Flags are just enums with their value carefully controlled.

You can even specify a string:

```C#
StrEnum: Str
    Low = "Low_Option"
    Mid = "Mid_Option"
    High = "High_Option"
```

Each option has a string value. Also note that the option names may also begin with a capital letter.

Leave out the string literal values:

```C#
StrEnum: Str
    Low
    Mid
    High
```

And now `StrEnum.Low` is the value `"Low"`, the same as the name of the enum option.

Lets do bits:

```C#
MyEnum: Bit<4>      // 4-bits wide
    opt1 = 0x1
    opt2 = 0x2
    opt3 = 0x4
    opt4 = 0x8
    optx = 0x10    // error: too large!
```

Floating point types can also be used. The auto-numbering interval for floating points is `1.0`.

```C#
MyEnum: F16
    opt1, op2, opt3
```

`opt1` would be 0.0, `opt2` would be 1.0 and `opt3` would be 2.0.

Or if you set a value on one option:

```C#
MyEnum: F16
    opt1 = 3.14
    opt2
    opt3
```

`opt1` would be 3.14, `opt2` would be 4.14 and `opt3` would be 5.14.

There is one type that cannot be used:

```C#
MyEnum: Bool        // error!
```

## Usage

When using an enum, the type name has to be specified too:

```C#
MyEnum
    opt1, opt2, opt3

e = MyEnum.opt1
```

The data type of an enum option is the enum type itself. So `e` in the example above is of type `MyEnum` with a value of `0` (zero).

When the enum type can be inferred from context it does not need repeating (optional).

```csharp
MyFunc: (p1: MyEnum)
    ...

// the parameter type dictates what enum to use
MyFunc(opt2)    // MyEnum.opt2
MyFunc(.opt2)   // MyEnum.opt2
```
