# Identifiers

Identifiers name program entities such as variable, functions and types. These names must follow these rules:

- Cannot be empty.
- Cannot contain spaces.
- Cannot contain 'special' characters (;,.:<>{}() etc).
- Cannot contain operator characters (+-/*) (*TBD).
- Cannot start with a digit (*TBD).
- Can contain `_`.
- Can contain alpha-numeric characters.
- Valid characters lie in the ASCII range (no unicode).

Here are examples of valid identifiers:

```C#
My_Function1
someVariable
_hidden
```

Here are examples of _invalid_ identifiers:

```C#
1Function
some variable
my-type
```

> TBD: I don't see any objection to add '`-`' (minus sign) as a valid character for an identifier...? Or start an identifier with a digit? The parser should be able to match that correctly.

## Case Sensitivity

- Type identifiers start with an upper case letter.
- Local Variables and Function Parameters must have a lower case first letter.
- Function names can use either.

Identifiers are the same when:

- The first letter matches exactly (case sensitive)
- All other letters match (case insensitive)
- `_` are ignored (removed) when comparing.

### Discard

Using a discard `_` in an identifier is ignored during matching.

```csharp
My_Function(p: U8)
    ...

Myfunction(42)  // calls My_Function

//----

Myfunction(p: U8)
    ...

My_Function(42)  // calls Myfunction
```

When an identifier only consists of a discard `_` it indicates it is not used.

```csharp
myFn(_: U8): U8    // param not used
    ...
_ = myFn(42)        // return value not used

```

If an identifier starts with a `_` it is hidden from immediate public access. This is mostly a function of the IDE.

```csharp
MyStruct
    _id: U32
    Name: Str

s: MyStruct
s.[intellisense does not show _id]
```

> Do we want to give meaning to identifiers ending with a `_`? Could we use this for weak-functions (or weak-anything)?

## Fully Qualified Names

```C#
MyModule.v2.MyFunction
```

> Do we want to distinguish between namespace separators and obj.fn() calls?

```C#
// namespace / module name (also for import, export aliases)
MyModule::v2::MyFunction

// function call
obj.MyFunction(42)
```

Only needed when function call may include namespace/module parts...
