# Compile Time Text Generation

Allow 'macros'? to generate text at compile time.

This text can be code or data or any other text.

You could even generate binary data.

- The target would be a string or a file.
- Multiple strings or files can be generated.
- Multiple strings or files can be consumed.
- Comparison if generated (file) content has changed.
- Extend generated code with custom file content (partial).

Use a special (compile-time) syntax to output strings? (think razor engine and T4 and the like)?

See also [Generator Functions](../lang/functions.md#Generator-Functions).

---

Embed any language inside the Z# code with some kind of marker that indicates the interpreter/compiler/source generator.

```csharp
[[CSharpCodeGenerator]]
#! SomeName     // unique name passed to the generator
"""             // what code block delimiters to use?
    var i = 0;
    for (int i; i < 42; i++)
        ...
"""
```

Can we do this inline?
