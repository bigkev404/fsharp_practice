printfn "Hello from F#"
open System


let askHow person =
    printfn "How are you doing, %s?" person 

let isValid person = 
    String.IsNullOrWhiteSpace person |> not

let runtime = System.DateTime.Now.ToShortTimeString()

let notAllowed person = 
    person <> "Karla"


[<EntryPoint>]

let main argv =

    
    if argv.Length = 0 then 
            
        printfn "Kevin, Did you forget to include an argument?"

    else 
        argv 
        |> Array.filter isValid
        |> Array.filter notAllowed
        |> Array.iter askHow

        printf "Nice to meet ya. It's currently %s" runtime

    0
