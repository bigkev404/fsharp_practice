open System.IO
open System
open StudentScores
open guessgame
//open Gaming


[<EntryPoint>]

let main argv = 


    //printfn $"Sum of consoles: {Gaming.totalSum}" 
    
    //play a game first! wahoo
    //GuessGame.guessGame

    if argv.Length > 0 then 
        
        let filePath = argv.[0] 

        if File.Exists filePath then 
            printf "Processing %s\n" filePath
            try
                Summary.summarize filePath
                0
            with 
                | :? FormatException as e -> 
                    printfn "Error: %s" e.Message
                    printfn "The file was not in the expected format"
                    1
                | :? IOException as e -> 
                    printfn "Error: %s" e.Message
                    printfn "The file is still open in another program"
                    1

        else 
            printf "%s does not exist\n" filePath
            2              
    else
        printfn "Please specify a file\n"
        3



