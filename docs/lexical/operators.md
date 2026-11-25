# Operators

> See the [Symbol Charter](symbols.md) for the core concept behind each symbol and the open decisions on symbol usage.

Most operators are syntactic sugar over a set of wellknown functions.

Of these wellknown functions there are two flavors: unchecked and checked implementations. The unchecked flavor performs no validation. The checked version implements extra validation and 'checking' to help make sure the code is correct.

> Use identifier prefixes to identify checked/unchecked operator functions implementations. (`checked_` / `unchecked_`?)
Or use a namespace for interop? `Checked.ArithmeticAdd()` The namespace can either be a dotnet namespace or static (nested) class.

> TBD

We need to define context categories where the same operators can have a different meaning.

An example of this would be operators in an arithmetic expression do different things than the same operator symbols in say a string-context.

| Context Category | Description |
| -- | -- |
| Arithmentic Expression | numeric operands. |
| Comparison Expression | Also margin-ternary operators. |
| Logical Expression | Boolean logic |
| Value Expression | Literals and string manipulation. |
| Object Expression | Object creation and mapping |
| Type Expression | Type info, coercian and declaration |
| Array Expression | Array manipulation and arithmetic |

---

## Precedence

In order of precedence (top is highest):

| Operator | Description |
| -- | -- |
| Unary | Unary operators are always applied first |
| Binary | infix operators typically require `()` |
| Ternary | ternary operators are applied last. |

That means that an expression with multiple (binary) expressions **MUST** use `( )`
to indicate the order of execution - unless all the operators are the same (left to right).

> We may need to define precedence between the different types of operators as well.
So arithmetic operators should run before comparison operators for instance.
It would probably look a bit silly if you had to put parenthises around `a < x + y`.

| Operator | Description |
| -- | -- |
| Arithmetic, Bitwise | Use parenthises to determine order. |
| Comparison | Compare with 'values' complete (previous). |
| Logical | Combine mulitple boolean (comparison) expressions last. |

---

## Operator Symbols

Arithmetic, comparison, bitwise and logical operators.

| Operator | Fn Name | Description |
| -- | -- | -- |
| `+` | ArithmeticAdd | Addition / Absolute? (unary) |
| `-` | ArithmeticSubtract, ArithmeticNegate | Subtraction / Negation (unary) |
| `*` | ArithmeticMultiply | Multiplication |
| `/` | ArithmeticDivide | Division |
| `%` | ArithmeticRemainder | Remainder |
| `/%` | ArithmeticDivideRemainder | Divide and remainder (tuple) |
| `**` | ArithmeticPower | Power (3 ** 2 = 9) |
| `//` | ArithmeticRoot | Root/Log (9 // 2 = 3) |
| `%%` | ArithmeticModulo | Modulo (negative numbers) |
| `( )` | - | Infix Operator Precedence, Function Call, List Literal, Tuple/deconstruct |
| `=` | IsEqual | Equals (value equality, `is` for identity?) |
| `<>` | IsNotEqual | Not Equals (value inequality, `is not` for identity?) |
| `>` | IsGreaterThan | Greater than |
| `<` | IsLesserThan | Smaller than |
| `>=` | IsGreaterEqual | Greater or Equal |
| `=<` | IsLesserEqual | Smaller or Equal |
| `?` | AsBoolean | Boolean postfix |
| `? ;` | - | Ternary Conditional (if-else) |
| `and` | LogicAnd | Logical And |
| `nand` | LogicAnd | Logical Nand? |
| `or` | LogicOr | Logical Or |
| `nor` | LogicOr | Logical Nor? |
| `xor` | LogicXor | Logical Xor |
| `nxor` | LogicNxor | Logical Nxor? |
| `not` | LogicNot | Logical Negation |
| `&` | - | Bitwise And* |
| `\|` | - | Bitwise Or* |
| `^` | - | Bitwise Exclusive Or* |
| `~` | - | Bitwise Negation (complement/invert)* |
| `>>` | - | Bitwise Shift Right |
| `<<` | - | Bitwise Shift Left |
| `>\|` | - | Bitwise Rotate Right |
| `\|<` | - | Bitwise Rotate Left |
| `>>>` | - | sign extend (arithmetic) bit shift right |
| `=` | - | Value Assignment, both mutable and immutable |
| `:=` | - | Value Assignment with inferred Type |

> TBD: *) we could reuse the logical operators for use as bitwise operators as well.
That also means that the bitwise short-hand operators (`&=`, `|=` or `^=`) no longer work.
>
> Decided: `&`, `|` and `^` stay as bitwise operators (binary context; attached `^` means mutable).
> `~` stays as bitwise invert for now, but it does not fit the `~` concept (approximately / cyclic). Alternative: `not` on integers.

Ternary operators can contain other ternary operators. Nested ternaries each have their own indent level.

```csharp
x := a > b
    ? a
    : b > c
        ? b
        : c
```

No other formatting is supported for nested ternaries.

> TBD: Can ternary be an l-value - that can be assigned to?

```csharp
x := 0
y := 1

// result is (a ref to) the var not the value of the var.
(x > y) ? x ; y = 42

// TBD: the ';' makes it read like 2 statements.
```

Allow logical `not` to be prefixed to other logical operators? `nand`, `nor`, `xnor` or use dedicated keywords?

> More mathematic concepts as operators?
PI (and other constants), rad, deg, vectors, matrix, infinity, sin, cos, tan (inv), rounding (floor, ceiling), medium, mean, average, factorial/permutation/combination, sum...

Operator that cascades the left value? See C# pattern matching with `is`: `if c is 42 or 101`.
So instead of `if c = 42 or c = 101` you can write something like `if c = 42 || 101`. See also [match expression](../expressions/match.md). Would also work with `if c in (42, 101)`.

| Operator | Fn Name | Description |
| -- | -- | -- |
| `&&` | cascading l-value logical-and | |
| `\|\|` | cascading l-value logical-or | |

> TBD: The `is` expression is probably easier to read and understand. No extra operators needed.

---

## Other Symbols

| Symbol | Description |
| --- | --- |
| `_` | Unused / Discard / Hidden / symbol separator (ignored) / digit separator (ignored) |
| `.` | Members Access / bound access |
| `..` | Range operator |
| `...` | Spread operator |
| `,` | List Separator |
| `:` | (Sub)Type Specifier |
| `;` | Ternary else branch |
| `` ` `` | Character escape |
| `< >` | Type Parameter |
| `( )` | Function / Array/List initialization |
| `" "` | String Literal |
| `' '` | Character Literal |
| `'' ''` | Special Name / keyword escape |
| `@` | Compiler extensions |
| `{ }` | (anonymous) Object construction |
| `[ ]` | Index / Slice / Range |
| `~` | Creates a Range from 2 operands. (r = x ~ y => [x-y, x+y]) |
| `\| \|` | Capture |
| `!` | Possible Error (on return type) (`Err<T>`) |
| `!!` | Possible Exception (on return type) TBD |
| `?` | Optional variable or parameter/return value / boolean operator / fallback |
| `?=` | Optional variable conditional assignment |
| `->` | Match case continuation / Line continuation (instead of indent) / fun decl return type (instead of `:`)? |
| `#` | (Type) Attribute access / Execute at compile-time |
| `#` | Compile-time code definition |
| `#!` | Compile-time error |
| `#?` | Compile-time warning |
| `#*` | Compile-time info? |
| `##` | Temporary comment (compiler 'warning') |
| `#xxx:` | Compile-time label 'xxx' used to identity code blocks / scopes |
| `#_` | Comment |

In case of a fallback `?` we could use `or` perhaps?
`opt? or 42` Use the `?` as the boolean operator and `or` for chaining.
`opt1? or opt2? or 42`

> Are there others like conditional assignment `?=`? Can any (applicable) operator be made conditional by prefixing `?` to it?

| Symbol | Description |
| --- | --- |
| `?=` | conditional assignment |
| `?+` | conditional add |
| `?`-arithmetic | conditional any arithmetic operation |
| `?+=` | conditional read-add-write |

```csharp
a: U8 = 42
o: U8? = _

// on which side is the optional?
x := a ?+ o
// the optional is on the side of the '?'
x := o ?+ a
```

> TBD: chaining bools with `?` => No, use `or`.

```csharp
tryFn(p: U8): Bool
    ...

b = tryFn(42) or tryFn(101)

// self-bound example
tryFn(self: Str, p: U8): Bool
    ...

s := "42"
// use auto-fluent syntax?
b := s.tryFn(42) or/and 
    .tryFn(101)

// pattern matching ??
b := s is tryFn(42) or tryFn(101)    // weird
```

---

## Reserved Operator Symbols

All non-characters and numbers are reserved at this point.

To be determined:

| Operator | Description |
| --- | --- |
| `\` | reserved |
| `$` | to-string / auto-constant string checked by compiler / String formatting parameter |
| `!` | reserved (factorial?) |
| `?.` | Safe Navigation |
| `=>` | used in mapping / some sort of (forward) assignment? (implies?) |
| `<=` | map structure / assign struct properties / copy instance |
| `()` | Function Object operator |
| `<=>` | Swap operator? / bi-directional mapping operator |
| `::` | traits? (type of type) |
| `->>` | parallel execution |
| `=>>` | parallel execution and collect results (in tuple or deconstruct) |
| `[[ ]]` | Decorators syntax |
| `[< >]` | .NET code attribute (the decorator is a type) |
| `[( )]` | Function decorator (the decorator is a function: code injection) |

> Notes on safe navigation `?.`:

In an expression that chains several safe navigations together in a path, the result may become a little hard to read.

```csharp
x := root?.obj1?.obj2?.prop1
```

Perhaps we could come up with something that turns on safe navigation for the entire expression?

```csharp
x := ?root.obj1.obj2.prop1
```

Is there use for safe navigation in collections?

```csharp
// safe indexing?
x := root.collection[?0]    // x: Opt<T>
```

Use of safe navigation (in any form) always results in an Optional `Opt<T>` that is nothing if the path could not be navigated completely.

Expression Type of safe navigation - specifying a default value:

TBD: syntax of specifying a default value.

```csharp
x: Opt<SomeType> = ...
// will the condition expression type be bool?
if x?.IsTrue
    ...
// we don't wanna write (works, but suboptimal)
if x?.IsTrue = true
    ...
// or (this is what we have now)
if x?.IsTrue ?? true
    ...

// allow a default value to be specified when it fails?
if x?false?.IsTrue
    ...

// v will be Opt<SomeIntegerType>
v := x?.Count
// v will be SomeIntegerType (based on Count)
v := x?0?.Count
```

// default value syntax?

```csharp
b := x?.IsTrue          // Opt<Bool>
b := x?<false>.IsTrue   // No, <> is for type parameters
b := x?(false).IsTrue   // No, () is for function parameters and lists
b := x?[false].IsTrue   // No, [] is for indexing arrays/lists
b := x?|false|.IsTrue   // No, || is alt for captures
b := x?false?.IsTrue    // could work
// other?
```

---

## Arithmetic Operators

Add specific operators for arithmetic operations for saturation (ignoring overflow and filling up till full) and wrapping (wrapping around to start over again when full) behavior.

Postfix arithmetic operator with:

| Modifier | Description |
| -- | -- |
| `\|` | Overflow/underflow will saturate (hits the wall). |
| `~` | Overflow/underflow will wrap around (cyclic). (`.NET unchecked`) |
| `!` | Overflow/underflow will return an error (`Err<T>`). |
| `!!` | Overflow/underflow will throw an exception. (`.NET checked`) |
| `?` | Overflow/underflow will result in `Nothing` (`Opt<T>`). |
| _(none)_ | Behavior is determined by the compiler profile (one of the above). |

Modifier symbols are never arithmetic operator symbols, so a modified operator always reads as one operator.

```csharp
a: U8 = 245
b: U8 = 125

c: U8 = a +| b      // saturate (255)
c: U8 = a +~ b      // wrap around (114)
c: U8! = a +! b     // Error (Err<U8>)
c: U8 = a +!! b     // overflow exception (370 > U8)
c: U8? = a +? b     // Nothing (Opt<U8>)
c: U8 = a + b       // compiler profile default
```

The modifier applies to the operator it is attached to, in the type the operation is performed in. Narrowing a result to a smaller type is a conversion, not an arithmetic operation (see [Conversion](../lang/conversion.md)).

---

## Type Operator Symbols

| Symbol | Type | Description |
| --- | --- | --- |
| `!` | `Err<T>` | Error return value or T |
| `?` | `Opt<T>` | Optional; T or Nothing |
| `^` | `Mut<T>` | Mutable T |

A type gets a postfix symbol only when the symbol's family _is_ that concept (`?` maybe, `!` error, `^` mutable).
All other wrapper types are written by name: `Ref<T>`, `Atom<T>`, `Ptr<T>`, `Async<T>`, ...

Binary Operators that operate on types.

| Operator | Description |
| --- | --- |
| `:?` | type test: has type? (`Bool`). Like `v?` tests for a value, `x :? T` tests for a type. |

There are no other type operators:

| Operation | How |
| --- | --- |
| Type test | `x :? T` (an immutable `x` is narrowed to `T` afterwards) |
| Down cast | `T(x)`: conversion syntax (see [Conversion](../lang/conversion.md)). A failing cast propagates like any other failure. |
| Optional cast | `match` with a type arm |
| Up cast | implicit |
| Type equality | `x#type = T` |

```csharp
MyStruct : BaseStruct
    ...
x := MyStruct
b := x :? OtherStruct       // false
z: BaseStruct = x           // up cast (implicit)
m := MyStruct(z)            // down cast (fails if z is not a MyStruct)

y := match z                // optional cast
    s: MyStruct -> s
    _ -> _                  // y: Opt<MyStruct>
```

> TBD: rule for the result type of a `match` (to be written in [Match](../expressions/match.md)): when one or more arms result in `_` (Nothing) and the others in `T`, the type of the `match` expression is `Opt<T>`.
>
> Former alternatives (`:=` / `=:` equals type, `<:?` / `<?:` optional cast, `<:` down cast, `:>` / `>:` up cast) have been removed. `:=` already means declare with inferred type, and `<` is the structural prefix.

---

## String Operator Symbols

Operators for strings and characters.

| Operator | Description |
| --- | --- |
| `'' ''` | Delimiters for a symbol name with special characters. |
| `=~` | Case (and culture) insensitive equals. |
| `<>~` | Case (and culture) insensitive not-equals. |
| `>~` | Case (and culture) insensitive greater-than - compare. |
| `<~` | Case (and culture) insensitive lesser-than  - compare. |
| `>=~` | Case (and culture) insensitive greater-than-or-equal - compare. |
| `=<~` | Case (and culture) insensitive lesser-than-or-equal - compare. |
| `s[2..6]` | sub-string using `Range`. |
| `<+` | Concat a string*. (also works for chars?) |
| `/<+` | Concat (join) a string with path separator. |
| `'x'<+` | Concat (join) a string with any (x) separator? |

*) Perhaps concatenation does not require an operator at all?
`"Hello " "World"`

Array operators should also work on string character items.

## Comparison Operator with Margin Symbols

Operators for comparing numbers with a margin.
Typically useful for floating point numbers but should also work for integers.

| Operator | Description |
| --- | --- |
| `=  ~` | Equals with a margin. |
| `<>  ~` | Not equals with a margin. |
| `>  ~` | Greater than with a margin. |
| `<  ~` | Lesser than with a margin. |
| `>=  ~` | Greater-than-or-equal. |
| `=<  ~` | Lesser-than-or-equal. |

These operators require 3 operands.

```csharp
f := 3.14
pi := 3.1415

// how to specify three operands?
if f = pi ~ 0.01
    // equals (within margin)

// or use 'in' syntax /bc its basically a range
if f in pi ~ 0.01
```

Also applicable to dates:

```csharp
now := DateTime.Now
d := DateTime.Now.AddDays(1)    // just some date

if f = now ~ TimeSpan.FromMinutes(1)
    // equals (within margin)
```

---

## Structural Operators

Operators starting with `<` work on the _structure_ of the left operand instead of its value.
They work on arrays and lists (the structure is the items) and on objects, including anonymous objects (the structure is the fields, matched by name).
The plain form produces a new instance; the left operand is unchanged.
The mutating form (`<+=`, `<-=`, `<|=`, ...) writes the result back into the (mutable) left operand.

| Operator | Array / List (items) | Object (fields) |
| --- | --- | --- |
| `<+` | add item(s) to the end (concat, same as string concat) | add field(s). Error if a field already exists. |
| `<-` | remove item(s) (first exact match) | remove field(s). Results in a (smaller) anonymous type. |
| `<\|` | union: add the item(s) that are not already present | _with_: union of the fields, the right side wins. |
| `<&` | intersection: keep only the items present in both | keep only the fields present in both. |
| `<^` | symmetric difference: keep the items present in only one of both | keep the fields present in only one of both. |
| `</` | split into chunks/tuples of n | - |
| `<=` | copy / map into the left | map fields by name into the shape of the left. |

`<|` and `<=` both copy values from right to left. The difference is the type of the result:

- `<|` results in the union of both shapes. On an anonymous object this can add fields. On a named type the right side must be a subset of its fields, so the result keeps the same type.
- `<=` always keeps the shape (and type) of the left. Matching fields are mapped (possibly converted), the others are ignored. This makes it the mapping operator between different types.

`<=` only works on structured types (structures, objects, maps, arrays / lists). On primitive types it is a compile error: `a <= b` on two numbers is almost certainly meant as _less or equal_, which is `=<` in Z#.

```csharp
p := { Name = "John", Age = 42 }

p2 := p <| { Age = 43 }             // { Name = "John", Age = 43 }
p3 := p <+ { City = "Amsterdam" }   // { Name = "John", Age = 42, City = "Amsterdam" }
p4 := p <- { Age }                  // { Name = "John" }
p5 := p <& { Name = "" }            // { Name = "John" }

s: Struct
    ...
s2 := s <| { fld1 = 42 }            // copy of s with fld1 changed (s2: Struct)
```

```csharp
arr1 := (1, 2, 3)
arr2 := (3, 4)

arr3 := arr1 <| arr2    // (1, 2, 3, 4)
arr4 := arr1 <& arr2    // (3)
arr5 := arr1 <^ arr2    // (1, 2, 4)
```

Insert at front is a library function.

> TBD: Syntax for naming fields without values in `<-` (`{ Age }`).

---

## Array and List operators

Operators for working with `Array<T>` and `List<T>` types.

Any arithmetic operator also works on arrays and lists.
Operators starting with `<` indicates a structural operation, like adding or removing elements from an array/list.

> TBD: Do we also support comparison and logical operators that result in an array of booleans?
> What about set operators (union etc.)?
> What happens (arithmetic) if arrays are not of the same size? -> Take shortest, ignore long tail.

| Operator | Description |
| --- | --- |
| `+` | adds the values of each element of two array/lists together. |
| `-` | subtracts the values from each element of two array/lists. |
| `x` | x = any arithmetic operator that acts on each element of the two array/lists. |
| `+=` | mutable - add a value to all array/list elements |
| `-=` | mutable - subtract a value from all array/list elements |
| `*=` | mutable - multiply a value with all array/list elements |
| `x=` | mutable - x = any arithmetic operator acts on all elements of the array/list |
| `<+` `<-` `<^` `<\|` `<&` `</` `<=` | structural operators, see [Structural Operators](#structural-operators) |
| `in` | test if item(s) is in array/list (contains) |
| `not in` | test if item(s) is not in array/list (not contains) |
| `.>` | aggregate over all instances in the array (Select) - property or function |
| `[..]` | Range operator returns a sub array/list. |
| `[i]` | Index (i) operator returns a single item. |
| `[?i]` | Safe index (i) operator returns an optional single item. |
| `[*]` | returns the enumerator of an array/list. |
| `[**]` | returns the parallel enumerator of an array/list. |
| `[**%n]` | returns the parallel enumerator of an array/list for max `n`-threads. |
| `[**/n]` | returns the parallel enumerator of an array/list with `n`-elements in each thread. |

> Zip and Unzip operators have been removed.

```csharp
arr1 := (1, 2, 3, 4, 5)
arr2 := (5, 4, 3, 2, 1)

arr3 := arr1 + arr2
// arr3 = (6, 6, 6, 6, 6)

// mutable
arr1 <+= 42      // arr1 = (1, 2, 3, 4, 5, 42)
arr2.InsertAt(0, 101)  // arr2 = (101, 5, 4, 3, 2, 1) - library function

arr4 := arr1 <+ arr2
// arr4 = (1, 2, 3, 4, 5, 42, 101, 5, 4, 3, 2, 1)
```

```csharp
arr := (1, 2, 3, 4, 5)
// add single item
arr2 := arr <+ 6   // arr2 = (1, 2, 3, 4, 5, 6)
// remove multiple items - whole array must match!
arr3 := arr <- (1, 2, 3)    // arr3 = (4, 5, 6)
// 'arr' is unchanged

// contains
b := 4 in arr            // true
c := (2, 4) not in arr   // true
```

Special collection operator for accessing the same property on an array of objects.

```csharp
Person
    Name: Str
    Age: U8

arr: Array<Person> = (...)

// returns the names of all persons in the array
names := arr.>Name
// names: Array<Str>

// Collect multiple properties?
people := arr.>Name, arr.>Age
people := { Name = arr.>Name, Age = arr.>Age }
// people: Array<{Str, U8}>
```

```csharp
// Does it work the same with functions?

fn: (self: Person): Str
    return "$self.Name is $self.Age years old."

// returns "'name' is 'age' years old." for all persons in the array
names := arr.>fn

// Do we allow more function parameters?

// 'self' param is array item-type
fn: (self: Person, magic: U8): Str
    return "$self.Name called with $magic."

// calls fn with magic=42 for all persons in the array
names := arr.>fn(42)
```

> TBD: Allow 'array programming' operator (overloads) that target simd instructions?

---

## Mutating Operators

All these operators work as follows:
_Read left, [op] right to left, write left_

These operators cannot be overloaded, they simply use the standard operators.

| Operator | Description |
| --- | --- |
| `++` | Increment variable (prefix or postfix) |
| `--` | Decrement variable (prefix or postfix) |
| `xx` | x = Any arithmentic operator (prefix or postfix) |
| `+=` | read (left) - add (right to left) - write (left) |
| `-=` | read - subtract - write |
| `*=` | read - multiply - write |
| `/=` | read - divide - write |
| `%=` | read - modulo/remainder - write |
| `**=` | read - power - write |
| `>>=` | read - shift right - write |
| `<<=` | read - shift left - write |
| `>\|=` | read - roll right - write |
| `\|<=` | read - roll left - write |
| ?? | read - test - write (locking?) - what operator? |
| `!=` | read - error?? - write |
| `&=` | read - bit and - write |
| `\|=` | read - bit or - write |
| `^=` | read - bit xor - write |
| `$=` | read - ?? - write |
| `^=` | read - 'immutable' ?? - write |
| `<\|=` | read - with (structural union) - write |

> TBD: Some of these could be interlocked. Perhaps the `Atom<T>` type automatically uses an interlocked impl?

What syntax to use? `|+=|`, `\\+=`

| Operator | Description |
| --- | --- |
| `\|+=\|` | interlocked (read - add - write) |
| `\|-=\|` | interlocked (read - subtract - write) |

Do we allow a list of right values? (yes)

```csharp
a :=^ 42    // mutable
a += (12, 23, 34)
// a = a + 12 + 23 + 34

a += (x, y, z)
```

---

## Data Type Wrapper Conversion Assignment Operators

Goal is to have a quick and easy way to convert from a normal data type `T` to one of the wrapper types (of T).
Note the type-operator is on the right side of the equals sign.

> TBD: Should we add a `:` to the syntax to indicate it is about types? `opt =:? x`

| Operator | Description |
| --- | --- |
| `=!` | `Err<T>` = `T` |
| `=?` | `Opt<T>` = `T` |
| `=^` | `Mut<T>` = `T` |

```csharp
a: U8 = 42
// infer var type ':'
err :=! a    // err: Err<U8>
opt :=? a    // opt: Opt<U8>

// explicit var type
mut: Mut<U8>    // declare mutable
mut =^ a

// as parameters inline
fnErr(!a)
fnOpt(?a)
fnImm(^a)

// or named parameters
fnErr(err =! a)
fnOpt(opt =? a)
fnImm(imm =^ a)
```

> Or are these conversions implicit? No.

Alternate syntax:

```csharp
a: U8 = 42
err := a.!    // err: Err<U8>
opt := a.?    // opt: Opt<U8>
```

---

> What if operators cause overflow (or underflow)? A bitwise shift `<<` can shift out bits - sort of the point. Does every operator determine for itself if overflow is a problem or is there a general principle? Yes - each operator (function) determines the consequences for itself.

> What syntax to specifically use/call checked or unchecked operator implementations? How to ignore overflow?

See `TBD` note at the top about checked, wrap around and saturate operators.

---

> TBD: Is operator overloading useful for operators other than arithmetic, comparable and possibly logical?

Which operators will definitely not be overloadable?

---

Allow all operators to be written in postfix notation?

In some cases a postfix notation makes your code more readable, especially when chaining operators.

```csharp
x := 42 101 +       // meh
x := (42, 101) +    // list notation
```
