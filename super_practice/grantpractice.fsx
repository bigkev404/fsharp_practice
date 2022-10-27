// run this code with alt enter after selecting the lines you want to run
open System

open System.Collections.Generic
//#time "on"
let numberSeq = seq{1 .. 1_000_000}
let numberArray = [|1 .. 1_000_000|]
let numberList = [1 .. 1_000_000]

numberList

[|1..1000|]
|> Array.map (fun i -> i * i)
|> Array.sum