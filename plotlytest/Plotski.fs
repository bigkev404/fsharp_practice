open System
open System.IO
open Plotly.NET

let joinedpath = "C:\Users\kevin\Downloads\model\Model Run Prep\Miniruntwo\Final_Joined_Weights.csv"

let readLines (filePath:string) = seq {
    use sr = new StreamReader (filePath)
    while not sr.EndOfStream do
        yield sr.ReadLine () }

let splitAtTabs (text: string) =
    text.Split '\t'
    |> Array.toList

let splitAtCommas (text: string) =
    text.Split ','
    |> Array.toList

let convertToFloat numberText =
    let floatResult =
        numberText
        |> Seq.choose (fun c -> 
            let badChars = ['$'; ','; ')'; '('; '\"']
            match (badChars |> List.contains c) with
            | false -> Some c
            | _ -> None)
        |> String.Concat
        |> System.Double.TryParse
    match floatResult with
    | true, result -> result
    | false, _ -> 0.0001


// Define the custom record type
type joinedCSV = 
    {
        ItemNumber: string
        SR_Weight: float
        SR_UOM: string
        IM_BOD: string
        IM_weight_lb: float
        IM_weight_kg: float
        IM_weight_diff_lbs: float
        IM_Origin: string
        Oracle_Weight: float
    }
let data = 
    readLines ( joinedpath)
    |> Seq.map splitAtCommas
    |> Seq.skip 1
    |> Seq.choose (fun cols -> //This is for ignoring lines that are blank, usually at the end of the file
        match (cols[0].ToString().Trim().Length > 0) with //If something in the 1st column...
        | true -> Some cols
        | false -> None )
    |> Seq.map (fun row ->

        let im_lbs = convertToFloat row[4]
        let im_kgs = convertToFloat row[5]
        let diff = 
            try im_lbs - (im_kgs*2.205)
            with
                | ex -> 
                    // Handle any other exception
                    //printfn $"Error: {ex.Message}"
                    0.0001
        {
            ItemNumber = row[0]
            SR_Weight= convertToFloat row[1]
            SR_UOM= row[2]
            IM_BOD= row[3]
            IM_weight_lb= im_lbs
            IM_weight_kg= im_kgs
            IM_weight_diff_lbs = diff 
            IM_Origin= row[6]
            Oracle_Weight= convertToFloat row[7]
        } )

data
|> Seq.take 5
|> Seq.iter (fun item ->
    printfn "ItemNumber: %s, SR_Weight: %f, IM_weight_lb: %f" item.ItemNumber item.SR_Weight item.IM_weight_lb
)




// let N = 500
// let rnd = System.Random()
// let x = Array.init N (fun _ -> rnd.NextDouble())

// Chart.Histogram(x) |> Chart.show
let imWeightLbValues = 
    data
    |> List.ofSeq
    |> List.map (fun item -> item.IM_weight_kg)

Chart.Histogram(imWeightLbValues) |> Chart.show