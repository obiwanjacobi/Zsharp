# Export

By default all code in a file is private to that file. One could think of it as being inside an anonymous namespace.

The `export` pragma publishes the code identified by the name to be used by other code files / modules.

The following types of identifiers can be exported:

- Types
- Functions
- Immutable Variables

> Mutable variables cannot be exported! They need an accessor function.

Here is an example of exporting a function:

```C#
# export MyFunction
// -or-
# export
    MyFunction
    // more functions here...

MyFunction: ()
    ...
```

The `export` pragma can also be applied directly to the code identifier to be made publicly available:

```C#
# export MyFunction: ()
    ...
```

> How we make clear(er) this is not a compile-time function?

---

## Aliasing

By using an alias the name of the exported item can be renamed to something new. It can also be a way to simulate namespaces.

```C#
# module alias_example
# export MyNamespace.MyFunc = MyFunc

MyFunc: ()
    ...
```

```C#
# import alias_example

MyNamespace.MyFunc()
```

> TBD: This can conflict with module names with namespaces. Drop this?

---

> Rename `export` to `pub` for public or publish?

> Rename `import` to `use`?

> Drop the `#`?
