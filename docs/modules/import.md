# Import

When the code in the file depends on code located in a different module, the `import` directive can be used to declare the location of that dependency.

This import example indicates that the code uses one or more functions from the standard math (library) module.

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

> This should be a compiler option.

---

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

The difference between `import` aliasing and other types of aliasing is that the original names are not brought into scope with `import` aliasing. With 'normal' aliasing both the original identifier as well as the alias are in scope.

When module names look the same:

```C#
// module #1
# module MyModule
# export MyFunc

// module #2
# module MyModule.MyFunc

// module #3
# module UsingModule
# import
    // imports module #2
    MyModule.MyFunc
    // imports module #1 - MyFunc
    Alias = MyModule.MyFunc
```

---

> TBD: A way to import modules into a local scope?

```csharp
fn: ()
    use mod1        // load at runtime?
    # import mod1   // load at compile-time?
    s = Struct1     // from mod1
        ...
```

The advantage of specifying `import` in a local scope is that the code unit (type, function etc.) can be moved (refactoring) to a different file without the dependencies needing to be reviewed and file-level imports to be added.

Does loading at runtime (`use`) require the containing function to have an `Err<T>` return type? Will the program be aborted when the module is not found or can it be handled gracefully? (yes)

```csharp
fn: ()
    use mod1 catch err
        // handle loading error
```

Does `use` return a type that represents the loaded module? Something like `Err<Ptr<ModuleInfo>>`?

---

Be explicit about what assembly to use to import the name/namespace? (for .NET compatibility)

```csharp
# import
    System.Core             // <= assembly -or-
    System.Core.dll         // <= assembly
    System.IO.*             // <= namespace
    System.DBNull           // <= Type
```

> TBD: this would require a fallback when resolving a type and if not found, trying to locate the identifier as an assembly.

Or more C# like - always specify .NET namespaces (`.*`) or specific type / Z# modules?

```csharp
# import System.IO          // <= namespace
```

> TBD: this does not fit well with Z#'s `module` paradigm.

---

> TBD

Allow import from a remote location?

```csharp
#import https://user.github.com/master/package1
#import https://user.github.com/master/package1@v1.2.3

#import alias = https://user.github.com/master/package2
```

What format should the file be that is being imported? Is it an assembly file or something else. Should these 'modules' or 'packages' be hosted on a specific package manager's site?

> remote packages/modules are downloaded at compile time. They are not resolved at runtime.

Use some sort of (plugable) loader -or unpacker- during compile-time?

```csharp
[[zip-loader]]
#import https://user.github.com/master/package1
```

> TBD: import/include based on online source code?

```csharp
#include https://user.github.com/master/src
```

> TBD: import/include source files from the file system?

```csharp
#include $/utils/fileHeader.txt // $ => project root
#import subdir/folder/file      // no .z extension necessary
```

---

> Rename `export` to `pub` for public or publish?

> Rename `import` to `use`?

> Drop the `#`?

---

> TBD

Internal modules are always available to other internal modules.
This means that an `import` is never needed for an internal module.
That means that the `import` name always references an external module, which has to be .NET compatible. A Z# compiled assembly is -at first glance- indistinguishable from any other .NET assembly.

The name of the module is a namespace for the code of that module. This will be compiled to a .net static class (if no file-scoped variables are defined).

So how to determine the scope of the `import` name. Should it always end with a .NET type (a Z# module)?
