namespace guessgame
open System

module GuessGame = 

    let guessGame = 
        let objrandom = new Random()

        let randnum= objrandom.Next(10)
        let num = randnum |> string
        let mutable guesses = []

        let mutable status  = false
        while not status do

            Console.WriteLine "Guess a number between 1 and 10 please: "
            let str = Console.ReadLine()
            if str = num then   
                status <- true 
                printfn $"{num} was correct! Really great job!"
                guesses <- guesses |> List.sort 
            else
                guesses <- guesses |> List.append [str]

                printfn $"{str} was not correct. Please guess again."

        printfn $"These were your guesses {guesses}"

    
    let numbers = [2;3;4;5;6]

    let pickDivisTwo = List.filter(fun x -> x % 2 = 0) numbers

    let filterDivisTwo i = 
        i % 2 = 0

    let filteredNumbers = numbers|> List.filter filterDivisTwo

    




         


