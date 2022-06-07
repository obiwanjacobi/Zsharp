module Tests.ScratchPad

open System
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``My test`` () =
    42 |> should equal 42

