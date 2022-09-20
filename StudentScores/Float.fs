namespace StudentScores 

module Float = 
    let tryFromString (s:string) : float option = 
        if s = "N/A" then   
            None 
        else 
            Some (float s) 


    let fromStringOr (d:float) (s:string) : float =  
        s
        |> tryFromString
        |> Option.defaultValue d