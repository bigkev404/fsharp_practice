namespace all
open System

module sequenceFunctions = 


    let generateDates (startDate: DateTime) = 
            Seq.initInfinite (fun i -> startDate.AddDays(float i))

    let listDaysofWeek = 

        generateDates DateTime.Now 
        |> Seq.filter (fun d -> d.Month = 1 && d.Day = 1)
        |> Seq.truncate 10
        |> Seq.iter (fun d -> printfn "%i %s" d.Year (d.DayOfWeek.ToString()))

