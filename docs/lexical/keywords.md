# Keywords

The number of keywords have been kept to a minimum. Much of the language is expressed through context. Keywords are always lower case.

| Keyword | Description
|--|--
| `loop` | Loop statement
| `break` | Break out of a loop
| `continue` | Skip to next iteration
| `cont` | Alternative continue?
| `if` | Conditional branch
| `else` | Opposite of conditional branch
| `return` | Exit a Function
| `ret` | Alternative return?
| `yield` | Exits a Coroutine Function
| `in` | Iterating values
| `self` | A bound Type
| `try` | Propagate if Error
| `catch` | Handle Error
| `match` | Pattern matching

---

> These keywords are reserved words and cannot be used as [identifiers](identifiers.md) in the code.

## Operators

- and
- or
- not
- xor?

## Reserved for future use

Keyword | Description
--|--
`defer` | defers execution until end of scope
`errdefer` | defers execution till exit with error
`out` | opposite of in?
`var` | thread local?
`def` | default? (looks like define!)
`ref` | explicit reference? / read-only pointer?
`deref` | unpack a reference (too similar to defer)
`use` | inline module imports?
`any` | a type that can be anything (object)
`asm` | inline assembly (IL?)
`with` | context variables
`async` | async execution
`await` | awaiting async execution
`fun` | for pure functions?

> Use `var` to make mutable variables and normal syntax is always immutable?
