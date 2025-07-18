# Expressions

Almost all operators are binary and use an infix notation: the operator symbols is in between its (left and right) operands.

A unary expression is one that takes only one operand. Negating a numeric value (`-5`) is an example.

The supported expressions are divided in these categories:

- [Arithmetic](arithmetic.md)
- [Logical](logical.md)
- [Comparison](comparison.md)
- [Bitwise](bitwise.md)
- [Match](match.md)

> Note: Assignment is **not** an expression.

```csharp
v := 42
// compare c to 101 => x: Bool
x := v = 101
```

---

## Operator Precedence

When using more than one operator in a single expression, some operators are processed before others: precedence.

The use of `()` indicates that everything inside the parentheses is processed before interacting with other parts of the expression.

See also [operators](../lexical/operators.md#Precedence)

---

## Operator Overloads

Every operator can be specialized (overloaded) for a specific type.
These functions are all [checked](../compiler/checked.md#Operator-Overloads) by the compiler if they confirm to the rules for that specific operator.

The name and prototype of the function that implements an operator overload is bound to specific rules, otherwise it cannot be resolved.

```csharp
[[Operator]]
name: (self, params): ret
```

- `[[Operator]]` decorator code attribute that indicates what operator is implemented by this function.
- `name` Each [operator](../lexical/operators.md#Operator-Symbols) has an associated recommended function name, but any name can be chosen.
- `self` The left-hand-side operand is defined as a self parameter. For unary operators only the right-hand-side parameter is defined as a self parameter.
- `params` The right-hand-side of the operator is the second parameter. Based on the type of this self parameter an initial match is made. A secondary match is made on the exact type of the second parameter - if available. If the type is an `Array<T>` it can be passed multiple values at once for syntax like `a += (1, 42, 101)`.
- `ret` The resulting type of the operator function. Usually this is the same type as the self parameter, but it can be different. The compiler will respect this type for further processing. For comparison and logical operators the return type is always `Bool`.

```csharp
Vector
    x: I32
    y: I32
    z: I32

// '-' operator
[[UnaryOperator("-")]]
NegateVector: (self: Vector): Vector
    return { x = -self.x, y = -self.y, z = -self.z }
// '=' operator
[[BinaryOperator("=")]]
VectorIsEqual: (self: Vector, other: Vector): Bool
    return self.x = other.x and
            self.y = other.y and
            self.z = other.z

v = Vector
    x = 42
    y = 68
    z = 101

notV := -v       // calls NegateVector operator overload function
areEq := (v = notV)  // calls VectorIsEqual operator overload function
```

See [Operator Functions](../lang/functions.md#Operator-Functions) for more info.

These operator overload functions can also be called directly if one prefers the explicit nature of them.

> Not sure if the bitwise operators overloading would not result in a totally different semantics. For now not supported.

---

> TBD

- Pattern Matching Expressions: `c is >= 'a' and <= 'z' or >= 'A' and <= 'Z'` (see also match)
- Value block expressions that eliminate the need for `return` for instance.

> TBD: more support for expressions? Like returning an expression from a scope (no return statement needed)?
> A loop that results in a value (`break 42`)??
