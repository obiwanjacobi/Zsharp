# Operators

Most operators are syntactic sugar over a set of wellknown functions.

Of these wellknown functions there are two flavors: unchecked and checked implementations. The unchecked flavor performs no validation. The checked version implements extra validation and 'checking' to help make sure the code is correct.

> Use identifier prefixes to identify checked/unchecked operator functions implementations. (`checked_` / `unchecked_`?)

Note that `checked` and `unchecked` do not refer to the .NET variants. It only means that any conversion the operator does is checked to be correct (or not checked).

> TBD

Add specific operators for arithmetic operations for saturation (ignoring overflow and filling up till full) and wrapping (wrapping around to start over again when full) behavior.

Postfix arithmetic operator with:

| Operator | Description
|--|--
| `!` | Overflow/Underflow will cause an exception. (`.NET checked`)
| `|` | Overflow/underflow will wrap around. (`.NET unchecked`)
| `~` | Overflow/underflow will saturate.

(Zig has explicit operators too)

```csharp
a: U8 = 245
b: U8 = 125

c: U8 = a +! b  // overflow exception (370 > U8)
c: U8 = a +| b  // wrap around (115)
c: U8 = a +~ b  // saturate (255)
```

> Is checked `!` the default?

---

## Precedence

In order of precedence (top is highest):

| Operator | Description
|--|--
| Unary | Unary operators are always applied first
| Binary | infix operators typically require `()`
| Ternary | ternary operators are applied last.

That means that an expression with multiple (binary) expressions **MUST** use `( )`
to indicate the order of execution - unless the operators are all the same.

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
| `**` | ArithmeticPower | Power (3 ** 2 = 9)
| `//` | ArithmeticRoot | Root/Log (9 // 2 = 3)
| `%%` | ArithmeticModulo | Modulo (negative numbers)
| `( )` | - | Infix Operator Precedence, Function Call, List Literal, Tuple/deconstruct
| `=` | IsEqual | Equals (value equality, `is` for identity?)
| `<>` | IsNotEqual | Not Equals (value inequality, `is not` for identity?)
| `>` | IsGreaterThan | Greater than
| `<` | IsLesserThan | Smaller than
| `>=` | IsGreaterEqual | Greater or Equal
| `=<` | IsLesserEqual | Smaller or Equal
| `?` | AsBoolean | Boolean postfix
| `? :` | - | Ternary Conditional (if-else) (don't like the `:` used for types everywhere else)
| `? ;` | - | -alt- Ternary Conditional (if-else)
| `and` | LogicAnd | Logical And
| `or` | LogicOr | Logical Or
| `xor` | LogicXor | Logical Xor
| `not` | LogicNot | Logical Negation
| `&` | - | Bitwise And*
| `|` | - | Bitwise Or*
| `^` | - | Bitwise Exclusive Or*
| `~` | - | Bitwise Negation (complement/invert)*
| `>>` | - | Bitwise Shift Right
| `<<` | - | Bitwise Shift Left
| `>|` | - | Bitwise Rotate Right
| `|<` | - | Bitwise Rotate Left
| `->>` | - | sign extend (arithmetic) bit shift right
| `>>>` | - | -alt- sign extend (arithmetic) bit shift right
| `=` | - | Value Assignment
| `:=` | - | Value Assignment with inferred Type

> TBD: *) we could reuse the logical operators for use as bitwise operators as well.
That also means that the bitwise short-hand operators (`&=`, `|=` or `^=`) no longer work.

> Ternary operators cannot contain other ternary operators. No nesting of `? :` for readability.

Allow logical `not` to be prefixed to other logical operators? `nand`, `nor`, `nxor`?

More mathematic concepts as operators?
PI (and other constants), rad, deg, vectors, matrix, infinity, sin, cos, tan (inv), rounding (floor, ceiling), medium, mean, average, factorial/permutation/combination, sum...

Operator that cascades the left value? See C# pattern matching with `is`.
So instead of `if c = 42 or c = 101` you can write something like `if c = 42 || 101`. See also [match expression](../expressions/match.md). Would also work with `if c in (42, 101)`.

| Operator | Fn Name | Description
|--|--|--
| `&&` | cascading l-value logical-and
| `||` | cascading l-value logical-or

---

## Other Symbols

| Symbol | Description
|---|---
| `_` | Unused / Discard / Hidden
| `.` | Members Access / bound access
| `..` | Range operator
| `...` | Spread operator
| `,` | List Separator
| `:` | (Sub)Type Specifier
| `;` | reserved
| `< >` | Type Parameter
| `( )` | Function / Tuple / Array/List initialization
| `" "` | String Literal
| `' '` | Character Literal
| `'' ''` | Special Name
| `@` | Disable String formatting features / keyword escape?
| `{ }` | String formatting parameter / Code Decorator / Object construction
| `[ ]` | Index / Slice / Range / Capture
| `!` | Possible Error (return type)
| `?` | Optional variable or parameter/return value / boolean operator / fallback
| `?=` | Optional variable conditional assignment
| `->` | Line continuation (instead of indent)
| `#` | Pragma / Attribute access / Execute at compile-time
| `#!` | Compile-time code definition (perhaps only `#`)
| `#!` | Compile-time error (alt)
| `##` | Temporary comment (compiler warning)
| `#_` | Comment

`#!` does make the distinction clear between compile-time functions and for instance inline exported functions: `#export fun: ()...` vs. `#! fun: () ...`.

> Are there others like conditional assignment `?=`? Can any (applicable) operator be made conditional by prefixing `?` to it?

| Symbol | Description
|---|---
| `?=` | conditional assignment
| `?+` | conditional add
| `?`-arithmetic | conditional any arithmetic operation
| `?+=` | conditional read-add-write

```csharp
a: U8 = 42
o: U8? = _

// on which side is the optional?
x = a ?+ o
// does it matter?
x = o ?+ a
```

> TBD: chaining bools with `?` => No, use `or`.

```csharp
tryFn(p: U8): Bool
    ...

b = tryFn(42) or tryFn(101)

// self-bound example
tryFn(self: Str, p: U8): Bool
    ...

s = "42"
// use auto-fluent syntax?
b = s.tryFn(42) or/and 
    .tryFn(101)

// pattern matching ??
b = s is tryFn(42) or tryFn(101)    // weird
```

---

## Type operators

| Symbol | Type | Description
|---|---|---
| `!` | `Err<T>`  | Error return value or T
| `?` | `Opt<T>`  | Optional; T or Nothing
| `*` | `Ptr<T>`  | Pointer to T
| `^` | `Imm<T>`  | Immutable T
| `%` | `Atom<T>`  | Atomic T

---

## Reserved Operator Symbols

All non-characters and numbers are reserved at this point.

To be determined:

| Operator | Description
|---|---
| `\` | reserved
| `$` | to string / auto-constant string checked by compiler.
| `!` | reserved (factorial?)
| `?.` | Safe Navigation
| `=>` | used in mapping / some sort of (forward) assignment? (implies?)
| `<=` | map structure / assign struct properties
| `()` | Function Object operator
| `|>` | Parameter pipe?
| `<|` | Reverse parameter pipe?
| `<=>` | Swap operator
| `::` | traits? (type of type)
| `:=` | equals type (bool/condition) (also assignment with type inference)
| `:?` | type is (C# is keyword)
| `<:?` | type as (optional cast)
| `<:` | down cast type
| `:>` | up cast type? (is implicit)
| `<-` | reserved
| `->>` | parallel execution (also sign extended shift)
| `=>>` | parallel execution and collect results (in tuple or deconstruct)
| `[[ ]]` | Alternate Decorators syntax (instead of `{}`)

---

## String Operator Symbols

Operators for strings and characters.

| Operator | Description
|---|---
| `'' ''` | Delimiters for a symbol name with special characters.
| `=~` | Case (and culture) insensitive equals.
| `<>~` | Case (and culture) insensitive not-equals.
| `>~` | Case (and culture) insensitive greater-than - sorting.
| `<~` | Case (and culture) insensitive lesser-than  - sorting.
| `>=~` | Case (and culture) insensitive greater-than-or-equal - sorting.
| `=<~` | Case (and culture) insensitive lesser-than-or-equal - sorting.
| `s[2..6]` | sub-string using `Range`.
| `<+` | Concat a string.
| `+` | alt - Concat a string?
| `/<+` | Concat a string with path separator.
| `x<+` | Concat a string with any (x) separator?

## Comparison Operator with Margin Symbols

Operators for comparing numbers with a margin.
Typically useful for floating point numbers.

| Operator | Description
|---|---
| `=  ~` | Equals with a margin.
| `<>  ~` | Not equals with a margin.
| `>  ~` | Greater than with a margin.
| `<  ~` | Lesser than with a margin.
| `>=  ~` | Greater-than-or-equal.
| `=<  ~` | Lesser-than-or-equal.

These operators require 3 operands.

```csharp
f := 3.14
pi := 3.1415

// how to specify three operands?
if f = pi ~ 0.01
    // same (within margin)
```

---

## Array and List operators

Operators for working with `Array<T>` and `List<T>` types.

| Operator | Description
|---|---
| `+=` | add item to array/list
| `<+=` | alt - add item to array/list
| `-=` | remove item from array/list
| `<-=` | alt - remove item from array/list
| `^=` | insert item into array/list (front)
| `&=` | add item to array/list/tree as a child
| `|=` | insert item to array/list/tree as a child (front)
| `in` | test if item is in array/list (contains)
| `not in` | test if item is not in array/list (not contains)
| `/` | split in array with chunks/tuples of n
| `|` | zip two arrays
| `~` | unzip (split in 2)
| `<+` | Concat an array to another.
| `+` | alt - concat array?

```csharp
arr = (1, 2, 3, 4, 5)
// add single item
arr += 6
// remove multiple items
arr -= (1, 3, 5)    // arr = (2, 4, 6)
b = 4 in arr        // true
```

Using arithmetic operators for array item manipulation hinders future support of array programming where the operators are applied to the items within the array, not the array itself.
So perhaps it would be more clear to reserve the common arithmetic operators for array programming and special adorned operators for array content manipulation.

```csharp
arr1 = (1, 2, 3, 4, 5)
arr2 = (5, 4, 3, 2, 1)

arr3 = arr1 + arr2
// arr3 = (6, 6, 6, 6, 6)

arr1 <+ 42      // arr1 = (1, 2, 3, 4, 5, 42)
arr2 <^ 101     // arr2 = (101, 5, 4, 3, 2, 1)

arr4 = arr1 + arr2
// arr4 = (102, 7, 7, 7, 7, 43)
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
| `!=` | read - error?? - write
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
Note the type-operator is on the right side of the equals sign.

| Operator | Description
|---|---
| `=!` | `Err<T>` = `T`
| `=?` | `Opt<T>` = `T`
| `=*` | `Ptr<T>` = `T` (`Ptr()` conversion)
| `=^` | `Imm<T>` = `T`
| `=%` | `Atom<T>` = `T`

```csharp
a: U8 = 42
err =! a    // err: Err<U8>
opt =? a    // opt: Opt<U8>
ptr =* a    // ptr: Ptr<U8>
imm =^ a    // imm: Imm<U8>
atom =% a    // atom: Atom<U8>

// as parameters inline
fnErr(!a)
fnOpt(?a)
fnPtr(*a)
fnImm(^a)
fnAtom(%a)

// or named parameters
fnErr(err =! a)
fnOpt(opt =? a)
fnPtr(ptr =* a)
fnImm(imm =^ a)
fnAtom(imm =% a)
```

> Or are these conversions implicit?

---

> What if operators cause overflow (or underflow)? A bitwise shift `<<` can shift out bits - sort of the point. Does every operator determine for itself if overflow is a problem or is there a general principle?

> What syntax to specifically use/call checked or unchecked operator implementations? How to ignore overflow?

See `TBD` note at the top about checked, wrap around and saturate operators.

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
x: U16 = !(a ** a)
```

---

> TBD

- allow custom defined operators? `.>>.`, `|<<` etc. Requires identifiers to be less strict. Also requires escape characters in function definition symbol: `''.>>.'': (...): Bool`. The names would be considered normal functions and therefor -to be used as operators- would need to adhere to the infix function rules.

Then there is also a `.NET` interop problem. How are these operators exposed to (for instance) a C# program?
We could spell out each character to make a unique name that is still callable from other .NET languages.

- '`.>>.`' => `op_dotgtgtdot`
- '`|<<`' => `op_pipeltlt`

---

> TBD: Is operator overloading useful for operators other than arithmetic, comparable and possibly logical?

Which operators will definitely not be overloadable?

---

> TBD: Allow 'array programming' operator (overloads) that target simd instructions?

---

> TBD

Implement operators by tagging regular functions with the operator signs.
This seems to be the best way to also interop with normal .NET / C#.

```csharp
// must follow the infix function rules.
[[Operator(">>|")]]
MyWeirdOperator: <T>(self: T, other: T): T
    ...

a = 42
x = a >>| 101   // calls MyWeirdOperator

// How to do unary or ternary operators?
[[Operator("-")]]
MyUnaryOperator: <T>(other: T): T

[[Operator(">>|", "|<<")]]
MyTernaryOperator: <T>(self: T, other: T, third: T): T
```
