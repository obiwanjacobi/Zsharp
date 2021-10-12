# Literals

## Numbers

Literal numeric values can be specified in the code using prefixes as follows.

- 0b - binary
- 0c - Octal
- 0d - Decimal (default, optional)
- 0x - Hexadecimal

Examples of this would be:

```C#
b = 0b01110011      // binary
o = 0c154           // octal
d = 42              // decimal is default
d = 0d42            // decimal (explicit)
h = 0x8F            // hexadecimal
```

The `_` character may be used to separate parts for readability. They have no meaning for the value. It can be used for any of the prefixes, so if used on a decimal, the prefix `0d` has to be present.

```C#
b = 0b1010_01010    // still a 8-bit binary number
d = 0d7_654_321     // 7,654,321
h = 0xFF_FF
```

> TBD I am thinking of defaulting to Int64 for all literal numbers.

> Add postfixes to indicate type? `a = 42u` (unsigned)

---

### Floating Point

Floating point literals can be specified in different ways. Mainly the use of the decimal separator `.` is an indication that the value is a floating point value. Only a decimal format is supported (no prefix).

```C#
1.0  1.  .42
```

> Use of exponents (like `1.42e-3`) is currently not defined.

---

## Strings

A string literal is enclosed with double quotes. Here are some examples:

```C#
s = ""                          // empty string
s = "Hello World!"
s = "I say 'Hello'"             // use of single quotes is ok
s = "C:\Windows\Path\File.Ext"  // no need to escape '\'
```

To use special characters in a literal string, you have to use the escape sequence: ` (backtick).

```C#
s = "Some text `n with newlines `n and `"quotes`"."
```

Escaped Characters

Char | Note
--|--
`n | NewLine
`r | Carriage Return
`t | Tab
`f | Line Feed
`b | Bell

More?

---

For longer string you may want to spread them out over multiple lines. Using indents the compiler can see that the string is spread out.

> Use one indent extra for spreading out strings.

There are two indents for the extra lines of the spread out string:

```C#
s = "Some text
        spread over
        multiple lines"
```

These indents on the beginning of the new lines do not become part of the resulting string: `"Some text spread over multiple lines"`

> Or surround each line of string-part with double quotes?

Adjacent literal string will be concatenated. Explicit how whitespace is handled.

What indentation to use? At least 1 max 2 from root?

```C#
s = "Some text"
    "spread over"
        "multiple lines"    // mix indentations?
```

---

Basic formatting of dynamic values into a string is done in the following way:

```C#
v = 42
s = "Answer to everything is '{v}'"
// Answer to everything is '42'

// hex (lower case) formatting
s = "Answer to everything is '{v:x}'"
// Answer to everything is '2a'
```

Alternate syntax?

```C#
v = 42
s = "Answer to everything is '$v'"
s = "Answer to everything is '$v:x'"
```

> More on formatting floats, dates, etc.

> What character to use to disable string features like formatting? `@`

Using the `{}` characters as is in a string literal, requires the escape sequence `.

```C#
// escape braces
s = @"This will print {braces}"
// This will print {braces}

s = "This will print `{braces`} too"
// This will print {braces} too
```

It should be possible for custom types to implement custom formatting. Most likely the `"{}"` syntax will compile to format calls bound to specific types. The `format` function will be passed all the info it needs to know - like formatting parameters.

```C#
MyStruct
    ...

format(self: MyStruct, ctx: FormatContext): Str
    return custom_formating_impl
```

---

## Character

Related to strings are character literals. A single character can be specified using single quotes:

```C#
c = 'X'                 // C16
```

---

## Arrays

There is a syntax for specifying literal arrays of basic types:

```C#
arr = (1, 2, 3, 4, 5)           // 5 elements of U8
```

---

## Immutable (Constant) Literal

Syntax for inferring literals to an `Imm<T>` type. There is an operator for that: `^`.

```csharp
a = 42              // U8
a =^ 42             // Imm<U8>
s =^ "Constant"     // Imm<Str>
```

---

> TBD

- Regex literals `regEx = #"$[0-9]^"` (syntax?) `regEx = /$[0-9]^/`
- Literal syntax for custom data types and units?
- Data Structure literals `{}` and lists `()`.
- literals as objects: `parts = "literal string".split(' ')`
- literals bound to custom types?

```csharp
[[Literal("Rx")]]
RegEx: (r: Str): RegEx
    ...

// 'Rx' literal => RegEx
regex = "$[a-z]*"Rx
// regex: RegEx
```

or

```csharp
[[LiteralString("!")]]
RegEx: (r: Str): RegEx
    ...

// "!...!" => RegEx
regex = "!$[a-z]*!"
// regex: RegEx
```

or alias

```csharp
RegEx: (r: Str): RegEx
    ...
Rx = RegEx

regex = Rx("$[a-z]*")
// regex: RegEx
```

---

> TBD

- Literal embedded Xml, Json, Yaml etc.

```csharp
"""@Xml
<?xml version="1.0"?>
<document xmlns="example">
    <line attr="42" />
</document>
"""

"""@Json
{
    "property": "value",
    "array": [
        "item1", "item2"
    ]
}
"""
```

The `@Xml` and `@Json` refer to the object (type) name that handles the content inside the `"""` block.

Assign to vars and work with implicit object model (XDocument/JsonObject)?

Most useful when allowing to inject programmatic constructs like loops and value (interpolated strings). See Generator Functions.

This idea can be extended to created embedded (declarative) DSLs.

```csharp
"""@Html
html
    header
        style
    body
        h1: 'Page Title'
        p:
            Loris ipsum
"""
```

```csharp
"""@Wpf
// xaml like wpf dsl?        
"""
```

The more this can look like normal code the better.
Is there a way to do this with normal function syntax?
