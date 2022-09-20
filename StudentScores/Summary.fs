namespace StudentScores
open System.IO


module Summary = 

    let printCount rows = 
        let studentCount = (rows |> Array.length) - 1
        printf "Student count: %i\n" studentCount

    let summarize filePath  = 
        let rows =  File.ReadAllLines filePath
        rows
        |> printCount
        rows 
        |> Array.skip 1
        |> Array.map Student.fromString
        |> Array.sortBy (fun student -> student.Surname)
        |> Array.iter Student.printSummary

        