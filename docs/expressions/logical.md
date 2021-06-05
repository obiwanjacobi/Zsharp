# Logical Expressions

Logical expressions evaluate boolean values. They are implemented lazily and only access the right operand when the left operand not already determines the result. This is pretty typical optimization for the `or` operator.

| Operator | Function
|-------|------
| and | And
| or | Or
| not | Not

```C#
true and true       // true
false or true       // true
not true and true   // false
```

The next example demonstrates the lazy evaluation of a logical expression. `funTwo` will never be called because `funOne` returned `true` which satisfied the `or` operator.

```C#
funOne(): Bool
    return true
funTwo(): Bool
    return false

if funOne() or funTwo()
    code_is_executed
```

---

> TBD

Allow these combinations?

```C#
true not and true       // nand: false
false not or true       // nor: false

true nand true          // nand: false
false nor true          // nor: false
true xor true           // xor: false
```
