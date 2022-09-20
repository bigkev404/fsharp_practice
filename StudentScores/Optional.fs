namespace StudentScores

type Optional<'T> = 
    |Something of 'T
    |Nothing


module Optional = 

    let a = Something "abv"
    let b = Something 1
    let c = Something 1.23
    let d = Nothing

    let defaultValue (d : 'T) (optional: Optional<'T>) =
        match optional with 
        |Something v -> v 
        | Nothing -> d
        


