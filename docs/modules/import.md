# Import

When the code in the file depends on code located in a different module, the `import` pragma can be used to declare the location of that dependency.

This import example indicates that the code uses one or more functions from the standard math library.

```C#
# import std.math
```

Only one name can be specified at a time. Importing multiple dependencies requires multiple statements or using a scope.

```C#
# import std.io
# import std.math
// -or-
# import
    std.io
    std.math
```

Importing a module that does not exist is not an error (perhaps only a warning) as long as no types from that missing module are used. This will make dealing with dependencies of conditional compiled code easier.

## Aliasing

By using an alias the name of the imported item can be renamed to something new. This can help to resolve conflicts or make long names shorter.

```C#
# module alias_example
# export MyFunc

MyFunc: ()
    ...
```

Using the dot-notation the items inside the module can be accessed.

```C#
# import NewName = alias_example.MyFunc

NewName()       // calls MyFunc
```

When module names look the same:

```C#
# module MyModule            // module #1
# export MyFunc

# module MyModule.MyFunc     // module #2

# module UsingModule         // module #3
# import
    MyModule.MyFunc          // imports module #2
    Alias = MyModule.MyFunc  // imports module #1 - MyFunc
```

> TBD: how does the import find the module?
