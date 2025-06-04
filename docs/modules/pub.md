# Pub

By default all code in a file is private to that file. One could think of it as being inside an anonymous namespace.

The `pub` pragma publishes the code identified by the name to be used by other code files / modules.

The following types of identifiers can be public:

- Types
- Functions
- Immutable Variables

> Mutable variables cannot be public! They need an accessor function.

Here is an example of making a function public:

```C#
#pub MyFunction
// -or-
#pub
    MyFunction
    // more functions here...

MyFunction: ()
    ...
```

The `pub` pragma can also be applied directly to the code identifier to be made publicly available:

```C#
#pub MyFunction: ()
    ...
```

> How we make clear(er) this is not a compile-time function?

---

## Aliasing

By using an alias the name of the public item can be renamed to something new. It can also be a way to simulate namespaces.

```C#
#module alias_example
#pub MyNamespace.MyFunc = MyFunc

MyFunc: ()
    ...
```

```C#
#use alias_example

MyNamespace.MyFunc()
```

> TBD: This can conflict with module names with namespaces. Drop this?
