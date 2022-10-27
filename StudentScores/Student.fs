namespace StudentScores

type Student = 
    { 
        
        Surname: string
        GivenName: string
        Id: string
        SchoolName: string
        MeanScore: float
        ScoresMin: float
        ScoresMax: float
    }

module Student = 

    open System.Collections.Generic

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
            



    let fromString(schoolCodes : Map<string,string>)( s : string)  = 
        let elements = s.Split('\t')
        let name = elements.[0] |> nameParts
        let id = elements.[1]
        let schoolCode = elements.[2] |> string
        //let schoolName = schoolCodes.[schoolCode]
        let schoolName = 
            // schoolCodes.TryFind(schoolCode) 
            // |> Option.defaultValue "(Unknown)"
            schoolCodes
            |> Map.tryFind(schoolCode)
            |> Option.defaultValue "(UNknown)"



        let scores = 
            elements 
            |> Array.skip 3
            |> Array.map TestResult.fromString
            |> Array.choose TestResult.TryEffectiveScore
            
        let meanScore = scores |> Array.average 
        let scoresMin = scores |> Array.min
        let scoresMax = scores |> Array.max
        { 
            Surname = name.Surname
            GivenName = name.Givenname
            Id = id 
            SchoolName = schoolName 
            MeanScore = meanScore
            ScoresMin = scoresMin
            ScoresMax = scoresMax
        }

        
    let printSummary (student: Student)   = 
        (printf "FirstName: %s, LasttName: %s, School: %s, ID: %s, Min, Max, Avg: %0.01f, %0.01f, %0.01f \n" 
            student.GivenName student.Surname student.SchoolName student.Id 
            student.ScoresMin student.ScoresMax student.MeanScore)

