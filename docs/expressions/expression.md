# Expressions

Almost all operators are binary and use an infix notation: the operator symbols is in between its (left and right) operands.

A unary expression is one that takes only one operand. Negating a numeric value (`-5`) is an example.

> Note that operators that mutate a value in-place are absent in Z#. Examples of these operators are `++`, `--` and `+=`, `-=`. These are not supported.

The supported expressions are divided in these categories:

- [Arithmetic](arithmetic.md)
- [Logical](logical.md)
- [Comparison](comparison.md)
- [Bitwise](bitwise.md)
- [Assignment](assignment.md)

See also the [Match Expression](../lang/match.md)

## Precedence

When using more than one operator in a single expression, some operators are processed before others: precedence.

The use of `()` indicates that everything inside the parentheses is processed before interacting with other parts of the expression.

---

> TBD
