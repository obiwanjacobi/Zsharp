# Comments

There are only line comments. Block comments like `/* block comment */` as seen in other languages like C, C++, Java and C#, are not supported.

This choice eliminates the problem of nested block comments and keeps parsing simple and fast.

_A comment starts when `#_` is encountered and continues for the rest of the line until a new-line character is seen._

`#` is a compiler directive and `_` means ignore or discard. So it tells the compiler to ignore the rest of the line.

```C#
#_ this entire line is considered a comment
              #_ this is a comment
```

Multiple comments, one on each line:

```C#
#_ this is a comment that tells
#_ a long story.
```

A comment can be placed after code:

```C#
some code here #_ the rest of this line is a comment
```

A comment can contain any characters, including more of these `#` or `_`:

```C#
#_123ABC-=!~@#$%^&_*()
```

A comment can be used to (temporarily?) disable code (not recommended):

```C#
#_ some code was here
```

---

## Documentation Comments

> TBD: some sort of 'doc-language'.
> doxygen like @param and @return docs etc?

```C#
#_ .summary: This is a summary.
```

---

## Temporary Comments

Temporary comment that will give a compiler warning. Useful for when testing out code and don't want to forget leaving it uncommented when submitting.

```csharp
## TODO: compiler warns me about this comment
```
