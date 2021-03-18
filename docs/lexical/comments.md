# Comments

There are only line comments. Block comments like `/* block comment */` as seen in other languages like C, C++, Java and C#, are not supported.

This choice eliminates the problem of nested block comments and keeps parsing simple and fast.

_A comment starts when `//` is encountered and continues for the rest of the line until a new-line character is seen._

A comment starting at the beginning of the line:

```C#
// this entire line is considered a comment
```

A comment can start anywhere on the line:

```C#
              // this is a comment
```

Multiple comments, one on each line:

```C#
// this is a comment that tells
// a long story.
```

A comment can be placed after code:

```C#
some code here // the rest of this line is a comment
```

A comment can contain any characters, including more of these `/` :

```C#
////////-------*******|||||________\\\\\\\\
```

A comment can be used to (temporarily?) disable code (not recommended):

```C#
// some code was here
```

---

## Documentation Comments

```C#
/// Three is magic
```

> doxygen like @param and @return docs etc?

---

## Temporary Comments

Temporary comment that will give a compiler warning. Useful for when testing out code and don't want to forget leaving it uncommented when submitting.

```csharp
## compiler warns me about this comment
```
