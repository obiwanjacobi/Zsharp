# Assembly

For .NET compatibility we need a way to indicate what modules will be compiled into one assembly.

The assembly file is a type of language supported project file that defines what modules to include into an assembly and what name that assembly has.

The `assembly` keyword works similar to how the `module` keyword works, it names the assembly.

Here an assembly that includes module1 and module2:

```csharp
assembly Name.OfMy.Assembly
import
    module1
    module2
```

There can be multiple assembly files in one compilation run. This will result in generating multiple assembly files.

An `assembly` file cannot contain any runtime code other than (assembly-level) code attributes or use of compiler directives and compile time code. Everything else is a compiler error.

> We may want to add additional settings as code attributes and/or compile directives to allow customizing the way the assembly is generated.

- External Assembly References
- NuGet Packages
- .NET platform / frameworks
