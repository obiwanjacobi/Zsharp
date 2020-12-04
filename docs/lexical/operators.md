# Operators

Most operators are syntactic sugar over a set of wellknown functions.

Of these wellknown functions there are two flavors: unchecked and checked implementations. The unchecked flavor is used in the final program and its implementation is optimized to the fullest extend. The checked version implements extra validation and 'checking' to help make sure the code is correct.

> Operators never allocate memory!

## Operator Symbols

Arithmetic, bitwise and logical operators.

| Operator | Fn Name | Description
|--|--|--
| `+` | Add | Addition / Absolute? (unary)
| `-` | Subtract, Negate | Subtraction / Negation (unary)
| `*` | Multiply | Multiplication
| `/` | Divide | Division
| `%` | Remainder | Remainder
| `**` | Power | Power
| `( )` | - | Math Precedence
| `=` | IsEqual | Equals
| `<>` | IsNotEqual | Not Equals
| `>` | IsGreaterThan | Greater than
| `<` | IsLesserThan | Smaller than
| `>=` | IsGreaterEqual | Greater or Equal
| `<=` | IsLesserEqual | Smaller or Equal (`=<`?)
| `? :` | - | Ternary Conditional
| `and` | LogicAnd | Logical And
| `or` | LogicOr | Logical Or
| `not` | LogicNot | Logical Negation
| `&` | - | Bitwise And
| `|` | - | Bitwise Or
| `^` | - | Bitwise Exclusive Or
| `~` | - | Bitwise Negation (complement/invert)
| `>>` | - | Bitwise Shift Right
| `<<` | - | Bitwise Shift Left
| `>|` | - | Bitwise Rotate Right
| `|<` | - | Bitwise Rotate Left
| `=` | - | Value Assignment
| `:=` | - | Value Assignment with inferred Type

> Ternary operators cannot contain other ternary operators. No nesting of `? :` for readability.

## Other Symbols

| Symbol | Description
|---|---
| `_` | Unused / Discard
| `.` | Members Access
| `..` | Range operator
| `...` | Spread operator
| `,` | List Separator
| `:` | (Sub)Type Specifier
| `;` | Line separator
| `< >` | Type Parameter
| `( )` | Function / Array/List initialization
| `" "` | String
| `' '` | Character
| `@` | Disable String formatting features / Compiler Function?
| `{ }` | String formatting parameter / Code Decorator / Object construction
| `[ ]` | Index / Slice / Range / Capture
| `!` | Possible Error (return type)
| `?` | Optional variable or parameter/return value
| `??` | Optional variable fallback
| `??=` | Optional variable conditional assignment
| `#` | Pragma / Attribute Access
| `#!` | Compile-Time Code
| `=>` | Line continuation (instead of indent)

> TBD: `=>` should perhaps be `->` so we can use `=>` for some 'assignment' variant? Currently `=>` is used in Lambdas.

> Are there others like conditional assignment `??=`?

## Type operators

| Symbol | Type | Description
|---|---|---
| `!` | `Err<T>`  | Error return value or T
| `?` | `Opt<T>`  | Optional; T or Nothing
| `*` | `Ptr<T>`  | Pointer to T
| `^` | `Imm<T>`  | Immutable T

---

## Reserved Operator Symbols

| Operator | Description
|---|---
| `\` | reserved
| `$` | reserved
| `->` | Alternate function return type (confusing in combination with `=>`)
| `<=` | map structure (also arithmetic)
| `( )` | array/list initializer/literal?
| `|>` | 
| `<|` | 
| `:=` | reserve for variable assignment with type inference.
| `<=>` | Swap operator

---

## Short-hand Operators

| Operator | Description
|---|---
| `+=` | read (left) - add (right to left) - write (left)
| `-=` | read - subtract - write
| `*=` | read - multiply - write
| `/=` | read - divide - write
| `?=` | read - test - write (locking?)
| `!=` | read - ?? - write
| `%=` | read - ?? - write
| `&=` | read - ?? - write
| `$=` | read - ?? - write
| `^=` | read - ?? - write

> TBD: do we allow a list of right values?

```csharp
a = 42
a += (12, 23, 34)
// a = a + 12 + 23 + 34
```

---

## Data Type Wrapper Conversion Assignment Operators

Goal is to have a quick and easy way to convert from a normal data type `T` to one of the wrapper types (of T).

| Operator | Description
|---|---
| `=!` | `Err<T>` = `T`
| `=?` | `Opt<T>` = `T`
| `=*` | `Ptr<T>` = `T` (`Ptr()` conversion)
| `=^` | `Imm<T>` = `T`

```csharp
a: U8 = 42
err =! a    // err: Err<U8>
opt =? a    // opt: Opt<U8>
ptr =* a    // ptr: Ptr<U8>
imm =^ a    // imm: Imm<U8>
```

---

> What if operators cause overflow (or underflow)? A bitwise shift `<<` can shift out bits - sort of the point. Does every operator determine for itself if overflow is a problem or is there a general principle?

---

> TBD

- using two single quotes for a character `'x'` is nice and symmetrical but also redundant. Is there a shorter way to specify characters: `'x`? Only really need to address this if we want to use `'` for something else...

- an operator to test for 'nothing' (optional) or 'default'?
