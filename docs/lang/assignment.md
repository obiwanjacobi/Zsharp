# Assignment

| Operator | Function
|--|--
| = | Assign value (right to left)
| := | Assign value (right to left) with inferred type

> The left operand of an assignment expression can not be a literal value.

Variable declaration and assignment.

```csharp
a: U8       // declare typed var (default init)
a: U8 = 1   // declare typed and init
a:= 1       // declare inferred-type and init
a = 1       // assign - 'a' must already be declared and be mutable
```

Here 42 is assigned to the variable `a` (inferred type).

```C#
a := 42
```

The receiving (left) operand can also be a path to a field.

```C#
MyStruct
    field1: U8

s: MyStruct
s.field1 = 42
```

```C#
z: MyStruct
    field1 = 42     // inline assignment
```

> Because both the `Equals` and the `Assignment` operators use the '`=`' symbol, it is not possible to assign values inside a comparison expression.

```C#
if a := myFunc()      // error! cannot assign inside a condition
if a = myFunc()      // error! variable a is not declared
    ...
```

---

## Boolean Assignment

> Assignment clashes with comparison is-equal operator!

Use parenthesis around comparison.

`[var] = (<bool expression>)`

`[var] = (<bool expression>)?`

```csharp
x := 42
// unclear syntax?
a: Bool = x = 42

// use () for comparison expression
a := (x = 42)
// a: Bool = true

// make it a rule for all bool expressions?
a := (x > 101)
// a: Bool = false
```

---

## Assignment Chaining

> TBD: It would be easiest to make assignment a statement (therefor cannot be used inside conditions)
and that would also mean that assignment cannot be chained.

The assignment can be chained across multiple variables (left operand) that all get assigned the same value (right operand). Type inference works as expected and the inferred type is applied to all untyped vars.

```csharp
// all var types are inferred
a := b := c := 42
// a: U8 = 42
// b: U8 = 42
// c: U8 = 42

b: Mut<U16>     // must be mutable to be assigned later
a := b = c: U32 = 42
// a: U8 = 42
// b: U16 = 42
// c: U32 = 42
```

> How to determine its not an is-equals comparison?

---

### Structure Assignment

Assigning structures works the same as scalar values but for structures only the reference is copied.

```csharp
s: MyStruct
    fld1 = 42
    fld2 = "42"

// reference passing
x := s
// this will make a new copy of Struct
x :<=^ s     // mutable assignment

// only changes x, not s
x.fld1 = 101

b := (s.fld1 = 42)
// b = true
```

More on the `<=` operator:

```csharp
s: MyStruct
    ...

// copy same type
x :<= s             // x: MyStruct
x : MyStruct <= s   // or this

YourStruct
    fld1: U8
    fld2: Str

// map / transform (default by field names)
y: YourStruct <= s  // y: YourStruct -mapped
```

See also [Structure Transformation](./structures.md#transformation-mapping).

---

## Conditional Assignment

Instead of an `if` statement, use the `?=` operator for use with optional values.

The `?=` operator only assigns the value to the left operand if it does not already have a value.

```csharp
a: Mut<U8>?
a ?= 42
// a = 42

b: Mut<U8>?
b = 42
b ?= 101
// b = 42
```

Only works with `Opt<T>` values or dotnet `null` reference types.

---

## Atomic Assignment

Protect from interrupts / thread context switches.

A way to ensure an assignment operation is uninterrupted.

- unconditional (lock)
- conditional (exchange-if)

---

### Atomic Type

A wrapper type that indicates the instance is accessed atomically.

> `Atom<T>` is implicitly mutable `Mut<T>`.

```csharp
// assignment is easy because Atom
a: Atom<U8> = 42
a = 101

b: U8   // does not need to be Atom
// supply function on Atom
a.Exchange(b.Ptr())
a.ExchangeIf(a = 42, b.Ptr())
```

Atomic locking requires guaranteed unlocking... (defer keyword)

```csharp
a: Atom<U8> = 42

a.Lock()
defer a.Unlock()    // defer keyword

// compact syntax?
a.Lock() -> defer .Unlock()

// atomic assignment
a = 101

// end of scope unlocks
```

`Atom<T>` implements IDisposable and unlocks automatically on destruction when going out of scope.

Using `Atom<T>` also allows to manage access to structs that are larger than a single primitive data type.

> TBD: Syntax to set multiple fields under lock?

```csharp
Struct
    fld1: U8
    fld2: Str

s: Atom<Struct>
// using object notation?
s = {fld1: 42, fld2: "42"}  // Atom overrides = operator

// or a capture?
|s|
    s.fld1 = 101
    s.fld2 = "101"
```

---

### Volatile

Volatile is used when the contents of a variable (memory location) can be changed from outside the program (memory mapped IO/hardware registers) or outside the compiler's field of view (interrupt service routines).

```csharp
// 'Volatile<T>' too long?
a: Volatile<U8> = 42    // write
x := a                  // read
```

> TBD: Memory Fences! => default .NET

See also the `Volatile` dotnet class.

---

## Deconstruction

Deconstruction is _copying_ the value into a variable or parameter.

```csharp
a, b := ...
// a and b can be used as separate vars
sum = add(a, b)
```

> TBD: auto deconstruction? => no

```csharp
add: (a: U8, b: U8): U16
    ...

// deconstruct either by name or in order (types must match exactly)
sum := add(x)    // a, b = x
// use spread operator to make deconstruction clear? => yes
sum := add(...x) // a, b = x
```

### Deconstructing an Array

```C#
// spread operator ...
a, b, ...rest := [1, 2, 3, 4, 5]
// -or- spread the array and the rest is a range? (Yes, better)
a, b, ..rest := ...[1, 2, 3, 4, 5]

// a: U8 = 1
// b: U8 = 2
// rest: Array<U8> = [3, 4, 5]
```

### Deconstructing Function Parameters

```C#
arr := [1, 2, 3, 4, 5]

func: (p: U8)
    ...
func(...arr)    // called 5 times? (no, unclear!)

func5: (p1: U8, p2: U8, p3: U8, p4: U8, p5: U8)
    ...
func5(...arr)   // or with 5 params?
func5(arr)      // without spread operator? => no

// what if the param count does not match array item count?
// must at least be the number of parameters without default values.

// should also work with a tuple (anonymous type)
params := { p1=1, p2=2, p3=3, p4=4, p5=5 }
func5(...params)
```

### Deconstructing a Structure

```C#
MyStruct
    Field1: U8
    Field2: U8
    Field3: U8

s = MyStruct
    ...

// partial by name
field1, field3 = s
// field1: U8 = <value of s.Field1>
// field3: U8 = <value of s.Field3>
// <value of s.Field2> is not used

a, b := s      // error! field names must match (case insensitive)
// or when in-order - all fields must be specified (or ignored)

a, _, _ := s
// a: U8 = <value of s.Field1>
// <value of s.Field2 and s.Field3> are ignored

_, _, a := s
// a: U8 = <value of s.Field3>
// <value of s.Field1 and s.Field2> are ignored

```

> Use the spread operator `...` on the right-hand-side value to indicate it is being deconstructed?

> Is there a need to override how deconstruction is done on a (custom) type? => yes

> TBD: Do we need to distinguish between array, tuple and object deconstruction?

```csharp
// array (old)
a, b := [1, 2]
// (new) list/array
a, b := (1, 2)
// object/tuple
a, b := {a=1, b=2}
```

> TBD: deconstruct variable names with aliases.

```csharp
o =: {x=42, y=101}

aliasX=x, aliasY=y := o
// here we need a different '=' operator for aliases!
// can we parse the '=' in these terms?
aliasX=.x, aliasY=.y := o   // '=.' operator? (both alias and name are in scope)
aliasX=_x, aliasY=_y := o   // '=_' operator? (only alias is in scope - see Identifiers/Aliases)
```

---

### Swap Scalar Variables

(unlike structs)

```C#
x := 42
y := 101

// left = deconstruct
// right = anonymous struct '{}'
x, y = { y, x }
// x = 101
// y = 42

// swap operator (needed?)
x <=> y
```

---
