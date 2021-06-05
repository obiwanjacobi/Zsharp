# Compiler Options

> Compiler profile: a configuration of what (language) features are on/off. Separate file. All options can also be supplied on the command line.

Think of it as an `editorconfig` for the compiler.

- safety checks
- user signal values (defines) (allow to be specified in file also)

## Syntax Options

- no auto vars / always typed vars
- explicit operator precedence: use `()`.
- no bool literal comparisons: `if myBoolVar = true` will not be allowed, `if myBoolVar` or `if not myBoolVar` is ok.
- explicit return value assignment. `_ = func()` to ignore retval.
- have each language feature assigned to a category and allow the compiler profile to select which categories are on by default and which ones need an extra switch in the code. The aim is to allow only those features that are considered safe and applicable to the project. Some exceptions can be made by adding special permissions (pragmas?) in the source code.
- choose the default type for number literals (I/U8-64)
- choose the default type for real number literals (F32-96)
