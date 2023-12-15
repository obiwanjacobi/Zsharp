# Match

The `match` expression tries to select the first pattern that matches the specified operand.

Here are some examples of patterns that can be used:

```C#
v = 42
x = 24

a = match v
    0 ->        // by literal numerical value
    "" ->       // by literal string value
    x ->        // by variable value
    n: U8 ->    // by type (with var name)
    Str ->      // by type (without var name)
    is Str ->   // use is?
    s: MyStruct ? s.fld1 = 20 ->  // by type with filter
    o: OtherStruct { fld1: 42, fld2: "42" } -> // by (property) prototype
    _ ->        // default (ignore) case
```

> Can the chosen variable names inside the match expression shadow those in the outer code? For instance if variable `x` is used in the outer code, can it be used inside the match as `x: U8`?

The syntax continues after the `->` which contains the result of the expression when that specific pattern matched.

```csharp
s = "42"
a = match s
    "" -> "Empty"
    "0"             // use indent-solves multiple lines
        "Zero"
    s: Str -> s
    _ -> ""
```

This example results in `"The Answer"` based on the string `"42"`:

```C#
s = "42"

a = match s
    "" -> "Empty"
    "0" -> "Zero"
    "42" -> "The Answer"    // this pattern will be chosen
    _ -> ""

// a = "The Answer (Str)
```

> How to detect an optional value?

```csharp
o: Opt<U8>  // nothing

a = match o
    ?? -> "Nothing"
    _ -> o.Str()
```

When more comprehensive logic is required to compute the result a function can be called to yield that result.

> The result is always of one type only (unless perhaps when we support union (||) types or compound (or) types) where only optional `?` or Error '!' can be added by other branches.

Here an example on matching a type pattern with multiple patterns for the same type - but different filters.

```C#
s = MyStruct
    field1 = 12

a = match s
    n: U8 -> make42(n)
    x: MyStruct ? x.field1 = 12 -> 42
    x: MyStruct -> 0

// a = 42 (U8)
```

> Implicit is the use of the 'equals' operator on native value data types. What if we make that explicit and therefor also allow other operators too.

```csharp
a = 42
s = match a
    < 10 -> "Smaller than 10"                   // `<` operator
    >= 10 and =< 100 -> "between 10 and 100"    // logical and
    > 100 -> "bigger than 100"                     // `>` operator
    // don't need an _ case, we've covered all numbers
```

The two patterns for `MyStruct` differ in filter. That is why this works.

> The compiler checks if later patterns are still reachable.

Patterns for values and patterns for types can be used at the same time. Type matching is done base on the underlying `#typeid`.

> TBD: How will the compiler be able to assess if a pattern will be reachable?

> TBD: more support for expressions? Like returning an expression from a scope (no return statement needed)? A loop that results in a value (`break 42`)??

> TBD: Match expressions on lists? Match on the content of the list, for instance the number of items in the list:

```csharp
r = match list
    [] -> 0     // match empty list (array syntax)
    () -> 0     // match empty list (tuple syntax)
    (x) -> x    // match list with one item.
    (x, ...lst) -> recurse(lst) + x    // recursive sum function
```

> TBD: match on an anonymous type / tuple.

```csharp
t = (42, "101")
r = match t
    (42, _) -> 0    // ignore one tuple property
    (I32, Str)      // match on types
    (,)             // any tuple with two properties
```

---

> TBD

The match expression always yields a result. It would be nice to be able to use pattern matching without having to return a result. Basically allow a void-match (statement).

```csharp
log: (txt: Str)
    ...

a = 42
match a
    = 42 -> log("This is the answer!")
    > 100 -> log("bigger than 100")
    I32 -> log("This int is smaller than 100 and its not 42.")
```

---

> TBD

Replace the `match` keyword with `is` to align other pattern matching constructions like: `c is >= 'a' and <= 'z' or >= 'A' and <= 'Z'`

```C#
a = s is
    n: U8 -> make42(n)
    x: MyStruct ? x.field1 = 12 -> 42
    x: MyStruct -> 0

a = 42
s = a is
    < 10 -> "Smaller than 10"                   // `<` operator
    >= 10 and =< 100 -> "between 10 and 100"    // logical and
    > 100 -> "bigger than 100"                  // `>` operator

// boolean result
b = c is (>= 'a' and =< 'z') or (>= 'A' and =< 'Z')
```

? How to distinguish (parsing: both start with `var = var is ...` - only diff is the newline) between pattern matching conditional expressions (bool) and selecting a matching case (switch)?

---

> TBD

Match on regex?

```csharp
s = "person@domain.com"
x = match s
    `@` -> ...
    `[a-z]` -> ...
    _ -> error
```

---

> TBD

Match on Exceptions?

```csharp
x = match someOperation()
    // how to differentiate from normal result type checking?
    ExceptionType -> ...
    _ -> happy_flow
```
