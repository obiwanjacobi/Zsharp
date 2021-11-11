# Module

The `module` pragma attaches the code in the file to a named (logical) unit. This name is also used with [`import`](import.md).

Multiple code files can be part of the same module.

```C#
# module math
```

Note the use of the `#` pragma syntax, for the module functionality is a compiler feature that is performed at compile-time.

The name may contain a few special characters: `.` and `_` .
This is a valid module name:

```C#
# module myproject.Custom_String1
```

Module names are [Identifiers](../lexical/identifiers.md) too.

In a sense, the module name is also the namespace of the code in the file.

> Packaging libraries is not part of the language but a function of the compiler. It uses modules to group code together. The module identifies all the code -not just the exported public identifiers.

---

> Should the module statements `module` and `import` only be used at the top of the file, or can they appear anywhere?

> `.NET`: Circular Assembly references are not supported. Multiple modules can go into one Assembly.

---

> Should there be a way to mark a module as open/closed for extension?

Because a module can be made up of multiple files, can any new file declare itself part of any module and therefor gain access to it's 'internals'?
Or just within the same compilation run?

```csharp
// explicitly declare the module as open
# module this_is_my_module, open
# extension this_is_my_module

// separate user file
# module this_is_my_module    // ok, module is open
// add to 'this_is_my_module' module
```

> What about file-scope state?

```csharp
// file-scope
# module this_is_my_module
globalVar: U8 = 42  // accessible by all files in module?
```

---

> TBD

- private modules?
- explicit namespace handling?

---

> TBD: Services

Module as a boundary for implementing 'services'?

A service would be an 'object' that communicates via messaging - the original OO concept.

Services as single threaded and concurrent. No additional threads allowed inside a service.

Exported functions are the public API for the service and the function parameters are the fields inside the request-message it receives.
The return value (which can be a tuple) is the response message the client receives.

Async/await is a separate concern and is mainly used as a .NET interop feature.

A runtime framework is needed to host these services. Something like ASPNET core would do nicely. I don't think HTTP would be a good fit. Something like gRPC would do better perhaps?

Maybe have a service be the conventional remote API and a component be the in-memory implementation for it? That way a component can be wrapped in different types of Service endpoints (REST/gRPC/JSON/XML)...?
> Actor Model / Active Object Pattern? An isolated component with an Event/Message Queue and a private (pooled?) processing thread... Preferably stateless? No (non-constant) data sharing across threads, ever.

We may want to add a proxy to immutable objects to share the proxy (identity) and not the object itself (Clojure does this) - so that changes to the immutable object -becoming a new instance- can be pointed to by the proxy?

> TBD

The principle of declaring a file to be part of a module can be extended to OO classes/records as well.
A future extension could introduce the `class` or `record` keyword to interpret the file as a class implementation.
Although currently the idea is selectively generate C# records based on the presents of self-bound functions.
