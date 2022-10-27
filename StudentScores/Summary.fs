namespace StudentScores
open System.IO


module Summary = 

    let printCount rows = 
        let studentCount = (rows |> Seq.length) - 1
        printf "Student count: %i\n" studentCount

    let printGroupSummary (surname: string) (students : Student[]) = 
        printfn "%s" (surname.ToUpperInvariant()) 
        students
        |> Array.sortBy (fun (s: Student) -> s.GivenName, s.Id)
        |> Array.iter (fun s -> 
            printfn "\t%20s\t%s\t%0.1f\t%0.1f\t%0.1f" s.GivenName s.Id s.MeanScore s.ScoresMin s.ScoresMax) 
        
    let summarize schoolCodesFilePath filePath  = 
        let rows =  File.ReadLines filePath
        rows|> printCount

        let schoolCodes = SchoolCodes.load schoolCodesFilePath

        rows 
        |> Seq.skip 1
        |> Seq.map (Student.fromString schoolCodes)
        |> Seq.truncate 15
        //|> Seq.groupBy (fun (s:Student) -> s.Surname)
        //|> Seq.sortBy fst
        //|> Seq.iter (fun (surname, students) -> printGroupSummary surname students)
        //|> Array.sortBy (fun student -> student.Surname)
        |> Seq.iter Student.printSummary


    
        

        