
open System
open System.IO


let allocationpath = "C:\Users\kevin\Downloads\Allocation2024.csv"

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

type allocation = 
    {
        InvoiceID: string //7
        BillTo: string //11
        OriginCity: string //3
        OriginState: string //4
        DestCity: string //5
        DestState: string //6
        FreightType: string 
        FreightMode: string//12
        Miles: float  //15
        PayAmount: float //21
        Weight: float //21
        Customer: string
     
    }

let ProcessAllocationData (filepath: string) = 
    readLines ( filepath)
    |> Seq.map splitAtCommas
    |> Seq.skip 1
    |> Seq.choose (fun cols -> //This is for ignoring lines that are blank, usually at the end of the file
        match (cols[0].ToString().Trim().Length > 0) with //If something in the 1st column...
        | true -> Some cols
        | false -> None )
    |> Seq.filter (fun row -> ["MEM"; "OGBD"; "RNO"] |> List.contains (row[10].ToString().Trim()))
    |> Seq.map (fun row ->

        
        //printfn "%A" row
        {
            InvoiceID = row[6] //7
            BillTo= row[10] //11
            OriginCity= row[2] //3
            OriginState= row[3] //4
            DestCity= row[4] //5
            DestState= row[5] //6
            FreightType= row[11] //12
            FreightMode= row[1] //12
            Miles= convertToFloat row[14] //15
            PayAmount= convertToFloat row[20] //21
            Weight= convertToFloat row[23] //24
            Customer = row[9]

        } )

let allocdata = ProcessAllocationData allocationpath

// printfn $"Number of US shipments: {Seq.length allocdata}"

// allocdata
// |> Seq.take 3
// |> Seq.iter (fun item ->
//     //printfn  "Invoice: %s, BillTo: %s, Paid: %f" item.Invoice item.BillTo item.PayAmount
//     printfn "%A" item
// )

type AllocationSummary = 
    {
        DestState: string
        BillTo: string
        FreightMode: string
        Count: int
        AveragePayAmount: float
        AverageWeight: float
        AverageMiles: float
        AvgCostPerPound: float
    }


let groupedAllocations (allocationData: seq<allocation>) =
    allocationData
    |> Seq.groupBy (fun a -> (a.DestState, a.BillTo, a.FreightMode))
    |> Seq.map (fun ((destState, billTo, freightMode), group) ->
        let count = group |> Seq.length
        let avgPay = group |> Seq.averageBy (fun a -> a.PayAmount)
        let avgWeight = group |> Seq.averageBy (fun a -> a.Weight)
        let avgDist = group |> Seq.averageBy (fun a -> a.Miles)
        {
            DestState = destState
            BillTo = billTo
            FreightMode = freightMode
            Count = count
            AveragePayAmount = avgPay
            AverageWeight = avgWeight
            AverageMiles = avgDist
            AvgCostPerPound = avgPay / avgWeight
        })

let groupedAlloc =
    allocdata
    |> groupedAllocations

groupedAlloc
|> Seq.filter (fun row -> ["MEM"] |> List.contains (row.BillTo.ToString().Trim()))
|> Seq.filter (fun row -> ["AZ"; "CA"; "NV"; "PA"; "WA"] |> List.contains (row.DestState.ToString().Trim()))
//|> Seq.take 3
|> Seq.iter (fun item ->
    printfn "WH: %s, DestState: %s, Mode: %s, CPP: %f" item.BillTo item.DestState item.FreightMode item.AvgCostPerPound
)


type UpdatedAllocation = 
    {
        InvoiceID: string //7
        BillTo: string //11
        OriginCity: string //3
        OriginState: string //4
        DestCity: string //5
        DestState: string //6
        FreightType: string 
        FreightMode: string//12
        Miles: float  //15
        PayAmount: float //21
        CostPerPound: float
        Weight: float //21
        Customer: string
        UpdatedWH: string
        UpdatedCost: float
        UpdatedCostPerPound: float
        UpdateStatus: string
     
    }

let enhanceAllocations (allocations: allocation seq) (summaries: AllocationSummary seq) =
    let alternativeWH = "MEM"

    allocations
    |> Seq.map (fun alloc ->
        let recordUpdate =
            if alloc.BillTo = "RNO" then
                let effectiveCostPerPound = 
                    let firsttry = 
                        summaries 
                        |> Seq.tryFind (fun s -> 
                            s.BillTo = alternativeWH && 
                            s.DestState = alloc.DestState && 
                            s.FreightMode = alloc.FreightMode)
                        |> Option.map (fun s -> s.AvgCostPerPound)

                    match firsttry with
                    | Some cost -> cost  
                    | None ->
                        
                        summaries
                        |> Seq.tryFind (fun s -> 
                            s.BillTo = alternativeWH &&  
                            s.DestState = alloc.DestState && 
                            s.FreightMode = "LTL")
                        |> Option.map (fun s -> s.AvgCostPerPound)
                        |> Option.defaultValue (alloc.PayAmount)

                let effectiveWH = alternativeWH

                let effectiveStatus = 
                    if effectiveCostPerPound = alloc.PayAmount then
                        "No Match Found For State/Mode"
                    else
                        "Match Found for State/Mode"

                let effectiveCost = effectiveCostPerPound * alloc.Weight

                effectiveWH, effectiveCost,  effectiveCostPerPound, effectiveStatus

            else
                alloc.BillTo, alloc.PayAmount, (alloc.PayAmount / alloc.Weight),  "NoUpdatedNeeded"
        
        let wh,newcost, costPP, status = recordUpdate
        let finalCOST = 
            if newcost < alloc.PayAmount then
                    if (["PA"; "WA"] |> List.contains (alloc.DestState)
                        && ["RNO"] |> List.contains (alloc.BillTo)
                    ) then
                        newcost
                    else
                        alloc.PayAmount * 1.05
                else
                    newcost
        
        { 
            
            InvoiceID = alloc.InvoiceID
            BillTo = alloc.BillTo
            OriginCity= alloc.OriginCity
            OriginState= alloc.OriginState
            DestCity= alloc.DestCity
            DestState= alloc.DestState
            FreightType= alloc.FreightType
            FreightMode= alloc.FreightMode
            Miles= alloc.Miles
            PayAmount= alloc.PayAmount 
            CostPerPound = (alloc.PayAmount / alloc.Weight)
            Weight= alloc.Weight 
            Customer = alloc.Customer
            UpdatedWH = wh
            UpdatedCost = finalCOST
            UpdatedCostPerPound = costPP
            UpdateStatus = status
            
        }

    )


let allcoationUpdate = enhanceAllocations allocdata groupedAlloc 

allcoationUpdate
|> Seq.filter (fun row -> ["RNO"] |> List.contains (row.BillTo.ToString().Trim()))
//|> Seq.filter (fun row -> ["No Match Found For State/Mode"] |> List.contains (row.UpdateStatus.ToString().Trim()))
//|> Seq.filter (fun row -> row.PayAmount > row.UpdatedCost )
|> Seq.take 15
|> Seq.iter (fun item ->
    printfn " %s ->  %s  (%s)... Weight: %f Distance: %f  Cost: %f, NewCost: %f (Invoice: %s)" item.BillTo item.DestState item.FreightMode item.Weight item.Miles item.PayAmount item.UpdatedCost item.InvoiceID 
)

saveToCsv<UpdatedAllocation> ("C:\Users\kevin\Downloads\Allocation2024Updated.csv") allcoationUpdate
