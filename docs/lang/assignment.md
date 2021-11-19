# Assignment

| Operator | Function
|--|--
| = | Assign value (right to left)

> The left operand of an assignment expression can not be a literal value.

Here 42 is assigned to the variable `a`.

```C#
a = 42
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
if a = myFunc()      // error!
    ...
```

---

## Boolean Assignment

> Assignment clashes with comparison is-equal operator!

Use parenthesis around comparison.

`[var] = (<bool expression>)`

`[var] = (<bool expression>)?`

```csharp
x = 42
// unclear syntax
a = x = 42

// use () for comparison expression
a = (x = 42)
// a: Bool = true

// make it a rule for all bool expressions?
a = (x > 101)
// a: Bool = false
```

---

## Assignment Chaining

The assignment can be chained across multiple variables (left operand) that all get assigned the same value (right operand). Type inference works as expected and the inferred type is applied to all untyped vars.

```csharp
// all var types are inferred
a = b = c = 42
// a: U8 = 42
// b: U8 = 42
// c: U8 = 42

b: U16
a = b = c: U32 = 42
// a: U8 = 42
// b: U16 = 42
// c: U32 = 42
```

---

### Structure Assignment

Assigning structures works the same as scalar values but for structures only the reference is copied.

```csharp
s: MyStruct
    fld1 = 42
    fld2 = "42"

// reference passing
x = s
// this will make a new copy of Struct
x <= s

// only changes x, not s
x.fld1 = 101

b = (s.fld1 = 42)
// b = true
```

More on the `<=` operator:

```csharp
s: MyStruct
    ...

// copy same type
x <= s  // x: MyStruct

YourStruct
    fld1: U8
    fld2: Str

// map / transform (default by field names)
y: YourStruct <= s  // y: YourStruct -mapped
```

---

## Conditional Assignment

Instead of an `if` statement, use the `?=` operator for use with optional values.

The `?=` operator only assigns the value to the left operand if it does not already have a value.

```csharp
a: U8?
a ?= 42
// a = 42

a: U8?
a = 42
a ?= 101
// a = 42
```

---

## Atomic Assignment

Protect from interrupts / thread context switches.

A way to ensure an assignment operation is uninterrupted.

- unconditional
- conditional (exchange-if)

---

### Atomic Type

A wrapper type that indicates the instance is accessed atomically.

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
[s]
    s.fld1 = 101
    s.fld2 = "101"
```

---

### Volatile

Volatile is used when the contents of a variable (memory location) can be changed from outside the program (memory mapped IO/hardware registers) or outside the compiler's field of view (interrupt service routines).

If we are able to tag ISR's in the language, we can automatically tag all used variables as volatile.
Memory mapped IO is harder to auto detect.

> What 3-letter abbreviation means volatile?

```csharp
// we may want to save IO for language supported Input/Output instructions.
a: IO<U8> = 42          // used in functional (impure)
a: Volatile<U8> = 42    // too long?
a: Vol<U8> = 42         // unclear?
a: Weak<U8> = 42        // save for ptrs?
a: Alt<U8> = 42         // alternate
a: Soft<U8> = 42

// like optional - but different
// will get weird with optional/error
a: &U8 = 42
```

> TBD: Memory Fences! => default .NET

---

## Deconstruction

Deconstruction is _copying_ the value into a variable.

```csharp
a, b = ...
// a and b can be used as separate vars
sum = add(a, b)
```

> TBD: auto deconstruction?

```csharp
add: (a: U8, b: U8): U16
    ...

// deconstruct either by name or in order (types must match exactly)
sum = add(x)    // a, b = x
// use spread operator to make deconstruction clear?
sum = add(...x) // a, b = x
```

### Deconstructing an Array

```C#
// spread operator ...
a, b, ...rest = [1, 2, 3, 4, 5]

// a: U8 = 1
// b: U8 = 2
// rest: Array<U8> = [3, 4, 5]
```

### Deconstructing Function Parameters

```C#
arr = [1, 2, 3, 4, 5]

func: (p: U8)
    ...
func(...arr)    // called 5 times?

func5: (p1: U8, p2: U8, p3: U8, p4: U8, p5: U8)
    ...
func5(...arr)   // or with 5 params?
func5(arr)      // without spread operator?

// what if the param count does not match array item count?
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

a, b = s      // error! field names must match (case insensitive)
// or when in-order - all fields must be specified (or ignored)

a, _, _ = s
// a: U8 = <value of s.Field1>
// <value of s.Field2 and s.Field3> are ignored

_, _, a = s
// a: U8 = <value of s.Field3>
// <value of s.Field1 and s.Field2> are ignored

```

> Is there a need to override how deconstruction is done on a (custom) type? => yes

---

### Swap Scalar Variables

(unlike structs)

```C#
x = 42
y = 101

// left = deconstruct
// right = anonymous struct '{}'
x, y = { y, x }
// x = 101
// y = 42

// swap operator
x <=> y
```

---

> TBD

Make type decl and assignment distinct.

```csharp
a: U8       // declare typed var (default init)
a: U8 = 1   // declare typed and init
a:= 1       // declare inferred-type and init
a = 1       // assign - 'a' must already be declared
```

Could this help improving variable assignments?
