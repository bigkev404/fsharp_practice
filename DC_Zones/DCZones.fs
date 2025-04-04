open System
open System.IO


let inputFolderPath = "C:\Users\kevin\Downloads\model\Model Run Prep\Minirunthree"
let shipmentpath = "C:\Users\kevin\Downloads\model\Model Run Prep\Minirunthree\SalesDataStitched2024_6.txt"
let zonespath = "C:\Users\kevin\Downloads\model\Model Run Prep\Minirunthree\DC Zones.csv"
let allocationpath = "C:\Users\kevin\Downloads\model\Model Run Prep\Minirunthree\24AllocationRnoMemOrg.csv"

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


//want to add in shipment type and parcel data too
type shipments = 
    {
        ShipKey: string //1
        Customer: string //9
        City: string //13
        State: string //14
        Country: string //
        ShipToLat: float //17
        ShipToLong: float //18
        Distance: float  //19
        ItemNumber: string //20
        Quantity: float //21
        Weight: float //22
        Date: string //33
        WH: string //38

    }

type allocation = 
    {
        Invoice: string //7
        BillTo: string //11
        OriginCity: string //3
        OriginState: string //4
        DestCity: string //5
        DestState: string //6
        FreightType: string //12
        Miles: float  //15
        PayAmount: float //21
        Weight: float //21
        Customer: string
     
    }

type allocationCompared = 
    {
        Invoice: string //7
        BillTo: string //11
        OriginCity: string //3
        OriginState: string //4
        DestCity: string //5
        DestState: string //6
        FreightType: string //12
        Miles: float  //15
        PayAmount: float //21
        Weight: float //21
        Customer: string
        Status: string
     
    }


type zones = 
    {
        State: string 
        Abbr: string 
        Reno: string
        Memphis: string
        Orangeburg: string 

    }

type ComparedShipments = 
    {
        ShipKey: string //1
        Customer: string //9
        City: string //13
        State: string //14
        Country: string //
        ShipToLat: float //17
        ShipToLong: float //18
        Distance: float  //19
        ItemNumber: string //20
        Quantity: float //21
        Weight: float //22
        Date: string //33
        WH: string //38
        Status: string

    }



let ProcessShipmentdata (filepath: string)  = 
    readLines ( filepath)
    |> Seq.map splitAtTabs
    |> Seq.skip 1
    |> Seq.choose (fun cols -> //This is for ignoring lines that are blank, usually at the end of the file
        match (cols[0].ToString().Trim().Length > 0) with //If something in the 1st column...
        | true -> Some cols
        | false -> None )
    |> Seq.filter (fun row -> row[13].ToString().Trim() = "US") //filter to only US
    |> Seq.map (fun row ->
        {

            ShipKey = row[1]//1
            Customer= row[7] //9
            City= row[11] //13
            State= row[12]//14
            Country = row[13]
            ShipToLat= convertToFloat row[15] //17
            ShipToLong= convertToFloat row[16]//18
            Distance= convertToFloat row[17] //19
            ItemNumber= row[18] //20
            Quantity= convertToFloat row[19] //21
            Weight= convertToFloat row[20]//22
            Date= row[31] //33
            WH= row[36] //38
        } )

let ProcessZonesdata (filepath: string) = 
    readLines ( filepath)
    |> Seq.map splitAtCommas
    |> Seq.skip 1
    |> Seq.choose (fun cols -> //This is for ignoring lines that are blank, usually at the end of the file
        match (cols[0].ToString().Trim().Length > 0) with //If something in the 1st column...
        | true -> Some cols
        | false -> None )
    |> Seq.map (fun row ->
        {
            State =  row[0]
            Abbr=  row[1]
            Reno =  row[2]
            Memphis =  row[3]
            Orangeburg =  row[4]
        } )

let ProcessAllocationData (filepath: string) = 
    readLines ( filepath)
    |> Seq.map splitAtCommas
    |> Seq.skip 1
    |> Seq.choose (fun cols -> //This is for ignoring lines that are blank, usually at the end of the file
        match (cols[0].ToString().Trim().Length > 0) with //If something in the 1st column...
        | true -> Some cols
        | false -> None )
    |> Seq.map (fun row ->

        
        //printfn "%A" row
        {
            Invoice = row[6] //7
            BillTo= row[10] //11
            OriginCity= row[2] //3
            OriginState= row[3] //4
            DestCity= row[4] //5
            DestState= row[5] //6
            FreightType= row[11] //12
            Miles= convertToFloat row[14] //15
            PayAmount= convertToFloat row[20] //21
            Weight= convertToFloat row[23] //24
            Customer = row[9]

        } )

let shipdata = ProcessShipmentdata shipmentpath
let zonedata = ProcessZonesdata zonespath 

let allocdata = ProcessAllocationData allocationpath

printfn $"Number of US shipments: {Seq.length shipdata}"
printfn $"Number of US zones: {Seq.length zonedata}"
printfn $"Number of allocations: {Seq.length allocdata}"

// shipdata
// |> Seq.take 5
// |> Seq.iter (fun item ->
//     printfn  "ShipKey: %s, City: %s, State: %s, Country: %s, WH: %s" item.ShipKey item.City item.State item.Country item.WH
// )

// zonedata
// |> Seq.take 5
// |> Seq.iter (fun item ->
//     printfn  "State: %s, Abbr: %s, Reno: %s, Memphis: %s, Orangeburg: %s" item.State item.Abbr item.Reno item.Memphis item.Orangeburg
// )

// allocdata
// |> Seq.take 3
// |> Seq.iter (fun item ->
//     //printfn  "Invoice: %s, BillTo: %s, Paid: %f" item.Invoice item.BillTo item.PayAmount
//     printfn "%A" item
// )

//for each sequence in shipments, if shipments.WH = 'DC MEM', then try and match shipments.State with either zones.Abbr or zones.State. 
//if match is successful, if zones.Memphis = "Yes" or "Neutral", return "In Zone"
//if zones.Memphis = "No" then return "Out of zone"
//if no matches made, return "No matches made"

let classifyShipment (zones: seq<zones>) (shipment: shipments) =
    
    let zoneMatch = 
            zones 
            |> Seq.tryFind (fun z -> 
                z.Abbr.Trim().ToUpper()  = shipment.State.Trim().ToUpper()  || z.State.Trim().ToUpper()  = shipment.State.Trim().ToUpper() )

    //memphis
    if shipment.WH = "DC MEM" then
        
        match zoneMatch with
        | Some zone ->
            match zone.Memphis with
            | "Yes" -> "In Zone"
            | "Neutral" -> "Neutral"
            | "No" -> "Out of zone"
            | _ -> "Unknown zone status"
        | None -> "No matches made"
    //ogb
    elif shipment.WH = "DC OGB" then
        
        match zoneMatch with
        | Some zone ->
            match zone.Orangeburg with
            | "Yes" -> "In Zone"
            | "Neutral" -> "Neutral"
            | "No" -> "Out of zone"
            | _ -> "Unknown zone status"
        | None -> "No matches made"
    //reno
    elif shipment.WH = "DC REO" then
        
        match zoneMatch with
        | Some zone ->
            match zone.Reno with
            | "Yes" -> "In Zone"
            | "Neutral" -> "Neutral"
            | "No" -> "Out of zone"
            | _ -> "Unknown zone status"
        | None -> "No matches made"

    else
        "Nada"

let processShipments (zones: seq<zones>) (shipments: seq<shipments>) =
    shipments
    |> Seq.map (fun s -> 
        { 
            
            ShipKey = s.ShipKey
            Customer = s.Customer
            City = s.City
            State = s.State
            Country = s.Country
            ShipToLat = s.ShipToLat
            ShipToLong = s.ShipToLong
            Distance = s.Distance
            ItemNumber = s.ItemNumber
            Quantity = s.Quantity
            Weight = s.Weight
            Date = s.Date
            WH = s.WH
            Status = classifyShipment zones s 
        })

let comparisons =  processShipments zonedata shipdata//|> Seq.take 50000

// comparisons
// |> Seq.filter (fun x ->
//     x.Status <> "In Zone" && x.Status <> "Out of zone" && x.Status <> "Neutral" )
// |> Seq.iter (fun x -> 
//     printfn "Shipkey: %s, State: %s, WH: %s, Status: %s" x.ShipKey x.State x.WH x.Status
//     ) 

//saveToCsv<ComparedShipments> (Path.Combine(inputFolderPath, "Shipping_Zones_Comparison_Test.csv")) comparisons

let classifyAllcoation (zones: seq<zones>) (allocs: allocation) =
    
    let zoneMatch = 
            zones 
            |> Seq.tryFind (fun z -> 
                z.Abbr.Trim().ToUpper()  = allocs.DestState.Trim().ToUpper()   )

    //memphis
    if allocs.BillTo = "MEM" then
        
        match zoneMatch with
        | Some zone ->
            match zone.Memphis with
            | "Yes" -> "In Zone"
            | "Neutral" -> "Neutral"
            | "No" -> "Out of zone"
            | _ -> "Unknown zone status"
        | None -> "No matches made"
    //ogb
    elif allocs.BillTo = "OGBD" then
        
        match zoneMatch with
        | Some zone ->
            match zone.Orangeburg with
            | "Yes" -> "In Zone"
            | "Neutral" -> "Neutral"
            | "No" -> "Out of zone"
            | _ -> "Unknown zone status"
        | None -> "No matches made"
    //reno
    elif allocs.BillTo = "RNO" then
        
        match zoneMatch with
        | Some zone ->
            match zone.Reno with
            | "Yes" -> "In Zone"
            | "Neutral" -> "Neutral"
            | "No" -> "Out of zone"
            | _ -> "Unknown zone status"
        | None -> "No matches made"

    else
        "Nada"

let processAllocation (zones: seq<zones>) (alls: seq<allocation>) =
    alls
    |> Seq.map (fun a -> 
        { 
            
            Invoice = a.Invoice
            BillTo= a.BillTo
            OriginCity= a.OriginCity
            OriginState= a.OriginState
            DestCity= a.DestCity
            DestState= a.DestState
            FreightType= a.FreightType
            Miles= a.Miles
            PayAmount= a.PayAmount
            Weight= a.Weight
            Customer = a.Customer
            Status = classifyAllcoation zones a
        })

let Alloccomparisons =  processAllocation zonedata allocdata//|> Seq.take 500

// Alloccomparisons
// |> Seq.filter (fun x ->
//     x.Status <> "In Zone" && x.Status <> "Out of zone" && x.Status <> "Neutral" )
// |> Seq.iter (fun x -> 
//     printfn "Invoice: %s, BillTo: %s, DestState: %s, Status: %s" x.Invoice x.BillTo x.DestState x.Status
//     ) 

//saveToCsv<allocationCompared> (Path.Combine(inputFolderPath, "Allocation_Zones_Comparison.csv")) Alloccomparisons