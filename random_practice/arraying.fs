namespace all
open System

module arrayFunctions = 

    let printArray a =  
        printfn "%A\n" a 


    let isEven x = 
        x % 2 = 0

    let numbers = [|1;2;4;8;16|]

    let numbersNew = 
        [|
            for i in 1..9 do
                let x = i * i 
                if x |> isEven then 
                    x
                
        |]

    let sumOfSquares = 
        let numArray = 
            [|
                for i in 1..1000 -> i * i 
            |]                   
            
        numArray
        |> Array.sum

    let intiallyZeros = Array.zeroCreate<int> 10

    let changeInitiallyZeroes = 
        intiallyZeros.[0] <- 14
        intiallyZeros 








        

    


    

    

    


