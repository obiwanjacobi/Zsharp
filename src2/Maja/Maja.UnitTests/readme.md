# Tested

Keeping track of the progress of Maja features.

**Columns**

- Feature: the language feature (syntax)
- Grammar: does it parse and show up in the parse tree
- Syntax: is there an Syntax node to represent the feature
- Ir: is there an Ir node to represent it
- Checks: are type (and other) checks in place
- Lower: is the Ir lowered
- Emit: is code emitted to represent the feature
- Runtime: does the feature work at runtime
- Description: example, description, notes

**Values**

- (blank) = TODO
- \- = not applicable
- x = supported
- ? = unknown

---

|Feature      |   |Grammar|Syntax|Ir|Checks|lower|Emit|Runtime|Description|
|-------------|--------------|:-:|:-:|:-:|:-:|:-:|:-:|---|---------------|
| comments    | `#_` | x | x | - | - | - | - | - | `#_ regular comment`  |
|             | `##` | x | x | - | - | - | - | - | `## comment warning`  |

## Module

|Feature      |   |Grammar|Syntax|Ir|Checks|Lower|Repl|Emit|Runtime|Description|
|-------------|-----|:-:|:-:|:-:|:-:|:-:|:-:|:-:|---|---------------|
| mod    |          | x | x | x |   |   |   | x |   | `mod myModule`  |
| pub    |          | x | x | x |   |   |   | x |   | `pub mySymbol`  |
|        | inline   |   |   |   |   |   |   | x |   | `pub myFn(): U8`  |
|        | type     |   |   |   |   |   |   | x |   | `pub myType`  |
| use    | assembly | x | x | x |   |   |   |   |   | `use assembly.class`  |
|        | module   | x | x |   |   |   |   |   |   | `use module`  |

## Function

|Feature      |   |Grammar|Syntax|Ir|Checks|Lower|Repl|Emit|Runtime|Description|
|-------------|-----|:-:|:-:|:-:|:-:|:-:|:-:|:-:|---|---------------|
| decl       |       | x | x | x |   |   | x | x |   | `fn: ()`  |
|            | param | x | x | x |   |   | x | x |   | `fn: (p: U8)`  |
|            | type  | x | x | x |   |   |   | x |   | `fn: <T>(p: T)`  |
|            | ret   | x | x | x |   |   | x | x |   | `fn: (): U8`  |
| generics   |       |   |   |   |   |   |   |   |   | `fn: <T>()`  |
| template   |       |   |   |   |   |   |   |   |   | `fn: <#T>()`  |
| invocation |       | x | x | x |   |   | x |   |   | `fn()`  |
|         | generics | x | x | x |   |   |   | x |   | `fn<T>()`  |

## Variable

|Feature      |   |Grammar|Syntax|Ir|Checks|Lower|Repl|Emit|Runtime|Description|
|-------------|-----|:-:|:-:|:-:|:-:|:-:|:-:|:-:|---|---------------|
| decl       |   | x | x | x |   |   | x | x |   | `var: U8`  |
| init       |   | x | x | x |   |   | x | x |   | `var: U8 = 42`  |
| infer      |   | x | x | x |   |   | x | x |   | `var := 42`  |

## Type

|Feature      |   |Grammar|Syntax|Ir|Checks|Lower|Repl|Emit|Runtime|Description|
|-------------|-----|:-:|:-:|:-:|:-:|:-:|:-:|:-:|---|---------------|
| struct      |      | x | x | x |   |   | x | x |   | `MyType -> fld: U8`  |
|             | enum | x | x |   |   |   |   |   |   | `MyType -> fld: U8 -> Opt1, Opt2`  |
| generics    |      | x |   |   |   |   |   |   |   | `MyType<T> -> fld: T`  |
| template    |      | x | x |   |   |   |   |   |   | `MyType<#T> -> fld: T`  |
| enum        |      | x | x | x |   |   |   | x |   | `MyType -> opt1, opt2`  |
| custom      |      |   |   |   |   |   |   |   |   | `MyType: U8`  |
| rule        |      |   |   |   |   |   |   |   |   | `MyType -> #fld1 > 0`  |
| struct      | init | x | x | x |   |   | x | x |   | `MyType -> fld = 42`  |

## Expressions

|Feature      |   |Grammar|Syntax|Ir|Checks|Lower|Repl|Emit|Runtime|Description|
|-------------|-----|:-:|:-:|:-:|:-:|:-:|:-:|:-:|---|---------------|
| literal     |   | x | x | x |   |   | x | x |   | `42`, `'hello'` |
| const fold  |   | - | - | x |   |   | x |   |   | `42 + 101` |
| precedence  |   | x | x | x |   |   | x |   |   | `(42 + 101) / 2` |
| member access | variable | x | x | x |   |   | x | x |   | `y.fld` |
|             | invocation | x | x | x |   |   | x | x |   | `fn().fld` |

## Statements

|Feature      |   |Grammar|Syntax|Ir|Checks|Lower|Repl|Emit|Runtime|Description|
|-------------|-----|:-:|:-:|:-:|:-:|:-:|:-:|:-:|---|---------------|
| assignment |      | x | x |   |   |   | x |   |   | `x = 42`  |
| if      |      | x | x | x |   |   | x |   |   | `if <condition>`  |
|         | else | x | x | x |   |   | x |   |   | `if <condition> else`  |
|         | elif | x | x | x |   |   | x |   |   | `if <condition> elif <condition>`  |
