namespace matching
open System

module Green = 

    let bottles n = 
        match n with 
        | 1 -> "One green bottle"
        | 2 -> "Two green bottles"
        | _ -> sprintf "%i green bottles" n

    let runGreen = 

        for i in 1..10 do 
            printfn "%s" (bottles i)

module Option = 
    
    let describe x = 
        match x with
        | Some v -> sprintf "The value was %O\n" v 
        | None -> sprintf "There was no value\n"

    let runOption = 
        printf "---\n"
        printf "%s" (Some 99 |> describe)
        printf "%s" (Some "abc" |> describe)
        printf "%s" (None |> describe)

module Array = 

    let describe a = 
        match a with 
        | [||] -> "An empty Array\n"
        | [|x|] -> sprintf "an array with one element: %O\n" x
        | [|x;y|] -> sprintf "an array with two element: %O, %O\n" x y
        | _ -> sprintf "an array with %O elements \n" a.Length

    let runArray = 
        printf "---\n"
        printf "%s" ([||] |> describe)
        printf "%s" ([|"kevin"|] |> describe)
        printf "%s" ([|"kevin"; "ashley"|] |> describe)
        printf "%s" ([|"kevin"; "ashley"; "kvothe"; "tinker"|] |> describe)

 module Tuple = 
    
    let describe t = 
        let a, b = t
        sprintf "A 2-tuple, first value %O, second value %O" a b

    let runTuple = 
        printf "---\n"
        let testTuble = 
            ("Kevin", "Ashley")
        printf "%s" ( testTuble  |> describe)
        

        
