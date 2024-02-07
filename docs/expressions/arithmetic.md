# Arithmetic Expressions

|Operator|Function
|--|--
| `+` | Addition
| `-` | Subtraction
| `*` | Multiplication
| `/` | Division*
| `%` | Remainder (integer)
| `**` | Power
| `-` | Negation

> *) Division for integer types are rounded towards zero.

> *) Cannot do square-root with `//` for it conflicts with comment syntax. Perhaps `/*`? Or change the comment syntax?

Some examples:

```C#
a := 12 * 3   // 36
b := a / -3   // -12
c := 3 ** 3   // 27
d := 16 % 3   // 1

x := (2 + a) * (b / 3) + d
```

> TBD: Do operands need to be (made) of the same type?

Can you add an `U8` and an `U16`?

I think we could make 'overloads' operator functions that take all possible operand data types and have an optimized implementation for each one.

---

## Overflow / Underflow

Checked functions can be used to implement the operators that check for overflow and underflow at runtime. Overflow is a condition where the results exceeds the storage capacity of the data types used. Underflow is a condition where a subtraction resulted in a numerical value smaller than 0 (on an unsigned type).

---

## Result Data Type Promotion

The arithmetic operation result may be too large to fit into the same data type as its operands.

```csharp
a := 42      // U8
b := 101     // U8
// what is the data type of c?
c := a * b
```

Each arithmetic operator has its own result type promotion.

|Operator|Result Type|Description
|--|--|--
| `+` | Type#size * 2  | U8=>U16, U16=>U32
| `-` | Type => signed | U8=>I8, U16=>I16, etc
| `*` | Type#size * 2  | U8=>U16, U16=>U32
| `/` | Type           | unchanged
| `%` | Type#size / 2  | U16=>U8, U32=>U16
| `**` | Type#size * 2 | U8=>U16, U16=>U32
| `** x` | Float?      | approx.
| `-` | Type => signed | U8=>I8, U16=>I16, etc

This all should play nice in [Custom Data Types](../lang/types.md#Custom-Data-Types) too, although they have their own operator implementations.

Note that numerical literals could also be thought of restricted custom data types (in a way). A constant value of 2 only has a small impact on the number of extra bytes needed for the result after an arithmetic operation.

Have templated overloads for the operator-functions that allow the code to select the result type.

```csharp
a := 42
b := 101

r := a.Add<U32>(b)   // r => U32
```
