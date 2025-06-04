# Use

When the code in the file depends on code located in a different module, the `use` directive can be used to declare the location of that dependency.

This use example indicates that the code uses one or more functions from the standard math (library) module.

```C#
#use std.math
```

Only one name can be specified at a time. Using multiple dependencies requires multiple statements or using a scope.

```C#
#use std.io
#use std.math
// -or-
#use
    std.io
    std.math
```

Using a module that does not exist is not an error (perhaps only a warning) as long as no types from that missing module are used. This will make dealing with dependencies of conditional compiled code easier.

> This should be a compiler option.

---

## Aliasing

By using an alias the name of the used item can be renamed to something new. This can help to resolve conflicts or make long names shorter.

```C#
#module alias_example
#pub MyFunc

MyFunc: ()
    ...
```

Using the dot-notation the items inside the module can be accessed.

```C#
#use NewName = alias_example.MyFunc

NewName()       // calls MyFunc
```

The difference between `use` aliasing and other types of aliasing is that the original names are not brought into scope with `use` aliasing. With 'normal' aliasing both the original identifier as well as the alias are in scope.

When module names look the same:

```C#
// module #1
#module MyModule
#pub MyFunc

// module #2
#module MyModule.MyFunc

// module #3
#module UsingModule
#use
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
    #use mod1   // load at compile-time?
    s = Struct1     // from mod1
        ...
```

The advantage of specifying `use` in a local scope is that the code unit (type, function etc.) can be moved (refactoring) to a different file without the dependencies needing to be reviewed and file-level usings to be added.

Does loading at runtime (`use`) require the containing function to have an `Err<T>` return type? Will the program be aborted when the module is not found or can it be handled gracefully? (yes)

```csharp
fn: ()
    use mod1 catch err
        // handle loading error
```

Does `use` return a type that represents the loaded module? Something like `Err<Ptr<ModuleInfo>>`?

---

Be explicit about what assembly to pick to use the name/namespace? (for .NET compatibility)

```csharp
#use
    System.Core             // <= assembly -or-
    System.Core.dll         // <= assembly
    System.IO.*             // <= namespace
    System.DBNull           // <= Type
```

> TBD: this would require a fallback when resolving a type and if not found, trying to locate the identifier as an assembly.

---

Or more C# like - always specify .NET namespaces (`.*`) or specific type / Z# modules?

```csharp
#use System.IO          // <= namespace
#use System.Console     // <= type
```

This requires a secondary lookup of the external type if the first lookup as a namespace fails.

---

> Group related type usings from the same namespace?

```csharp
#use System
    .Console
    .String
    .Exception
```

---

> TBD

Allow using from a remote location?

```csharp
#use https://user.github.com/master/package1
#use https://user.github.com/master/package1@v1.2.3

#use alias = https://user.github.com/master/package2
```

What format should the file be that is being imported? Is it an assembly file or something else. Should these 'modules' or 'packages' be hosted on a specific package manager's site?

> Remote packages/modules are downloaded at compile time. They are not resolved at runtime.

Use some sort of (plugable) loader -or unpacker- during compile-time?

```csharp
[[zip-loader]]
#use https://user.github.com/master/package1
```

> TBD: include based on online source code?

```csharp
#include https://user.github.com/master/src
```

> TBD: import/include source files from the file system?

```csharp
#include $/utils/fileHeader.txt // $ => project root
#use subdir/folder/file      // no .z extension necessary
```

> What if an authentication header is required? Where to store the secret?

---

> TBD

Are Internal modules always available to other internal modules?
This means that an `use` is never needed for an internal module.
That means that the `use` name always references an external module, which has to be .NET compatible.
A Z# compiled assembly is -at first glance- indistinguishable from any other .NET assembly.

The name of the module is a namespace for the code of that module. This will be compiled to a .net static class (if no file-scoped variables are defined).

So how to determine the scope of the `use` name. Should it always end with a .NET type (a Z# module)?
