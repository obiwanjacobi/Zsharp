# Identifiers

Identifiers name program entities such as variable, functions and types. These names must follow these rules:

- Cannot be empty.
- Cannot contain spaces.
- Cannot contain 'special' characters (;,.:<>{}() etc).
- Cannot contain operator characters (+-/*) (*TBD).
- Cannot start with a digit (*TBD).
- Can contain `_`.
- Can contain alpha-numeric characters.
- Can end with a `'` (parameters/variables only).

Here are examples of valid identifiers:

```csharp
My_Function1
someVariable
_hidden
prime'
```

Here are examples of _invalid_ identifiers:

```csharp
1Function
some variable
my-type
```

> TBD: I don't see any objection to add '`-`' (minus sign) as a valid character for an identifier...? Or start an identifier with a digit? The parser should be able to match that correctly.

> TBD: Is there a practical reason not to allow special characters or even names that only consist of special characters (as custom operators)? This only works when white-space is significant: `hello-world` would be a name, but `hello - world` would be a subtraction.

> TBD: Specifically allowing `.` in an identifier could provide some flexibility for applying namespaces within a module. It could also be used to separate name parts with a non-optional character (`_` is an optional character).

---

## Symbols and Labels

A general kind of identifiers are code labels.

This syntax is basically consistent of all named code elements.

```csharp
symbol: <some other code>
#label: <some other code>
```

Symbols will be compiled into the binary while labels are only available at compile-time.

This allows some language features where an identifier/label/symbol refers to some specific code.

| Label | Code | Type | Description |
| -- | -- | -- | -- |
| var | `x: U8` | Type | Variable declaration |
| fn | `fn: (p: U8): U8` | FunctionType | Function declaration |
| struct | `MyStruct` | Type | Structure declaration |
| field | `fld: U8` | Type | Field declaration |
| loop | `#outer: loop 42` | LocationType | Loop identifier* |

*) The label is prefixed with a `#` to indicate it is a compile-time label and not a symbol that will be in the binary.

---

## Case Sensitivity

- Type identifiers **must** start with an upper case character.
- Local Variables and Function Parameters **must** have a lower case first character.
- Function names can use either upper or lower case first character.

Identifiers are the same when:

- The first letter matches exactly (case sensitive)
- All other letters match (case insensitive)
- `_` are ignored (removed) when comparing.

> TBD: exclude function names from having to match case of first letter of function name? That would allow for fully adapting your own naming conventions - at least for functions. Types still have to start with a capital first character.

---

### Discard

Using a discard `_` in an identifier is ignored during matching.

```csharp
My_Function: (p: U8)
    ...

Myfunction(42)  // calls My_Function

//----

Myfunction: (p: U8)
    ...

My_Function(42)  // calls Myfunction
```

When an identifier only consists of a discard `_` it indicates it is not used.

```csharp
myFn: (_: U8): U8    // param not used
    ...
_ = myFn(42)        // return value not used

```

If an identifier starts with a `_` it is hidden from immediate public access.
The field will be `internal` in .NET for a public structure, otherwise it'll be private.

```csharp
MyStruct
    _id: U32
    Name: Str

s: MyStruct
s.[intellisense does not show _id]
```

> Do we want to give meaning to identifiers ending with a `_`? Could we use this for weak-functions (or weak-anything)?

> TBD: We could also use `_` as a prefix for any symbol to indicate it is a private symbol. This would not require to explicitly `pub` a public symbol.

```csharp
_privateFn: ()
    ...
publicFn: ()
    ...
```

---

## Fully Qualified Names

```csharp
MyModule.Lib.MyFunction
```

---

### Navigation

Any identifier that has a `.` (dot) in it will be split up in parts. Those parts will be used to navigate to the correct 'location' where that symbol is defined.

- Absolute navigation: starting at the root scope (global)
- Relative navigation: starting at the current scope

> For now, relative navigation is only supported for symbols that do not have dot-name. In the `use` statement, long navigation paths (namespaces) can be aliased to shorter names to be used in the module's source code.

`use` statements for external modules are always specified in absolute (fully qualified) names.
`use` statements for local modules are specified with relative (in the same namespace) or absolute names.

```csharp
// external module
#use System.Console   // type
#use System.*         // namespace

// local module
#use myModule         // module (type)
#use namespace.module // same as external module
```

Fully qualified names in code are resolved during compilation.

```csharp
// this should not require an #use statement
fn: ()
    System.Console.WriteLine("Hello World")
```

Navigation into fields does work relatively.

```csharp
MyStruct
    fld1: U8
    fld2: Str

s : MyStruct =
    fld1 = 42
    fld2 = "42"

// struct field navigation
x := s.fld1
```

```csharp
MyEnum
    Opt1
    Opt2

// enum field navigation
e := MyEnum.Opt1
```

---

## Prefixes

Identifier prefixes are special 'keywords' that signify a special meaning to the construct identified.

Identifier prefixes are only used at the definition not for the reference.

To interop with .NET properties the `get_` and `set_` prefixes are used to target property setters and getters.

```csharp
// static property
get_MyProp: (): U8
    return 42

// instance property
get_MyProp: (self: MyStruct): U8
    return self.field1

// call static property
p := MyProp      // no () required?
```

| Prefix | .NET | Description |
|--|--|--|
| `get_` | property get | Implements a property getter. |
| `set_` | property set | Implements a property setter. |
| `opchk_` | - | Z# operator checked implementation (exception). |
| `opuchk_` | - | Z# operator unchecked implementation (wrap). |
| `op_` | - | Z# operator implementation where return type indicates semantics (`Err<T>`/`Opt<T>`) |
| `opsat_` | - | Z# operator implementation that saturates? |

> Perhaps start the prefix with a `_` to indicate that part is hidden?

---

## Aliases

Almost all identifiers can be aliased, given a new name that is resolved at compile time. These aliases themselves are identifiers and follow the same rules.

An Alias is created by assigning it an existing identifier.

```csharp
Fn: (p: U8)
    ...

MyAlias = Fn

MyAlias(42) // calls Fn(42)
```

> TBD

When an alias resolves to nothing (or empty string) it is treated as a weak reference and removed from the code.

This would allow conditional compilation to select different 'implementations' for an alias or even leave it empty when it does not apply.

```csharp
// alias resolves to nothing
MyEmptyAlias = _

// entire call removed because alias is empty
MyEmptyAlias(42)
```

- How do we know what type this alias represents (function, struct etc)?

- Can aliases be exported from a module or are they always local to the module they're declared in?

- do we want an operator for aliases where the symbol-name being aliased is not in scope?
    `=` => both symbol name and alias are in scope. `alias = symbolName`
    `=_` => only alias is in scope. `alias =_ symbolName`
