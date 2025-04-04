open System
open System.IO


let inputFolderPath = """C:\Users\kevin\Downloads\model\Model Run Prep\Minirunthree"""
//let stitchedpath = Path.Combine(inputFolderPath, "\SalesDataStitched2024_6.txt")
let stitchedpath = "C:\Users\kevin\Downloads\model\Model Run Prep\Minirunthree\SalesDataStitched2024_6.txt"
let newweightspath = "C:\Users\kevin\Downloads\model\Model Run Prep\Minirunthree\Weights_Master.txt"


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
    | false, _ -> 0.000111

let saveToCsv<'T> (filePath: string) (data: seq<'T>) =

    let headers = 
        typeof<'T>.GetProperties()
        |> Array.map (fun prop -> prop.Name)
        |> String.concat ","
    
    let csvData = 
        data 
        |> Seq.map (fun item ->
            typeof<'T>.GetProperties()
            |> Array.map (fun prop -> 
                let value = prop.GetValue(item)
                if value = null then "" else value.ToString()
            )
            |> String.concat ","
        )
    
    let csvLines = Seq.append [headers] csvData
    
    File.WriteAllLines(filePath, csvLines)
    printfn "File Saved Successfully to: %s" filePath

let readTxt (filepath: string)  = 
    readLines ( filepath)
    |> Seq.map splitAtTabs
    |> Seq.skip 1
    |> Seq.choose (fun cols -> //This is for ignoring lines that are blank, usually at the end of the file
        match (cols[0].ToString().Trim().Length > 0) with //If something in the 1st column...
        | true -> Some cols
        | false -> None )

type sales = 
    {
        KCUItemNumber: string
        Quantity: float
        StichedWeight: float
    }

type weights = 
    {
        KCUItemNumber: string
        MasterWeight: float
    }


type compared = 
    {
        KCUItemNumber: string
        Quantity: float
        StichedWeight: float
        MasterWeight: float
        MismatchTag: string
        StatusTag : string
        FinalWeight: float
    }

let stichedData = 
    stitchedpath 
    |> readTxt
    |> Seq.map(  fun row-> 
        {
            KCUItemNumber = row[18]
            Quantity= row[19] |> convertToFloat
            StichedWeight = row[20] |> convertToFloat
        })
printfn "\nstichedData %d" (Seq.length stichedData)

// stichedData
// |> Seq.take 3
// |> Seq.iter (fun item ->
//     printfn  "KCUItemNumber: %s, Quantity: %f, Weight: %f" item.KCUItemNumber item.Quantity item.StichedWeight )


let weightsData = 
    newweightspath 
    |> readTxt
    |> Seq.map(  fun row-> 
        {
            KCUItemNumber = row[1]
            MasterWeight = row[4] |> convertToFloat
        })
 
// weightsData
// |> Seq.take 3
// |> Seq.iter (fun item ->
//     printfn  "KCUItemNumber: %s, Weight: %f" item.KCUItemNumber item.MasterWeight )

printfn "\nweightsData: %d" (Seq.length weightsData)

let compareSalesAndWeights (salesList: sales seq) (weightsList: weights seq) =

    let weightsLookup = 
        weightsList 
        |> Seq.map (fun w -> w.KCUItemNumber, w)
        |> dict
    
    salesList
    |> Seq.map (fun sale ->
        match weightsLookup.TryGetValue(sale.KCUItemNumber) with
        | true, weight ->
            let comp = abs (sale.StichedWeight - weight.MasterWeight) < 0.01
            if weight.MasterWeight = 0.000111 then

                {
                    KCUItemNumber = sale.KCUItemNumber
                    Quantity = sale.Quantity
                    StichedWeight = sale.StichedWeight
                    MasterWeight = weight.MasterWeight
                    StatusTag = "Match Found"
                    MismatchTag = "Missing Item MasterData"
                    FinalWeight = sale.StichedWeight
                }
            else 
                {
                    KCUItemNumber = sale.KCUItemNumber
                    Quantity = sale.Quantity
                    StichedWeight = sale.StichedWeight
                    MasterWeight = weight.MasterWeight
                    StatusTag = "Match Found"
                    MismatchTag = (
                        match comp with
                        | true -> "Weights Match"
                        | false -> "Weight Mistmatch")
                    FinalWeight = weight.MasterWeight
                }
        | false, _ ->
            {
                KCUItemNumber = sale.KCUItemNumber
                Quantity = sale.Quantity
                StichedWeight = sale.StichedWeight
                MasterWeight = 0.000111 // Default value when no match found
                StatusTag = "No match found"
                MismatchTag = "No match found"
                FinalWeight = sale.StichedWeight
            }
    )

let results = compareSalesAndWeights stichedData weightsData
let distinctresults = results |> Seq.distinctBy (fun x -> x.KCUItemNumber)

let filteredResults =
 results
|> Seq.filter (fun x ->
    x.MismatchTag = "Missing Item MasterData" && x.FinalWeight = 0.0 )
|> Seq.distinctBy (fun x -> x.KCUItemNumber)


printfn "\nskus with no data in either: %d" (Seq.length filteredResults)

printfn "\nTotal distinct items: %d" (Seq.length distinctresults)

printfn "\n%A" (filteredResults |> Seq.take 3)

// x.MismatchTag = "Missing Item MasterData" && x.FinalWeight = 0.0
// skus with no data in either : 78

let finalclean (combineData: compared seq) =
    combineData
    |> Seq.map(  fun x-> 
        //StatusTag = "Match Found" MismatchTag = "Missing Item MasterData" and 0.0
        let finalweight = 
            if  x.FinalWeight = 0.0 then
                0.01
            else 
                x.FinalWeight
        {
            KCUItemNumber = x.KCUItemNumber
            MasterWeight = finalweight
        })

let lastclean = results |> finalclean

printfn "\n%A" (lastclean |> Seq.take 3)

saveToCsv<weights> (Path.Combine(inputFolderPath, "CleanedWeights.csv")) lastclean
