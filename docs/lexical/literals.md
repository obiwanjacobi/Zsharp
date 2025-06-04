# Literals

## Numbers

Literal numeric values can be specified in the code using prefixes as follows.

- 0b - binary
- 0c - Octal
- 0d - Decimal (default, optional)
- 0x - Hexadecimal

Examples of this would be:

```C#
b := 0b01110011      // binary
o := 0c154           // octal
d := 42              // decimal is default
d := 0d42            // decimal (explicit)
h := 0x8F            // hexadecimal
```

The `_` character may be used to separate parts for readability. They have no meaning for the value. It can be used for any of the prefixes, so if used on a decimal, the prefix `0d` has to be present.

```C#
b := 0b1010_01010    // still a 8-bit binary number
d := 0d7_654_321     // 7,654,321
h := 0xFF_FF
```

The data type size of a literal is inferred from it's use.

```csharp
a : U8 = 42
b : I32 = 1024
```

> TBD I am thinking of defaulting to Int64 for all literal numbers.

---

### Floating Point

Floating point literals can be specified in different ways. Mainly the use of the decimal separator `.` is an indication that the value is a floating point value. Only a decimal format is supported (no prefix).

```C#
1.0  1.  .42
```

> Use of exponents (like `1.42e-3`) is currently not defined. We probably will use the syntax that works directly in C#.

---

## Strings

A string literal is enclosed with double quotes. Here are some examples:

```C#
s := ""                          // empty string
s := "Hello World!"
s := "I say 'Hello'"             // use of single quotes is ok
s := "C:\Windows\Path\File.Ext"  // no need to escape '\'
```

To use special characters in a literal string, you have to use the escape sequence: `\`` (backtick).

```C#
s := "Some text `n with newlines `n and `"quotes`"."
```

### Escaped Characters

Char | Note
--|--
`n | NewLine
`r | Carriage Return
`t | Tab
`f | Line Feed
`b | Bell
`` | ` (backtick)
`$ | String format parameter

Multiple `"` charactes can be used to define a literal string that escapes the `"` character.

```csharp
s := """This is a multi-line "string" 
    that allows "embedded" "quotes"
    """
    // indent position of closing quotes determine the leading whitespace to ignore
```

---

### String Concatenation

For longer strings you may want to spread them out over multiple lines.
Using indents the compiler can see that the string is spread out.

> Use one indent extra for spreading out strings.

There are two indents for the extra lines of the spread out string:

```C#
// new line right after the opening quote
s := "
    Some text
    spread over
    multiple lines
    "
```

These indents on the beginning of the new lines do not become part of the resulting string: `"Some text spread over multiple lines"`
The closing `"` indentation (level) determines how the extra white-space inside the string is interpretted.

> Or surround each line of string-part with double quotes?

Adjacent literal string will be concatenated. There is no uncertainty how the preceding whitespace is handled - it is not part of the string.

```C#
s := "Some text"
    "spread over"
    "multiple lines"
```

> TBD: This is string-concatenation without any operator. Do we require the operator `<+`?

---

### String formatting

Basic formatting of dynamic values into a string is done in the following way:

```C#
a := 42
s := "Answer to everything is '$a'"
// Answer to everything is '42'

s := "Answer to everything is '$a:x'"
// Answer to everything is '2a'
```

> This is not compatible with dotnet and requires parsing each literal string to convert it during the emit-phase of the compiler.

> More on formatting floats, dates, etc.

> What character to use to disable string features like formatting? `$`

Using the `$` character as is in a string literal, requires the escape sequence.

```C#
// escape $
s := "How much `$"
// How much $
```

> TBD: Should it be possible for custom types to implement custom formatting? Not sure how to intercept that when compiling to dotnet interpolated string.

---

### Canonical String Constants

Where the name of the constant is the same as the value of the constant.

```csharp
$ConstantAsValue
// ConstantAsValue: Str = "ConstantAsValue"

s := "This is a $ConstantAsValue"
// s = 'This is a ConstantAsValue'
```

```csharp
$ConstantAsValue    // readonly field (run time)
#$ConstantAsValue   // const value (compile time)
```

---

## Character

Related to strings are character literals. A single character can be specified using single quotes:

```C#
c := 'X'                 // C16
```

Multi-byte character literals (max 4 chars)

```csharp
c: U64 := 'ABCD'    // 0x41424344
```

---

## Arrays

This is the syntax for specifying literal arrays of basic types:

```C#
// default to an array
arr := (1, 2, 3, 4, 5)           // array of 5 elements of U8

// unless variable is typed
list: List<U8> = (1, 2, 3, 4, 5)
```

---

## Regex

> TBD: Regex literals `regEx = #"$[0-9]^"` (syntax?) `regEx = /$[0-9]^/`

```csharp
[[Literal("Rx")]]
RegEx: (r: Str): RegEx
    ...

// 'Rx' literal => RegEx
regex := "$[a-z]*"Rx
// regex: RegEx
```

or

```csharp
[[LiteralString("!")]]
RegEx: (r: Str): RegEx
    ...

// "!...!" => RegEx
regex := "!$[a-z]*!"
// regex: RegEx
```

or alias

```csharp
RegEx: (r: Str): RegEx
    ...
Rx = RegEx

regex := Rx("$[a-z]*")
// regex: RegEx
```

Also look into the way Swift handles RegEx.
They seem to be using some sort of object model that is declarative in the code.
https://swiftregex.com/

---

## Structured String

- Literal embedded Xml, Json, Yaml etc.

> `@` indicates a compiler extension function.

```csharp
x := @Xml"""
<?xml version="1.0"?>
<document xmlns="example">
    <line attr="42" />
</document>
"""

j := @Json"""
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
h := @Html"""
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
x := @Wpf"""
// xaml like wpf dsl?        
"""
```

The more this can look like normal code the better.
Is there a way to do this with normal function syntax?

---

> TBD

- Literal syntax for custom data types and units?
- Data Structure literals `{}` and lists `()`.
- literals as objects: `parts = "literal string".split(' ')`
- literals bound to custom types?
