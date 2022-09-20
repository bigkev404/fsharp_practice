namespace StudentScores

type Student = 
    { 
        
        Surname: string
        GivenName: string
        Id: string
        MeanScore: float
        ScoresMin: float
        ScoresMax: float
    }

module Student = 

    let nameParts (s: string) = 
        let name = s.Split(',')
        match name with
        | [|surname; givenname|] -> 
            {| 
                Surname = surname.Trim()
                Givenname = givenname.Trim() 
            |}
            
        | [|surname|] -> 
            {| 
                Surname = surname.Trim()
                Givenname = ("(None)")
            |}

        | _ -> 
            raise(System.FormatException(sprintf "Invalid name format: %s" s ))
            



    let fromString( s : string) = 
        let elements = s.Split('\t')
        let name = elements.[0] |> nameParts
        let id = elements.[1]

        let scores = 
            elements 
            |> Array.skip 2
            |> Array.map TestResult.fromString
            |> Array.choose TestResult.TryEffectiveScore
            
        let meanScore = scores |> Array.average 
        let scoresMin = scores |> Array.min
        let scoresMax = scores |> Array.max
        { 
            Surname = name.Surname
            GivenName = name.Givenname
            Id = id 
            MeanScore = meanScore
            ScoresMin = scoresMin
            ScoresMax = scoresMax
        }

        


    let printSummary (student: Student)   = 
        printf "FirstName: %s LasttName: %s ID: %s Min, Max, Avg: %0.01f, %0.01f, %0.01f \n" student.GivenName student.Surname student.Id student.ScoresMin student.ScoresMax student.MeanScore
