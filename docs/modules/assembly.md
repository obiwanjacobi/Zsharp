# Assembly

For .NET compatibility we need a way to indicate what modules will be compiled into one assembly.

The assembly file is a type of language supported project file that defines what modules to include into an assembly and what name that assembly has that is being compiled.

The `assembly` keyword works similar to how the `module` keyword works, it names the assembly. It must be the first instruction in the file - only comments or whitespace can precede it.

An `assembly` file cannot contain any runtime code other than (assembly-level) code attributes or use of compiler directives and compile time code. Everything else is a compiler error.

```csharp
// comments in front are allowed
# assembly Name.OfMy.Assembly
```

There can be multiple assembly files in one compilation run. This will result in generating multiple assembly files.

---

## Include

Here an example of an assembly that includes module1 and module2 that are compiled in this project (not external):

```csharp
# assembly Name.OfMy.Assembly

// included in the compiled assembly
# include
    implModule1
    implModule2
```

The `include` keyword includes the compiled source code module into the specified assembly.

> TBD: include by file names?

```csharp
# assembly Name.OfMy.Assembly

// included in the compiled assembly
# include
    file1.zs
    file2.zs
```

> TBD: Include all by default and Exclude files specifically?

---

## Global Import

Modules globally imported in an assembly file are available throughout the project code files without explicit reference.

```csharp
# assembly Name.OfMy.Assembly

// global imports
# import
    externalModule1
    externalModule2

# include
    ...
```

---

## Project Attributes

Project dependencies can be listed in the assembly file as well.

- .NET platform / frameworks
- External Assembly References
- NuGet Packages
- Project References (TBD)

```csharp
// TBD: project reference?
#ref MyProject/assembly.zs

// assembly reference (pass probe paths to compiler)
#ref System.Core.dll    // or .exe

// nuget package (with version, no file extension)
#ref System.Console@>=5.0.*

// .NET framework / SDK reference
#sdk Net5.0@CoreAppSDK
```

> Does this work on Linux?

---

## Assembly Attributes

Standard .NET Assembly code Attributes can be applied to the assembly.

The `Attribute` postfix in the class name can be omitted just as in C#.

```csharp
# assembly Name.OfMy.Assembly

// decorator syntax
{AssemblyFileVersion("1.0.0.0")}
{AssemblyProduct("ProductName")}

// do we have project settings available as pragmas?
{AssemblyConfiguration(#project.configuration)}
```

Strong-naming an Assembly can be done with assembly attributes.

---

## Compile-time Code

An Assembly file can also contain compile time code and other `#` directives.

```csharp
# assembly Name.OfMy.Assembly

// a compile-time function
#! calculateVersion(): Str
    ...

// calling the function to yield the version string
{AssemblyVersion(#calculateVersion()}
```

---

## Assembly Entry Point

If the assembly is an executable the entry point of the program is listed in the assembly file.

```csharp
# assembly Name.OfMy.Assembly
# execute MyEntryPointFn
```

If `#execute` is not specified the assembly is a (dynamic link) library.

The function specified must be present in one of the included modules.

The declaration of the entry point function must be one of:

```csharp
main: ()
main: (): I32
main: (args: IEnumerable<CommandLineArgument>)
main: (args: IEnumerable<CommandLineArgument>): I32
```

> Research if we can use the new System.CommandLine library that allows any number of typed parameters on the 'main' function that get automatically parsed from the command line.

---

> TBD

Clearly this is nowhere near the information for a full project management and build system. Do we try to force everything we need into an assembly file, or do we introduce another (project) file?

- a Z# version specifier with what version of the language the assembly is compatible.

---

What about embedded resources? How to tell the project to embed a resource.

```csharp
# assembly Name.OfMy.Assembly

// files included in the compiled assembly as resources
# include
    image1.jpg
    assembly.dll
```

No control over naming (namespace and resource name)!
Have a compile-time function for this? `#embedFile("image1.jpg", "namespace.resourceName")` or have an `#embed` pragma?

How about string/int resources?

```csharp
// $ auto resource string
resourceText = $"Text loaded from resource"

// performs a load resource (with local var)
print(resourceText)
```

How to reuse an existing resource then?
What is the scope of the name? Module level? no - too narrow.

> Can we built-in localization on resources?
