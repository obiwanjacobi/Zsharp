module Tests.Parser

open System
open Xunit
open FsUnit.Xunit

open FParsec
open ZSharp.Parser

let toSuccess reply =
    match reply with
    | Success (r, _, _) -> r
    | Failure (err, _, _) -> failwith err

//
// Whitespace
//

[<Fact>]
let ``Empty`` () =
    let code = @""
    let parseResult = parseFile code None
    toSuccess parseResult |> ignore

//[<Fact>]
//let ``Blank Lines`` () =
//    let code = @"

//"
//    let parseResult = parseFile code None
//    toSuccess parseResult |> ignore
    

//[<Fact>]
//let ``Blank Lines and Spaces`` () =
//    let code = @"
    
//"
//    let parseResult = parseFile code None
//    toSuccess parseResult |> ignore

//
// Comment
//

[<Fact>]
let ``Empty Comment`` () =
    let code = @"__"
    let parseResult = parseFile code None
    toSuccess parseResult |> ignore


[<Fact>]
let ``Comment`` () =
    let code = @"__ comment"
    let parseResult = parseFile code None
    toSuccess parseResult |> ignore

[<Fact>]
let ``Comment Lines`` () =
    let code = @"__comment1
    __comment2
"
    let parseResult = parseFile code None
    toSuccess parseResult |> ignore

//[<Fact>]
//let ``Comments and Blank Lines`` () =
//    let code = @"__comment1

//    __comment2
//"
//    let parseResult = parseFile code None
//    toSuccess parseResult |> ignore


//
// Expression
//

[<Fact>]
let ``Expression Equals`` () =
    let code = "x = 42"
    let parseResult = runParser expression code None
    toSuccess parseResult |> ignore

[<Fact>]
let ``Expression Greater`` () =
    let code = "x > 42"
    let parseResult = runParser expression code None
    toSuccess parseResult |> ignore

[<Fact>]
let ``Expression Multiple`` () =
    let code = "x + 101 / 3 > 42 = true"
    let parseResult = runParser expression code None
    toSuccess parseResult |> ignore

[<Fact>]
let ``Expression Complex`` () =
    let code = "(x + 101) / 3 > 42 = true"
    let parseResult = runParser expression code None
    toSuccess parseResult |> ignore

//
// Variables
//

[<Fact>]
let ``Variable List`` () =
    let code = "x, y, z
"
    let parseResult = runParser variableList code None
    toSuccess parseResult |> ignore



//
// Assignment
//

[<Fact>]
let ``Variable Assignment Literal Int`` () =
    let code = "x = 42
"
    let parseResult = parseFile code None
    toSuccess parseResult |> ignore

[<Fact>]
let ``Variable Assignment Literal Float`` () =
    let code = "x = 42.42
"
    let parseResult = parseFile code None
    toSuccess parseResult |> ignore

[<Fact>]
let ``Assignment Literal Bool`` () =
    let code = "x = true
"
    let parseResult = parseFile code None
    toSuccess parseResult |> ignore

[<Fact>]
let ``Assignment Literal String`` () =
    let code = "x = \"Hello\"
"
    let parseResult = parseFile code None
    toSuccess parseResult |> ignore

[<Fact>]
let ``Assignment Variable List`` () =
    let code = "x, y, z = 42
"
    let parseResult = parseFile code None
    toSuccess parseResult |> ignore


//
// Loops
//

[<Fact>]
let ``Loop`` () =
    let code = "loop
"
    let parseResult = parseFile code None
    toSuccess parseResult |> ignore

[<Fact>]
let ``Loop n`` () =
    let code = "loop 42
"
    let parseResult = parseFile code None
    toSuccess parseResult |> ignore

[<Fact>]
let ``Loop Condition`` () =
    let code = "loop n < 42
"
    let parseResult = parseFile code None
    toSuccess parseResult |> ignore

[<Fact>]
let ``Loop Range`` () =
    let code = "loop n in rng
"
    let parseResult = parseFile code None
    //let parseResult = runParser loop code None
    toSuccess parseResult |> ignore
