# Module

The `module` directive attaches the code in the file to a named (logical) unit.
This name is also used with [`use`](use.md).

Multiple code files can be part of the same module within the same assembly.

```C#
#module math
```

Note the use of the `#` pragma syntax, for the module functionality is a compiler feature that is performed at compile-time.

The name may contain a few special characters: `.` and `_` .
This is a valid module name:

```C#
#module myproject.Custom_String1
```

Module names are [Identifiers](../lexical/identifiers.md) too.

In a sense, the module name is also the namespace of the code in the file.

A module file can contain type, variable and function declarations as well as (top-level) statements.
The statements will be run in order (top to bottom) when the module is initialized.

> Packaging libraries is not part of the language but a function of the compiler. It uses modules to group code together. The module identifies all the code -not just the public identifiers.

> TBD: Allow multiple modules to be defined in the same file.

```csharp
#module Module1
    // code for Module1 goes here (indented)

#module Module2
    // code for Module2 goes here (indented)
```

The only difference between global and scope `module` directive is that the next line either is indented or not. That may be a bit tricky.

---

> Should the module statements `module` and `use` only be used at the top of the file, or can they appear anywhere?

> `.NET`: Circular Assembly references are not supported. Multiple modules can go into one Assembly.

---

> Should there be a way to mark a module as open/closed for extension?
 No. Only multiple files within the same assembly (compilation).

> What about file-scope state?

```csharp
// file-scope
#module this_is_my_module
globalVar: U8 = 42  // accessible by all files in module?
```

Yes. module-static state is accessible by all module files.

---

> TBD

- private modules?
- explicit namespace handling?

---

> TBD: Services

Module as a boundary for implementing 'services'?

A service would be an 'object' that communicates via messaging - the original OO concept.

Services as single threaded and concurrent. No additional threads allowed inside a service.

Public functions are the public API for the service and the function parameters are the fields inside the request-message it receives.
The return value (which can be a tuple) is the response message the client receives.

Async/await is a separate concern and is mainly used as a .NET interop feature.

A runtime framework is needed to host these services.
I don't think HTTP would be a good fit. Something like gRPC would do better perhaps? Or configurable?

Maybe have a service be the conventional remote API and a component be the in-memory implementation for it? That way a component can be wrapped in different types of Service endpoints (REST/gRPC/JSON/XML)...?

> Actor Model / Active Object Pattern? An isolated component with an Event/Message Queue and a private (pooled?) processing thread... Preferably stateless? No (non-constant) data sharing across threads, ever.

We may want to add a proxy to immutable objects to share the proxy (identity) and not the object itself (Clojure does this) - so that changes to the immutable object -becoming a new instance- can be pointed to by the proxy?

---

> TBD

The principle of declaring a file to be part of a module can be extended to OO classes/records as well.
A future extension could introduce the `class` or `record` keyword to interpret the file as a class implementation.
Although currently the idea is selectively generate C# records based on the presence of self-bound functions.

- `module`: a namespace for 'flat' code
- `class`/`record`: .NET class or record
- `struct`: how to differentiate between class and struct?
- `service`: rpc-style distributed (stateless) 'module'
- `actor`: message based stateful object
- `workflow`: stateful orchestration
- `unittest`: a unit test file that is compiled into a separate assembly and can be run in unit test runners.

> TBD: class,struct,record => how to define these characteristics/properties in a more generic way?

| Attribute | class | struct | record-class | record-struct |
| -- | -- | -- | -- | -- |
| Immutable | o | `readonly` | o | `readonly` |
| Mutable | x | x | x | x |
| Value ref | - | x | - | x |
| Reference ref | x | - | x | - |
| Value equal | o |  x | x | x |
| Reference equal | x |  - | x | - |
| Stack allocated | - | x (`ref`) | - | x |
| Heap Allocated | x | - (box) | x | - (box) |
| Inheritence | x | - | x | x |

`x` => available by default
`o` => can be generated
`-` => not available
