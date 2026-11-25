# Set

> Research set theory.

are sets ordered (index) or unordered?

functions

- union
- intersects
- difference
- is sub / super set
- add / remove (join/disjoin?)
- contains
- iterator

## Set Operators

Set Theory? Set, intersect, union, subset, superset, powerset, (relative) complement, symmetric difference, membership, cardinality, ordered pair, cartesian product, wellknown sets (natural, integer, rational, real, complex).

Operations

All set operations are structural: they work on the elements of the set, not on their values. They use the structural `<` operators (see [Structural Operators](../lexical/operators.md#structural-operators)).
The plain form produces a new set; the mutating form (`<|=`, `<&=`, ...) writes the result back into the (mutable) left set.

| Operator | Fn Name | Description
| -- | -- | --
| `<\|` | Union | union sets (duplicates overwritten)
| `<&` | Intersection | intersection sets (only shared elements)
| `<-` | Difference | difference sets (left without right). Also removes single elements.
| `<^` | SymmetricDifference | symmetric difference (elements in only one of both)
| `<+` | Add | add element(s) to the set

Right without left is written as `b <- a`.

```csharp
a := Set(1, 2, 3)
b := Set(3, 4)

u := a <| b     // (1, 2, 3, 4)
i := a <& b     // (3)
d := a <- b     // (1, 2)
x := a <^ b     // (1, 2, 4)
s := a <+ 5     // (1, 2, 3, 5)
```

Comparison

| Operator | Fn Name | Description
| -- | -- | --
| `=` | Equal | Exact equality
| `<>` | NotEqual | Exact inequality
| `=<` | SubSetOrEqual | left is a subset of right or equal
| `>=` | SuperSetOrEqual | left is a superset of right or equal
| `<` | SubSet | left is subset of right
| `>` | SuperSet | left is a superset of right
| `in` | Contains | an element is contained in set
| `not in` | NotContains | an element is not contained in set

Comparisons result in a `Bool` and do not build a new set, so they do not use the structural `<` prefix.

Arithmetic

Not supported. Element-wise arithmetic (as on arrays) pairs elements by position, and a set has no positions.
The plain operators (`+`, `-`, `|`, `&`, `^`, ...) are a compile error on sets, so they can never be confused with the structural operators.
