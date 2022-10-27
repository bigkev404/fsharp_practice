namespace Bricks

open System

type Brick =
    {
        StudColumns : int
        StudRows : int
        Color : ConsoleColor
    }

module Brick =

    let printConsole (brick : Brick) =
        let rowChar = 
            match brick.StudRows with
            | 1 -> "·"
            | 2 -> ":"
            | _ -> raise <| ArgumentException("Unsupported row count")
        let pattern = String.replicate brick.StudColumns rowChar            
        printf "%s " (brick.Color.ToString().Substring(0,1))
        Console.BackgroundColor <- brick.Color
        Console.ForegroundColor <- ConsoleColor.Black
        printf "[%s]" pattern
        Console.ResetColor()
        printf " "
        Console.ResetColor()

    let printEachBrick bricks = 
        printfn "All the bricks:"
        bricks
        |> Array.iter (printConsole)
        printfn "\n"



    let countBricks bricks = 
        let count = 
            bricks|> Array.length

        printfn "There are %i bricks" count
        count

    let sumOfStuds bricks = 
        
        let sumBricks = 
            bricks
            |> Array.map (fun (b:Brick) -> b.StudRows * b.StudColumns)
            |> Array.sum

        printfn "There are %i studs total" sumBricks
    
    let getColorBrick bricks color = 
        bricks
        |> Array.filter (fun (b:Brick) -> b.Color = color)
        |> Array.iter printConsole
        printfn "\n"

    let groupBricksByStud bricks = 
        printfn "Grouped by stud count (Array.groupBy):"
        let groupedByStud =
            bricks
            |> Array.groupBy (fun b -> (b.StudRows * b.StudColumns))
 
        groupedByStud 
        |> Array.sortByDescending fst
        |> Array.iter (fun (studcount, bricks) ->
            printfn "%i:" studcount
            bricks
            |> Array.iter printConsole
            Console.ResetColor()
            printfn ""
            
        )
        Console.ResetColor()
        printfn "\n"
        
    let groupBricksByColor bricks = 
        printfn "Grouped by color (Array.groupBy):"
        let groupedByColor =
            bricks
            |> Array.groupBy (fun b -> (b.Color))
 
        groupedByColor 
        |> Array.sortByDescending fst
        |> Array.iter (fun (colors, bricks) ->
            printfn "%O:" colors
            bricks
            |> Array.iter printConsole
            Console.ResetColor()
            printfn ""
            
        )
        Console.ResetColor()
        printfn ""
        

    

        

    

    
