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

Enums can be exported from a module. In that case it is part of the API interface and all values must be assigned explicitly. You're less likely to break a public API when adding new enum options if its values are explicitly listed.

```csharp
#export MyEnum
    opt0 = 0
    opt1 = 1
    opt2        // error!
```

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

> How to detect `[Flags]` for .NET interop?

> Do we want additional validation on operators applied to enums and allow only Flags to use the bitwise operators?

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

Using the `Bit<T>` base type suggests that you are working with flags.

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

---

## Usage

When using an enum, the type name has to be specified too:

```C#
MyEnum
    opt1, opt2, opt3

e := MyEnum.opt1
```

The data type of an enum option is the enum type itself. So `e` in the example above is of type `MyEnum` with a value of `0` (zero).

When the enum type can be inferred from context it does not need repeating (optional).

```csharp
MyFunc: (p1: MyEnum)
    ...

// the parameter type dictates what enum to use
MyFunc(opt2)    // MyEnum.opt2
MyFunc(.opt2)   // MyEnum.opt2 (I like this best)
```

If `opt2` is ambiguous the type needs to be specified to resolve it.

---

> TBD: Allow `Str` enums to be used as indexers into a `Map`.

```csharp
Names: Str
    Field1
    Field2


map = (Field1 = "Field1", Field2 = "Field2")

v := map[Names.Field1]   // 'Field1'
// alternate map syntax?
v := map.Field1          // 'Field1'
v := map.$Field1         // 'Field1'
```

---

> Have validation functions on Enums to verify if values are in range. In .NET any (casted) integer is valid for an Enum.

> For .NET interop on enum base types that are not supported by .NET, a (record) class is generated with the options as static fields (and a private constructor).

```csharp
// C#
public  // if exported
sealed class MyEnum : IEquatable<MyEnum>, IComparable<MyEnum>
{
    private readonly string _value;
    private MyEnum(string value)
        => _value = value;

    public static MyEnum opt1 = new MyEnum("opt1");
    public static MyEnum opt2 = new MyEnum("opt2");

    public static implicit operator string(MyEnum myEnum)
        => myEnum._value;
    
    // IEquatable, IComparable implementation
}
```

---

### Value Iteration

How to iterate through the values of an enumeration:

```csharp
MyEnum
    opt1, opt2, opt3

// compiler attributes?
loop o in MyEnum#values
    ...

// first and last option value?
first := MyEnum#first
last := MyEnum#last
```

---

TBD

Combine enums with normal types and allow extra members per enum option. (SmartEnums/Java-enums).

Extend enum to be more like Rust enums?
https://doc.rust-lang.org/rust-by-example/custom_types/enum.html
They're more like union types.

---

Are enumerations nothing more than compile-time lists or dictionaries?

```csharp
// list-literal syntax with a compile-time token
myEnum := #(option1, option2, option3)
```

^^ Declaring a Type as if it was a var looks weird.

---

Enum options and flags:

```csharp
// single
myEnum
    option1 | option2 | option3

// flags (multiple)
myEnum
    flag1 & flag2 & flag3
```
