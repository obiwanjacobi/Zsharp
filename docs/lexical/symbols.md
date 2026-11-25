# Symbol Charter

This charter gives every symbol in Z# **one core concept**. A symbol may appear in several contexts, but each use must be a recognizable application of that concept. A use that does not fit is an _outlier_ and has to move to another symbol or be dropped.

Second rule: **one meaning per syntactic context.** Related meanings do not help the parser. If two uses of a symbol can appear in the same position, one of them must go.

Legend:

| Mark | Meaning |
| -- | -- |
| ✅ | Fits the core concept. |
| ⚠️ | Outlier or collision. Includes a proposed action. |
| ❓ | Open decision. |
| ❌ | Has been removed. |

---

## Syntactic Contexts

The parser must be able to determine the context from syntax alone (not from types).

| Context | Code | Where | Example |
|--|--|--|--|
| Type | `T` | After `:` in a declaration, inside `< >` type arguments | `p: U8?` |
| Binary | `B` | Between two operands, with a space on both sides | `a + b` |
| Prefix | `Pre` | Directly before an operand (no space) | `-a`, `?fn(v)` |
| Postfix | `Post` | Directly after an operand (no space) | `v?`, `a#size` |
| Assign | `A` | Composed around `=` in an assignment or declaration | `a +=  b`, `o :=? v` |
| Delimiter | `D` | Paired symbols enclosing content | `( )`, `\|x\|` |
| Directive | `Dir` | At the start of a line or declaration | `#module`, `#!` |
| Rule | `R` | A `#` rule line inside a type or function body | `#value > 0` |
| Lexical | `L` | Inside literals or identifiers | `0x_FF`, `"$a"` |

### Spacing rule (proposed)

Several contexts can only be told apart by whitespace, so whitespace has to be part of the spec:

- Binary operators **must** have a space on both sides: `a - b`.
- Prefix and postfix operators **must not** be separated from their operand: `-a`, `v?`.
- Type argument brackets attach directly to the name: `List<U8>` (type arguments) vs `a < b` (comparison).

---

## Single Symbols

### `?`: Maybe / test for presence

| Ctx | Use | Status |
| -- | -- | -- |
| `T` | `T?` = `Opt<T>` | ✅ |
| `Post` | `v?` has a value (to `Bool`) | ✅ |
| `Post` | `x?.y` safe navigation, `[?i]` safe index | ✅ |
| `Pre` | `?root.a.b` safe navigation over the whole path | ✅ |
| `Pre` | `fn(?v)` / `?fn(v)` call only if present | ✅ |
| `A` | `?=` assign if empty, `?+=` conditional read-add-write | ✅ |
| `A` | `=?` / `:=?` wrap value as `Opt<T>` | ✅ |
| `B` mod | `a +? b` overflow results in Nothing | ✅ |
| `B` | `a ?? b` fallback | ✅ |
| `Dir` | `#?` compile-time warning | ✅ (a softer form of `#!`) |
| `B` | `c ? a ; b` ternary | ✅ "maybe a, otherwise b" |
| `B` | `c ? a : b` ternary | ❌ `:` collides with type annotations. Now `c ? a ; b`. |
| match | `x: T if cond ->` match guard | ✅ (was `x: T ? cond ->`, which collided with `T?`) |
| `A` | `?=` "read-test-write (locking?)" | ❌ Collided with conditional assign. |
| `B` | `x?false?.y` default value in safe navigation | ⚠️ Drop; use `x?.y ?? false` |

### `!`: Error / attention

| Ctx | Use | Status |
|--|--|--|
| `T` | `T!` = `Err<T>` | ✅ |
| `T` | `T!!` may throw an exception (return type) | ✅ "double error". One token in type context: never `Err<Err<T>>`. ❓ "not pure" is a different concept, keep it out. |
| `Post` | `fn()!` propagate error | ❌ Propagation is automatic; handling is done with `match` ([errors](../lang/errors.md)). `try` / `catch` are gone too. |
| `B` mod | `a +! b` overflow results in an Error | ✅ |
| `B` mod | `a +!! b` overflow throws an exception | ✅ "double error": escalated from value to exception |
| `B` | `a !! b` error fallback (twin of `??`): `parse(s) !! 0` | ❓ Reserved. Shorthand for a `match` with one catch-all error arm. Exceptions too? |
| `A` | `=!` / `:=!` wrap value as `Err<T>` | ✅ |
| `Pre` | `fnErr(!a)` wrap argument as `Err<T>` | ✅ |
| `Dir` | `#!` compile-time error | ✅ |
| `Dir` | `#! fn: ...` compile-time function | ❌ Collided with compile-time error. Now `#fn: ...` (see `#`) |
| `A` | `!=` "read-error-write" | ❌ Undefined. |
| `Post` | factorial | ❌ Not the error concept. Library: `n.Factorial()`. |

### `:`: Is-a / has type

| Ctx | Use | Status |
|--|--|--|
| `T` | `name: Type` declaration | ✅ |
| `T` | `fn: (p: U8): U8` function signature and return type | ✅ |
| `T` | `MyStruct: Base` derive / based on | ✅ |
| `A` | `:=` declare with inferred type | ✅ |
| `B` | `x :? T` type test (`Bool`): "has type?", like `v?` "has value?" | ✅ Narrows an immutable `x` afterwards |
| `B` | `<:` down cast, `:>` up cast, `<:?` optional cast, `:=` / `=:` equals type | ❌ Down cast is conversion syntax `T(x)`, optional cast is `match`, up cast is implicit, type equality is `x#type = T`. `:=` already declares, `<` is structural. |
| `Dir` | `#label:` compile-time label | ✅ (declares a name) |
| `D` | `[[: Attr]]` decorator target = return value | ✅ |
| `B` | `::` traits (reserved) | ✅ |
| `B` | `c ? a : b` ternary else-branch | ❌ Same position as `name : Type`. Now `c ? a ; b` (see `;`). |
| `D` | `[0..10: 2]` range step | ❌ Not a type. Now `[0..10 / 2]` (see `/`). |

### `=`: Equality / binding

| Ctx | Use | Status |
|--|--|--|
| statement | `a = b` assignment (a statement, never an expression) | ✅ |
| `B` | `a = b` equality (in an expression) | ✅ |
| `T` | `p: U8 = 42` default value, `<#T=U8>` template default | ✅ |
| decl | `Opt1 = 1` enum value, `Alias = Fn` alias, `MyType = Other<T>` type alias | ✅ Undeclared left side means alias (decided semantically) |
| arg | `fn(p = 42)` named argument | ✅ |
| `A` | the anchor for all composed assignment operators (see Composition) | ✅ |

### `~`: Approximately / around / cyclic

| Ctx | Use | Status |
|--|--|--|
| `B` | `x ~ m` range around a value: `[x-m, x+m]` | ✅ |
| `B` | `f in pi ~ 0.01` margin comparison (range + `in`) | ✅ Replaces the six 3-operand margin operators |
| `B` | `=~ <>~ >~ <~ >=~ =<~` case/culture-insensitive comparison | ✅ |
| `B` mod | `a +~ b` overflow wraps around | ✅ a wave: cyclic |
| `Pre` | `~a` bitwise invert | ✅ for now. ⚠️ Fits neither "approximately" nor "cyclic"; legal because prefix is its own context. Alternative: `not` on integers. |
| `B` | `<~` unzip | ❌ Collided with case-insensitive `<~`. |
| `B` | `~` set symmetric difference | ❌ Use `<^` (structural xor). |

### `^`: Mutable, and as a binary operator: xor

Two families in distinct contexts: _attached_ (type postfix, prefix, after `=`) = mutable, _binary_ (spaced, or before `=`) = xor.

| Ctx | Use | Status |
|--|--|--|
| `T` | `T^` = `Mut<T>` | ✅ |
| `A` | `:=^`, `=^`, `:<=^` mutable binding / wrap | ✅ |
| `Pre` | `fn(^a)` pass as mutable | ✅ |
| `T` | `(p: ^U8)` prefix form in a parameter | ⚠️ Type operators are postfix. Use `U8^`. |
| `B` | `a ^ b` bitwise xor | ✅ binary context, spacing rule |
| `T` | `Struct1 ^ Struct2` type difference | ✅ type-level xor: fields in exactly one of both |
| `B` | `<^` structural xor (symmetric difference of items or fields) | ✅ See structural family. |
| `B` | `<^` insert at front | ❌ Library function. |
| `A` | `^=` xor and write back | ✅ before `=` changes the operation |
| `A` | `^=` "immutable??" | ❌ Collided with xor-assign. |

### `|`: Union / or, and as a delimiter: fence

`|` has two families, and they occupy different syntactic contexts: _binary_ = union, _delimiter / modifier_ = fence (boundary).

| Ctx | Use | Status |
|--|--|--|
| `B` | `a \| b` bitwise or | ✅ union of bits |
| `T` | `Struct1 \| Struct2`, `fld: U8 \|` memory union | ✅ ❓ Keep memory unions at all on .NET? |
| enum | `opt1 \| opt2` single-choice options | ✅ |
| `B` | set union | ❌ Plain `\|` is not defined on sets. Use structural `<\|`. |
| `B` | `\|\|` cascading or (`c = 42 \|\| 101`) | ❓ Parked. Suggestion: drop, use `c in (42, 101)`. Needs `c = (42 \|\| 101)` grouping, unlike any other binary operator. |
| `D` | `\|x\|` capture | ✅ fence around captured state |
| `D` | `\|\|x, y\|\|` master capture | ✅ |
| `A` | `\|+=\|` interlocked operation | ✅ fence around the operation |
| `B` mod | `a +\| b` saturate | ✅ hits the wall |
| `B` | `\|<` `>\|` rotate | ✅ wall the bits wrap around |
| `B` | `\|>` pipe | ❌ Neither union nor fence. Use dot-call on the first parameter: `42.fn3().fn2()`. |
| `B` | `<\|` with / structural union (items or fields) | ✅ union. See structural family. |
| `B` | `<\|` insert child at front | ❌ Tree operations are library functions. |
| `B` | `<\|` reverse pipe, zip | ❌ Collided with insert child. Zip is a library function. |
| `A` | `:=\|` wrap-around conversion | ❌ Removed from [conversion](../lang/conversion.md). |

### `&`: Intersection / and

| Ctx | Use | Status |
|--|--|--|
| `B` | `a & b` bitwise and | ✅ |
| `T` | `Struct1 & Struct2` common fields | ✅ |
| `B` | set intersection | ❌ Plain `&` is not defined on sets. Use structural `<&`. |
| `B` | `&&` cascading and | ❓ Parked. Suggestion: drop. With `=` always false; with comparisons it is a range: `c in ['a'..'z']`. |
| `T` | `T&` = `Ref<T>` | ❌ Not intersection. Write `Ref<T>`. |
| `Post` | `s&.fn()` safe navigation alternative | ⚠️ `?.` exists. Drop. |
| `D` | `\|&x\|` context variable marker | ⚠️ Drop. |
| `B` | `<&` structural intersection (items or fields) | ✅ See structural family. |
| `B` | `<&` add as child | ❌ Tree operations are library functions. |
| `B` | `&~` `~&` set difference | ❌ Use structural `<-`. |

### `%`: Modular arithmetic

| Ctx | Use | Status |
|--|--|--|
| `B` | `a % b` remainder, `%%` modulo | ✅ |
| `B` | `/%` divide and remainder | ✅ |
| `B` mod | `a +% b` wrap around | ❌ Modifiers are never arithmetic symbols. Wrap is `+~`. |
| `T` | `T%` = `Atom<T>` | ❌ Not modular. Write `Atom<T>`. |
| `A` | `=%` wrap value as `Atom<T>` | ❌ Follows `Atom`. |
| `D` | `[**%n]` parallel with max n threads | ⚠️ Drop (library). |

### `*`: Multiply, and as a pattern: all

Two concepts in distinct contexts: _binary_ = multiply, _inside `[ ]` or after `.`_ = all.

| Ctx | Use | Status |
|--|--|--|
| `B` | `a * b` multiply, `**` power | ✅ |
| `D` | `[*]` enumerator over all items, `[**]` parallel | ✅ ❓ library? |
| use | `System.*` all of the namespace | ✅ |
| `T` | `T*` = `Ptr<T>` | ❌ Not "all". Write `Ptr<T>`. |
| `Pre` | `fn(*a)`, `=*` wrap as `Ptr<T>` | ❌ Follows `Ptr`. |

### `+` `-`: Add / join, subtract / remove

| Ctx | Use | Status |
|--|--|--|
| `B` | `+` add, `-` subtract | ✅ |
| `Pre` | `-a` negate, `-[0..5]` reversed range | ✅ |
| `Pre` | `+a` "absolute?" | ⚠️ Undefined. ❓ |
| `R` | `#value += "Abc"` starts with, `=+` ends with, `+=+` contains, and the `-` negations | ❓ Coherent "joined at this end" but shares characters with mutating `+=`. Acceptable only because `R` is a separate context. |

### `/`: Divide / split

| Ctx | Use | Status |
|--|--|--|
| `B` | `a / b` divide, `//` root | ✅ |
| `B` | `</` split into chunks | ✅ structural divide |
| `D` | `[0..10 / 2]` range step | ✅ divide the range into steps |
| `B` | `/<+` join with path separator | ⚠️ Library. Drop. |

### `<` `>`: Order / direction

| Ctx | Use | Status |
|--|--|--|
| `B` | `<` `>` `>=` `=<` comparison | ✅ |
| `B` | `<<` `>>` shift, `>>>` arithmetic shift | ✅ |
| `D` | `< >` type parameters and arguments | ✅ spacing rule |
| `B` | `<>` not equal | ✅ ⚠️ `Open<>` empty type arguments: forbid empty `<>` |
| `B` | `<` prefix = structural operation (see below) | ✅ |
| `B` | `<=>` swap | ✅ both directions |
| `T` | `T>` = `Def<T>` | ❌ Breaks `List<U8>>`. Write `Def<T>`. |
| `B` | `->>` parallel execution | ✅ |
| `B` | `->>` arithmetic shift | ❌ Collided with parallel execution. Shift is `>>>`. |
| `B` | `.>` aggregate over all elements: `arr.>Name`, `arr.>fn(42)` | ✅ |
| `B` | `.>` call public nested function | ❌ Public nested functions removed. |

### `.`: Navigate into

| Ctx | Use | Status |
|--|--|--|
| `B` | `a.b` member access, `A.B.C` qualified names | ✅ |
| `Pre` | `.opt2` enum member (type inferred) | ✅ |
| `Pre` | `.fn()` on a continuation line, `.fld = x` implicit self | ✅ |
| `B` | `a..b` range | ✅ continues a sequence |
| `B` | `c..add(4)..sub(2)` fluent cascade | ❌ Removed. Use a `\|c\|` capture scope. |
| `Pre` | `...x` spread | ✅ unpacks a sequence |
| `L` | decimal point `1.0` | ✅ Float literals always have an integer and a decimal part (no `1.` or `.42`), so `[1..4]` and `42.fn()` lex correctly. |
| `D` | `(0..10)` range in list syntax | ✅ No collision now the cascade is gone: `..` always means range. |
| decon | `a, ..rest := ...` | ⚠️ Use `...rest` |
| `Post` | `a.!` `a.?` wrapper conversions | ⚠️ `=!` / `=?` exist. Drop. |

### `,` and `;`: Separators

| Ctx | Use | Status |
|--|--|--|
| `D` | `,` list item separator (newline = alternative) | ✅ |
| `D` | `fn(42,)` partial application | ⚠️ Collides with trailing commas. Drop. |
| `B` | `c ? a ; b` ternary else-branch | ✅ separates the two outcomes |

### `_`: Nothing / don't care

| Ctx | Use | Status |
|--|--|--|
| name | `_` discard (`_ = fn()`, `(_: U8)`) | ✅ |
| `L` | digit separator `0xFF_FF`, identifier separator `My_Fn` (ignored) | ✅ |
| value | `return _`, `o = _` Nothing | ✅ |
| decl | `Age: U8 _`, `fn: () _` nothing follows | ✅ |
| `T` | `fn<_, Str>` infer this one | ✅ |
| lambda | `_.Name` implicit parameter | ✅ |
| decl | `MyAlias = _` empty (weak) alias | ✅ |
| name | `_id` leading underscore = private / hidden | ⚠️ `_` is ignored when matching, so `_id` = `id`. Proposed: a **leading** `_` is significant (part of the name), only inner `_` is ignored. |

### `#`: Compile time

| Ctx | Use | Status |
|--|--|--|
| `Dir` | `#module`, `#use`, `#pub` and other directives | ✅ |
| `Dir` | `#_` comment (ignore at compile time), `##` comment with warning | ✅ |
| `Dir` | `#!` error, `#?` warning, `#$` info | ✅ |
| `Dir` | `#label:` compile-time label | ✅ |
| `D` | `<#T>` template parameter, `(#count: U16)` template value parameter | ✅ |
| `Post` | `a#size`, `T#max` intrinsic attributes | ✅ |
| `R` | `#value > 0` type and parameter rules | ✅ |
| `Pre` | `#code := 0` compile-time variable, `#fn(x)` compile-time call, `#if` | ✅ |
| `Pre` | `#$name` name as string | ✅ |
| `Dir` | `#fn: ...` compile-time function | ✅ (was `#! fn:`, which collided with `#!` error) |
| `Post` | `.#type` vs `#type` | ⚠️ Pick one: `a#type` |

### `$`: String

| Ctx | Use | Status |
|--|--|--|
| `L` | `"$a"`, `"$a:x"` interpolation | ✅ ⚠️ Docs also use `"{a}"`; fix the examples. |
| `Pre` | `$Name` canonical string constant | ✅ |
| `Pre` | `$"..."` resource string | ✅ ❓ |
| `L` | `` `$ `` literal `$` | ✅ |
| `A` | `$=` | ⚠️ Undefined. Drop. |

### `@`: Compiler extension

Core: _hand over to a compiler extension_.

| Ctx | Use | Status |
|--|--|--|
| `Pre` | `@Xml"""..."""` structured literal handled by an extension | ✅ ❓ syntax may still change ([literals](literals.md)) |
| `L` | `package@v1.2.3`, `System.Console@>=5.0.*` version in a package / assembly reference | ✅ Part of the reference token, not an operator |
| `Pre` | `@inline()` compiler function | ❌ Compile-time hint: a decorator (see [meta](../compiler/meta.md)) |
| `Pre` | keyword escape (`@if` as a name) | ❌ Use a quoted name `''if''` |
| `L` | disable string formatting | ❌ `` ` `` escapes exist. |
| decl | `@recurseFn:` recursive marker | ❌ `[[Recursive]]` or `rec` keyword ❓ ([functions](../lang/functions.md)) |
| `T` | `T@` = `Atom<T>` / `Mem<T>` | ❌ Two meanings, neither an extension. Write `Atom<T>` / `Mem<T>`. |

### `'` `"` `` ` ``: Quoting

| Ctx | Use | Status |
|--|--|--|
| `L` | `'X'` character, `'ABCD'` multi-char | ✅ |
| `L` | `''.>>.''` quoted (special-character) name, `''if''` keyword escape | ✅ |
| `L` | `"..."` string, `"""..."""` raw / multi-line | ✅ |
| `L` | `` ` `` escape character inside strings | ✅ |
| name | `prime'` identifier suffix | ⚠️ `f(p', 'x')` lexes `', '` as a character. Drop. |
| match | `` `@` `` regex pattern | ⚠️ Drop. |

### Brackets

| Symbol | Core | Uses | Outliers |
|--|--|--|--|
| `( )` | list | parameters, arguments, tuples, list / array literals, grouping (a one-element list is its element), `ptr()` call as function object | ✅ |
| `[ ]` | select from | index, range, slice, `[?i]`, `[*]` | ⚠️ `[x]` captures (old docs, use `\|x\|`), `[1..6)` interval notation (breaks bracket matching, drop), map literal `["k" = 1]` ❓ |
| `{ }` | structure | anonymous structs / tuples, object construction, `{s + o}` object logic | ⚠️ `{ }` as escape from indentation (drop), `WithParent<T> { }` group (drop) ❓ map literal |
| `[[ ]]` | decorator | `[[Attr]]`, `[[p: Attr]]`, `[[: Attr]]` | ✅ ❓ alternatives kept open (see below) |
| `[< >]` | .NET code attribute | `[<Serializable>]` an attribute _type_ (`<>` = type) | ✅ ❓ |
| `[( )]` | function decorator | `[(Trace)]` a decorator that _is_ a function: code injection (`()` = call) | ✅ ❓ |
| `\| \|` | fence | captures | see `\|` |

The decorator forms stand on their own line before a declaration. An index (`arr[(1, 2)]`) is always attached to an operand, so position separates them.

---

## Type Operators

A type gets a postfix symbol only when the symbol's family _is_ that concept. The same symbol then also works as prefix (`fn(?a)`) and as value modifier after `=` (`=?`).

| Symbol | Type | Family |
|--|--|--|
| `?` | `Opt<T>` | maybe |
| `!` | `Err<T>` | error |
| `!!` | may throw (exception) | error, escalated. Type and overflow modifier only (no prefix / `=!!` form). |
| `^` | `Mut<T>` | mutable |

All other wrapper types are written by name: `Ref<T>`, `Atom<T>`, `Ptr<T>`, `Async<T>`, `Mem<T>`, ...
Former type symbols: `T&` Ref ❌, `T%` Atom ❌, `T*` Ptr ❌, `T>` Def ❌, `T@` Atom / Mem ❌.

---

## Keyword Operators

| Keyword | Core | Uses | Outliers |
|--|--|--|--|
| `and` `or` `not` `xor` | logic | logical, and (proposed) bitwise on integers | ⚠️ `loop a in x and b in y` loop combinators ❓, `v? or 101` value fallback (result is not `Bool`; use `??`) ❓ |
| `or` | union | `Struct1 or Struct2` constrained variant | ✅ |
| `and` | union | `Struct1 and Struct2` type addition | ⚠️ Contradicts "no multiple inheritance" ([structures](../lang/structures.md)). ❓ |
| `in` | membership | `x in arr`, `x in [0..9]`, `x in pi ~ m` | ✅ ⚠️ `loop n in r` iterates. Inside `loop`, `in` always means iteration. |
| `is` | type / pattern test | `x is T`, `c is >= 'a' and =< 'z'` (single pattern, `Bool`) | ❓ Explored only (from C#). Type test is `:?`. Value patterns: see `\|\|` cascading or. |
| `as` | cast | `x as T` (`Opt<T>`) | ❓ Explored only (from C#). Optional cast is `match`. |

---

## Composition

Compound operators are built from parts in a fixed order:

```txt
[?]   [<]          <base>         [modifier]      [=]    [modifier]
cond  structural   + - * / % ...  overflow        assign value / type
```

| Part | Symbols | Meaning |
|--|--|--|
| Conditional prefix | `?` | Perform only if the operand has a value: `?+`, `?=` |
| Structural prefix | `<` | Operate on the structure (items) of the left operand, not on its value |
| Base operator | `+ - * / % ** // << >> ...` | The operation |
| Overflow modifier | `\|` saturate, `~` wrap, `!` error, `!!` exception, `?` nothing | What happens on overflow (none = compiler profile). Never an arithmetic symbol. |
| `=` | `=` | Write the result back (read - operate - write) |
| Value/type modifier | `! ? ^` after `=` | Wrap the assigned value: `=?` Opt, `=!` Err, `=^` Mut |

Rule: **a modifier before `=` changes the operation, a modifier after `=` changes the value.**
So `?=` = assign if empty, `=?` = assign wrapped as optional. `+=` = add and write back.

### Structural family (`<` prefix)

The structure is the items of an array / list, or the fields (by name) of an object, including anonymous objects. See [operators](operators.md#structural-operators).

| Operator | Items | Fields | Status |
|--|--|--|--|
| `<+` | append / concatenate (also strings) | add field(s) | ✅ |
| `<-` | remove item(s) | remove field(s) | ✅ ("assign mutable" ❌) |
| `<\|` | union | _with_: union, right side wins | ✅ (insert child at front ❌, zip ❌) |
| `<&` | intersection | common fields only | ✅ (add as child ❌) |
| `<^` | symmetric difference | fields in exactly one side | ✅ (insert at front ❌, library function) |
| `<~` | unzip | - | ❌ Collided with case-insensitive `<~`. |
| `</` | split into chunks | - | ✅ |
| `<=` | copy / map into left | map by name into the left's shape | ✅ Structured types only: on primitives it is a compile error ("did you mean `=<`?"), because it reads as _less or equal_ to most programmers. |
| `<+=` `<-=` ... | mutating versions | mutating versions | ✅ |

`<|` results in the union of both shapes (a named type keeps its type: the right side must be a subset). `<=` always keeps the left's shape and type.

---

## Collisions to Resolve

These are the cases where the same characters end up with two meanings in the same syntactic context:

| Characters | Meaning 1 | Meaning 2 | Proposed |
| -- | -- | -- | -- |
| `<~` | case-insensitive less-than (`<` + `~`) | unzip (`<` structural + `~`) | drop unzip ✅ |
| `<\|` | reverse pipe | zip / insert child | drop reverse pipe ✅, drop zip ✅ |
| `<-` | remove items | assign mutable (reserved) ❌ | remove mutable assign |
| `?=` | conditional assign | read-test-write (locking) ❌ | keep conditional assign |
| `++` `--` | increment / decrement (postfix) | element-wise add / subtract ([array](../types/array.md)) | element-wise via plain `+` on arrays ✅ |
| `**` `//` | power / root | element-wise multiply / divide | follows `++` decision ✅ |
| `:` | declaration `a : T` | ternary else `c ? a : b` | `c ? a ; b` ✅ |
| `T ? x` / `T?` | match guard | optional type | `if` as guard ✅ |
| `#!` | compile-time error | compile-time function | `#fn:` ✅ |
| `->>` | arithmetic shift | parallel execution | `>>>` for shift ✅ |
| `.>` | collect property | call nested function | remove public nested functions ✅ |
| `~` mod / `\|` mod | saturate vs exception ([operators](operators.md)) | wrap vs saturate ([conversion](../lang/conversion.md)) | one table in operators.md: `\|` saturate, `~` wrap, `!!` exception ✅ |
| `//` | root operator | comment in all doc examples (real comment: `#_`) | doc convention for syntax coloring, see [index](../index.md) ✅ |

---

## Open Decisions

1. ~~`^` = mutable, and therefore bitwise operators become `and` / `or` / `xor` / `not` on integers?~~ ✅ `^` keeps xor in binary context (mutable when attached). Remaining: `~a` bitwise invert stays for now, or `not` on integers?
2. Ternary: `c ? a ; b` ✅, or only the `if c -> a else b` expression? ❌ (not yet anyway)
3. Range step: `[0..10 / 2]` ✅ or another separator instead of `:` ❌?
4. ~~Saturate: `+|` (wall) or `+~` (approximately)?~~ ✅ `+|` saturate, `+~` wrap, `+!!` exception.
5. ~~Error propagation: postfix `fn()!` or prefix `try fn()`?~~ ✅ Neither. Errors and exceptions propagate automatically and are handled with `match`.
6. ~~Type tests: operators (`:?`, `<:?`) or keywords (`is`, `as`)?~~ ✅ `:?` type test, `T(x)` down cast, `match` optional cast, up cast implicit. `is` / `as` remain explored ideas only.
7. ~~Operators for `Ref<T>`, `Atom<T>`: new symbols or none?~~ ✅ None. Only `?` `!` `^` are type operators (see Type operators).
8. ~~`|>` pipe: allowed exception in the `|` family?~~ ✅ No pipe. Dot-call works on any first parameter; `self` is only about binding.
9. ~~`@` as the "compiler extension" symbol: include `@inline` and keyword escape?~~ ✅ `@` only prefixes structured literals. Inline is a decorator, keyword escape is `''if''`.
10. Map literal: `{ "k" = 1 }`, `[ "k" = 1 ]`, or constructor only?
11. Cascading `||` / `&&` (and `is` value patterns): keep, or use `in` with values, ranges and lists of ranges (`c in (['a'..'z'], ['A'..'Z'])`)? Parked.
