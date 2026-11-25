# Keywords

The number of keywords have been kept to a minimum. Much of the language is expressed through context. Keywords are always lower case.

| Keyword | Description
|--|--
| `loop` | Loop statement
| `break` | Break out of a loop
| `brk` | Alternative for Break
| `continue` | Skip to next iteration
| `cont` | Alternative continue?
| `if` | Conditional branch
| `else` | Opposite of conditional branch
| `return` | Exit a Function
| `ret` | Alternative return?
| `yield` | Exits a Coroutine Function
| `in` | Iterating values / range condition *
| `self` | A bound Type
| `match` | Pattern matching (also handles errors and exceptions, see [Errors](../lang/errors.md))

## Project and File keywords

| Keyword | Description
|--|--
| `assembly` | Identify the name of the resulting assembly.
| `ref` | Reference an external library/assembly/project
| `module` | Declare a file belongs to a named module.
| `use` | Reference public symbols from another module.
| `pub` | Declare symbols to be publicly available from a module.
| `include` | Copy in the content of a file at the location of this statement.

> TBD: `use` keyword to bring other things in scope besides modules.

```csharp
MyStruct
    fld1: U8
    fld2: Str
s = MyStruct
    ...

// with scope/indent?
use s
    fld1 = 42   // MyStruct.fld1 of s

// without scope/indent?
use s
fld1 = 42   // MyStruct.fld1 of s
```

---

> These keywords are reserved words and cannot be used as [identifiers](identifiers.md) in the code.

*) as an alternative for collections that support 'Contains'?

```csharp
arr = (1, 2, 3, 4, 5)
if 42 in arr        // false
    ...

// can also use in a match expression?
```

---

## Operators

- and
- or
- not
- xor?

---

## Reserved for future use

| Keyword | Description
| -- | --
| `defer` | defers execution until end of scope
| `errdefer` | defers execution till exit with error
| `out` | opposite of in?
| `var` | mutable variable? / thread local?
| `val` | constant value?
| `def` | default? (looks like define!)
| `ref` | explicit reference? / read-only pointer?
| `deref` | unpack a reference (too similar to defer)
| `rec` | recursive function?
| `use` | inline module imports? IDisposable wrapper?
| `any` | a type that can be anything (object) (not a keyword but a type?)
| `asm` | inline assembly (IL?)
| `with` | context variables
| `async` | async execution (state machine)
| `await` | awaiting async execution
| `fun` | for pure functions?
| `class` | for future OOP extension
| `use` | import public code from another module
| `pub` | public  code
| `is` | TBD
| `as` | TBD

> Use `var` to make mutable variables and normal syntax is always immutable?
Kotlin uses val for constants/immutable and var for mutable vars.

> Use `is` to do type checking and for use in conditional expressions?

> Use `as` to cast (`Option<T>`) to a type?

> Note: `is` / `as` are explored ideas borrowed from C#. The current model uses the `:?` operator for type tests, conversion syntax `T(x)` for casts and `match` for optional casts (see [Operators](operators.md)).
