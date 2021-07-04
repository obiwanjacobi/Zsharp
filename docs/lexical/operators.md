# Operators

Most operators are syntactic sugar over a set of wellknown functions.

Of these wellknown functions there are two flavors: unchecked and checked implementations. The unchecked flavor performs no validation. The checked version implements extra validation and 'checking' to help make sure the code is correct.

> Use identifier prefixes to identify checked/unchecked operator functions implementations. (`checked_` / `unchecked_`?)

Note that `checked` and `unchecked` do not refer to the .NET variants. It only means that any conversion the operator does is checked to be correct (or not checked).

---

## Operator Symbols

Arithmetic, bitwise and logical operators.

| Operator | Fn Name | Description
|--|--|--
| `+` | ArithmeticAdd | Addition / Absolute? (unary)
| `-` | ArithmeticSubtract, ArithmeticNegate | Subtraction / Negation (unary)
| `*` | ArithmeticMultiply | Multiplication
| `/` | ArithmeticDivide | Division
| `%` | ArithmeticRemainder | Remainder
| `**` | ArithmeticPower | Power
| `( )` | - | Math Precedence, Function Call, List Literal, Tuple/deconstruct
| `=` | IsEqual | Equals
| `<>` | IsNotEqual | Not Equals
| `>` | IsGreaterThan | Greater than
| `<` | IsLesserThan | Smaller than
| `>=` | IsGreaterEqual | Greater or Equal
| `=<` | IsLesserEqual | Smaller or Equal
| `? :` | - | Ternary Conditional (if-else)
| `and` | LogicAnd | Logical And
| `or` | LogicOr | Logical Or
| `xor` | LogicXor | Logical Xor
| `not` | LogicNot | Logical Negation
| `&` | - | Bitwise And
| `|` | - | Bitwise Or
| `^` | - | Bitwise Exclusive Or
| `~` | - | Bitwise Negation (complement/invert)
| `>>` | - | Bitwise Shift Right
| `<<` | - | Bitwise Shift Left
| `>|` | - | Bitwise Rotate Right
| `|<` | - | Bitwise Rotate Left
| `->>` | - | sign extend (arithmetic) bit shift right
| `=` | - | Value Assignment
| `:=` | - | Value Assignment with inferred Type

> Ternary operators cannot contain other ternary operators. No nesting of `? :` for readability.

Allow logical `not` to be prefixed to other logical operators? `nand`, `nor`, `nxor`?

Operator that cascades the left value?
So instead of `if c = 42 or c = 101` you can write something like `if c = 42 || 101`. See also [match expression](../lang/match.md).

| Operator | Fn Name | Description
|--|--|--
| `&&` | cascading l-value logical-and
| `||` | cascading l-value logical-or

---

## Other Symbols

| Symbol | Description
|---|---
| `_` | Unused / Discard / Hidden
| `.` | Members Access
| `..` | Range operator
| `...` | Spread operator
| `,` | List Separator
| `:` | (Sub)Type Specifier
| `;` | Line break/separator
| `< >` | Type Parameter
| `( )` | Function / Tuple / Array/List initialization
| `" "` | String Literal
| `' '` | Character Literal
| `@` | Disable String formatting features / keyword escape
| `{ }` | String formatting parameter / Code Decorator / Object construction
| `[ ]` | Index / Slice / Range / Capture
| `!` | Possible Error (return type)
| `?` | Optional variable or parameter/return value (boolean operator)
| `??` | Optional variable fallback
| `??=` | Optional variable conditional assignment
| `#` | Pragma / Attribute access / Execute at compile-time
| `#!` | Compile-time code definition (perhaps only `#`)
| `->` | Line continuation (instead of indent)
| `##` | Temporary comment (compiler warning)

> Are there others like conditional assignment `??=`? Can any (applicable) operator be made conditional by prefixing `??` to it?

`#!` does make the distinction clear between compile-time functions and for instance inline exported functions: `#export fun: ()...` vs. `#! fun: () ...`.

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
| `|` | reserved
| `$` | reserved
| `=>` | used in mapping / some sort of (forward) assignment?
| `<=` | map structure / assign struct properties
| `()` | Function Object operator
| `|>` | Parameter pipe?
| `<|` | Reverse parameter pipe?
| `<=>` | Swap operator
| `::` | reserved
| `<-` | reserved
| `[[ ]]` | Alternate Decorators syntax (instead of `{}`)

---

## String Operator Symbols

Operators for strings and characters.

| Operator | Description
|---|---
| `''` | Delimiters for a symbol name with special characters (surround with double single-quotes).
| `=~` | Case (and culture) insensitive equals.
| `<>~` | Case (and culture) insensitive not-equals.
| `>~` | Case (and culture) insensitive greater-than - sorting.
| `<~` | Case (and culture) insensitive lesser-than  - sorting.
| `>=~` | Case (and culture) insensitive greater-than-or-equal - sorting.
| `=<~` | Case (and culture) insensitive lesser-than-or-equal - sorting.
| `TBD` | Concat a string.

```csharp
// string concat operator?
s = "Hello " <| "World"
s = "Hello " <+ "World"
s = "Hello " <& "World"
s = "Hello " + "World"
s = "Hello " & "World"
s = "Hello " << "World"
```

---

## Short-hand Operators

All these operators work as follows:
_Read left, [op] right to left, write left_

These operators cannot be overloaded, they simply use the standard operators.

| Operator | Description
|---|---
| `+=` | read (left) - add (right to left) - write (left)
| `-=` | read - subtract - write
| `*=` | read - multiply - write
| `/=` | read - divide - write
| `%=` | read - modulo/remainder - write
| `**=` | read - power - write
| `>>=` | read - shift right - write
| `<<=` | read - shift left - write
| `>|=` | read - roll right - write
| `|<=` | read - roll left - write
| `?=` | read - test - write (locking?)
| `!=` | read - ?? - write
| `&=` | read - bit and - write
| `|=` | read - bit or - write
| `^=` | read - bit xor - write
| `$=` | read - ?? - write
| `^=` | read - 'immutable' ?? - write
| `|>=` | ?
| `<|=` | ? (or `=<|`)

Do we allow a list of right values? (yes)

```csharp
a = 42
a += (12, 23, 34)
// a = a + 12 + 23 + 34

a += (x, y, z)
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

// as parameters inline
fnErr(!a)
fnOpt(?a)
fnPtr(*a)
fnImm(^a)

// or named parameters
fnErr(err =! a)
fnOpt(opt =? a)
fnPtr(ptr =* a)
fnImm(imm =^ a)
```

> Or are these conversions implicit?

---

> What if operators cause overflow (or underflow)? A bitwise shift `<<` can shift out bits - sort of the point. Does every operator determine for itself if overflow is a problem or is there a general principle?

> What syntax to specifically use/call checked or unchecked operator implementations? How to ignore overflow?

```csharp
a = 42
// checked on U16 target by default
x: U16 = a ** a

// use explicit conversion overload?
x: U16 = U16(a ** a, .Unchecked)

// use explicit conversion function?
x: U16 = U16unchecked(a ** a)

// unchecked operator?
x: U16 = $(a ** a)
```

---

> TBD

- an operator to test for 'nothing' (optional) or 'default'?

- allow custom defined operators? `.>>.`, `|<<` etc. Requires identifiers to be less strict. Also requires escape characters in function definition symbol: `''.>>.'': (...): Bool`
