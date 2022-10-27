namespace StudentGrades
open System.IO


type Student = 
    {
        SurName: string
        GivenName: string
        Id: string
        MinScore: float
        MaxScore: float
        MeanScore: float
        MissingCount: int
    }

type TestResult = 
    | Absent
    | Excused
    | Scored of float

module FLoat = 
    let tryFromString defaultValue s = 
        if s = "N/A" then 
            Some (defaultValue)
        else 
            Some (s |> float) 

    let filterScores s = 
        if s = "A" then 
            true
        elif s = "E" then 
            true
        else 
            false

module processTest = 
    let fromString s = 
        if s = "A" then 
            Absent 
        elif s = "E" then 
            Excused 
        else 
            let balue = s |> float 
            Scored balue 

    let effectiveScore (testResult: TestResult) = 
        match testResult with 
        | Absent  -> Some 0.0
        | Excused -> None
        | Scored score -> Some (score)

module Student =  

    let nameParts (s:string)  = 
        if s.Contains(',') then

            let names = s.Split(',')
            let firstname = names.[1].Trim()
            let lastname = names.[0].Trim()
            (firstname, lastname)
        else 
            let names = s.Split(',')
            let firstname = "(None)"
            let lastname = names.[0].Trim()
            (firstname, lastname)
            
        

    let fromString (row:string) = 
        let elements = 
            row.Split('\t')
        let firstname, lastname = nameParts elements.[0]
        let id = elements.[1]
        let scores = 
            elements
            |> Array.skip 2
            |> Array.map processTest.fromString
            |> Array.choose processTest.effectiveScore
        
        let missingcount = 
            elements
            |> Array.skip 2
            |> Array.filter FLoat.filterScores
            |> Array.length 

            //elements.Length - scores.Length - 2
        
        let meanScore = scores|> Array.average
        let minScore = scores|> Array.min
        let maxScore = scores|> Array.max
        
        {
            Id = id
            SurName = lastname
            GivenName = firstname
            MinScore = minScore
            MaxScore = maxScore
            MeanScore = meanScore
            MissingCount = missingcount
        }

    let printStats (kiddo: Student) = 
        
        (printf "%s\t%0.1f\t%0.1f\t%0.1f\t%i\t%s\n" 
        kiddo.Id kiddo.MinScore kiddo.MaxScore 
        kiddo.MeanScore kiddo.MissingCount kiddo.GivenName)

    let printStatsGrouped (surname: string) ( kiddos : Student[]) = 
        printfn $"{surname.ToUpperInvariant()}"
        //printfn "%s" (surname.ToUpperInvariant())
        kiddos
        |> Array.sortBy ( fun k -> k.GivenName)
        |> Array.iter printStats


module summarize = 

    //let filePath = "Samples/StudentScores.txt"
    let filePath = "Samples/StudentScoresAE.txt"

    let testFilePath filePath = 
        
        if File.Exists filePath then
            printf "Processing file %s.\n" filePath
            let rows = File.ReadAllLines filePath
            let studentCount = rows.Length - 1
            printfn "There are %i students" studentCount

        else 
            printf "File not found. Please Specify an existing file."

    
    
    let summarize = 
        testFilePath filePath

        let rows = File.ReadAllLines filePath
        rows
        |> Array.skip 1
        |> Array.map Student.fromString
        //|> Array.sortBy (fun student -> student.MeanScore)
        //skip everyone but top 10 students
        |> Array.groupBy (fun s -> s.SurName)
        |> Array.sortBy fst
        //|> Array.skip 90
        |> Array.iter (fun (surname, students) -> 
            Student.printStatsGrouped surname students)
        //|> Array.iter Student.printStats


        

    
        