# Expressions

Almost all operators are binary and use an infix notation: the operator symbols is in between its (left and right) operands.

A unary expression is one that takes only one operand. Negating a numeric value (`-5`) is an example.

> Note that operators that mutate a value in-place are absent in Z#. Examples of these operators are `++` and `--`. These are not supported. Operators like `+=` and `-=` are short hand for `x = x + y`. These are supported.

The supported expressions are divided in these categories:

- [Arithmetic](arithmetic.md)
- [Logical](logical.md)
- [Comparison](comparison.md)
- [Bitwise](bitwise.md)
- [Match](match.md)

> TBD: Assignment is also an expression.

The value returned from the assignment expression is the old/previous value of the assignment target.
The resulting value does not have to be assigned or explicitly discarded.

```csharp
v := 42
// here we run into a syntax problem:
// - this means compare c to 101 and x would be Bool
// - we don't support chained assignments
x := v = 101
// if we support something like this, swap would become
v := x = v

// function returns the previous value of 'self'
add42: (self: U8): U8
    return self += 42
    // would even be better with auto-return of last expression value
    self += 42

v := 42
x := v.add42()
// x = 42
```

> TBD: Make it a different operator?

```csharp
v := 42
// '<=' means: assign and push out previous value
// '<=' is also used in mapping
x := v <= 101
// some other operator?
x := v \= 101
x := v != 101
```

Downside is that this allows you to embed assignments into other expression constructs (passing args to a function) which we are trying to avoid (for readability)...

---

## Precedence

When using more than one operator in a single expression, some operators are processed before others: precedence.

The use of `()` indicates that everything inside the parentheses is processed before interacting with other parts of the expression.

---

## Operator Overloads

Every operator can be specialized (overloaded) for a specific type. These functions are all [checked](../compiler/checked.md#Operator-Overloads) by the compiler if they confirm to the rules for that specific operator.

The name and prototype of the function that implements an operator overload is bound to specific rules, otherwise it cannot be resolved.

```csharp
[name]: ([self], [params]): [ret]
```

- `[name]` Each [operator](../lexical/operators.md#Operator-Symbols) has an associated function name.
- `[self]` The left-hand-side operand is defined as a self parameter. For unary operators only the right-hand-side parameter is defined as a self parameter.
- `[params]` The right-hand-side of the operator is the second parameter. Based on the type of this self parameter an initial match is made. A secondary match is made on the exact type of the second parameter - if available. If the type is an `Array<T>` it can be passed multiple values at once for syntax like `a += (1, 42, 101)`.
- `[ret]` The resulting type of the operator function. Usually this is the same type as the self parameter, but it can be different. The compiler will respect this type for further processing. For comparison and logical operators the return type is always `Bool`.

```csharp
Vector
    x: I32
    y: I32
    z: I32

// '-' operator
Negate: (self: Vector): Vector
    return { x = -self.x, y = -self.y, z = -self.z }
// '=' operator
IsEqual: (self: Vector, other: Vector): Bool
    return self.x = other.x and
            self.y = other.y and
            self.z = other.z

v = Vector
    x = 42
    y = 68
    z = 101

notV := -v       // calls Negate operator overload function
areEq := (v = notV)  // calls IsEqual operator overload function
```

These operator overload functions can also be called directly if one prefers the explicit nature of them.

> Not sure if the bitwise operators overloading would not result in a totally different semantics. For now not supported.

---

> TBD

- Pattern Matching Expressions: `c is >= 'a' and <= 'z' or >= 'A' and <= 'Z'` (see also match)

- Value block expressions that eliminate the need for `return` for instance.
