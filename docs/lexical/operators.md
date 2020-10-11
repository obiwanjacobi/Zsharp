# Operators

Most operators are syntactic sugar over a set of wellknown functions.

Of these wellknown functions there are two flavors: unchecked and checked implementations. The unchecked flavor is used in the final program and its implementation is optimized to the fullest extend. The checked version implements extra validation and 'checking' to help make sure the code is correct.

> Operators never allocate memory!

## Operator Symbols

Arithmetic, bitwise and logical operators.

| Operator | Description
|---|---
| `+` | Addition
| `-` | Subtraction / Negation
| `*` | Multiplication
| `/` | Division
| `%` | Remainder
| `**` | Power (`**2` ? => `**3`, `**4` etc?)
| `( )` | Math Precedence
| `=` | Equals
| `<>` | Not Equals
| `>` | Greater than
| `<` | Smaller than
| `>=` | Greater or Equal
| `<=` | Smaller or Equal (`=<`?)
| `? :` | Ternary Conditional
| `and` | Logical And
| `or` | Logical Or
| `not` | Logical Negation
| `&` | Bitwise And
| `|` | Bitwise Or
| `^` | Bitwise Exclusive Or
| `~` | Bitwise Negation (invert)
| `>>` | Bitwise Shift Right
| `<<` | Bitwise Shift Left
| `>|` | Bitwise Rotate Right
| `|<` | Bitwise Rotate Left
| `=` | Value Assignment

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
| `@` | Disable String formatting features
| `{ }` | String formatting parameter / Code Decorator / Object construction
| `[ ]` | Index / Slice / Range / Capture
| `!` | Possible Error (on return type)
| `?` | Optional variable or parameter/return value
| `??` | Optional variable assignment fallback
| `??=` | Optional variable conditional assignment
| `#` | Pragma / Attribute Access
| `#!` | Compile-Time Code
| `=>` | Line continuation (instead of indent)

> TBD: `=>` should perhaps be `->` so we can use `=>` for some 'assignment' variant?

> Are there others like conditional assignment `??=`?

## Type operators

| Symbol | Type | Description
|---|---|---
| `!` | Err\<T> (post) | Error return value or T
| `?` | Opt\<T> (post) | Optional; T or Nothing
| `*` | Ptr\<T> (pre) | Pointer to T
| `^` | Imm\<T> (pre) | Immutable T

---

## Reserved Operator Symbols

| Operator | Description
|---|---
| `\` | reserved
| `$` | reserved
| `->` | Alternate function return type (confusing in combination with `=>`)
| `<=` | map structure (also arithmetic)
| `( )` | array/list initializer?
| `|>` | 
| `<|` | 
| `:=` | reserve for variable assignment with type inference.
| `<=>` | Swap operator (`><`?)

---

Not sure about these:

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

---

> What if operators cause overflow (or underflow)? A bitwise shift `<<` can shift out bits - sort of the point. Does every operator determine for itself if overflow is a problem or is there a general principle?

---

> TBD

- using two single quotes for a character `'x'` is nice and symmetrical but also redundant. Is there a shorter way to specify characters: `'x`? Only really need to address this if we want to use `'` for something else...

- an operator to test for 'nothing' (optional) or 'default'?
