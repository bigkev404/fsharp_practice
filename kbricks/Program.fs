open System
open Bricks

[<EntryPoint>]
let main argv =

    let bricks =

        [|
            //(3, 3, ConsoleColor.Yellow)
            //(2, 4, ConsoleColor.Yellow)
            (4, 2, ConsoleColor.Green)
            (2, 1, ConsoleColor.Magenta)
            (1, 1, ConsoleColor.Blue)
            (2, 2, ConsoleColor.Red)
            (4, 2, ConsoleColor.Blue)
            (4, 2, ConsoleColor.Magenta)
            (2, 2, ConsoleColor.Magenta)
            (2, 2, ConsoleColor.Red)
            (4, 2, ConsoleColor.Blue)
            (3, 2, ConsoleColor.Magenta)
            (4, 2, ConsoleColor.Green)
            (3, 2, ConsoleColor.Red)
            (4, 1, ConsoleColor.Blue)
            (4, 2, ConsoleColor.Yellow)
            (4, 2, ConsoleColor.Yellow)
            (1, 1, ConsoleColor.Blue)
            (1, 1, ConsoleColor.Green)
            (2, 1, ConsoleColor.Yellow)
            (4, 1, ConsoleColor.Magenta)
        |]
        |> Array.map (fun (sc, sr, cc) -> { StudColumns = sc; StudRows = sr; Color = cc })

    
    Brick.printEachBrick bricks
    //Brick.sumOfStuds bricks
    // Brick.countBricks bricks
    //Brick.getColorBrick bricks ConsoleColor.Yellow
    //Brick.groupBricksByStud bricks
    //Brick.groupBricksByColor bricks
    0
