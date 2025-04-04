open System
open System.IO

let inputFolderPath = """C:\Users\kevin\Downloads\model\Model Run Prep\Miniruntwo""" // 
let SRinputFolderPath = """C:\Users\kevin\Downloads\model\Model Run Prep\Miniruntwo\AllShipReports"""

let inScopeWH = ["MEM";"OGB";"REO";"COE"]

type SRInputRow =
        {
            ShipNumber: int64 
            ShipKey: string
            Delivery: string
            Warehouse: string
            CustomerName: string
            ShipToNumber: string
            ShipCity: string
            ShipState: string
            CustomerLoc: string
            ItemNumber: string
            QuantityShipped: float
            SalesOrderNumber: string
            ShipmentDate: DateTime
            FreightTerm: string
            FreightMode: string
            CalculatedFreight: string
            Waybill: string
            Wt_Lb: float
            Wt_UOM: string
            LineItem_Wt_Lb: float
            Shipment_Wt_Lb: float
            ShipMethod: string
        }


 
let readLines (filePath:string) = seq {
    use sr = new StreamReader (filePath)
    while not sr.EndOfStream do
        yield sr.ReadLine () }

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
    | false, _ -> failwith "Quantity parse error."


let splitAtTabs (text: string) =
    text.Split '\t'
    |> Array.toList

// Function to save data to a file
let saveResultsToFile (outputFilePath: string) (data: seq<string>) =
    File.WriteAllLines(outputFilePath, data)


let readSRFile (fileName: string) = 
        let mutable numCols = 0
        readLines fileName
        |> Seq.map splitAtTabs
        |> Seq.skip 1
        |> Seq.mapi (fun idx row -> idx, row)
        |> Seq.choose (fun (idx, cols) -> //This is for ignoring lines that are blank, usually at the end of the file
            match (cols[0].ToString().Trim().Length > 0) with //If something in the 1st column...
            | true -> Some (idx, cols)
            | false -> None )
        |> Seq.choose (fun (idx, row) -> 
            let isInScope =
                inScopeWH
                |> List.tryFind (fun wh ->
                    wh = row[9].Trim().ToUpper())
            match isInScope with
            | Some warehouses -> Some (idx, row)
            | None -> None )
        


let srFilesData = 

    let dir = (DirectoryInfo(SRinputFolderPath) )
    // 
    let dirs = dir.EnumerateDirectories()
    let shippingFiles = dir.EnumerateFiles("*.txt")
    let srRows = 
        shippingFiles 
        |> Seq.choose (fun file ->
            match file.Name.ToUpper().Contains("SHIPPING REPORT") with
            | true -> 
                printfn "Selected SR Report: %s" file.Name
                Some file
            | false -> None )
        |> Seq.map (fun file ->
            readSRFile file.FullName )
        |> Seq.concat
        |> Seq.map (fun (idx, row) ->
            let wb = 
                row[95]// this could return a ""
             
            let delivery = row[59].Trim().ToUpper()
            //let tpMode, cost, term, shipkey = 
            let foundMode = 
                row[56].Trim().ToUpper()
            let itemNumber = row[13].Trim().ToUpper()

            let customerLoc = row[68].Trim().ToUpper() + "_" + row[72].Trim().ToUpper()
            let wt_lb = 
                
                try 
                        convertToFloat (row[57].Trim())
                with

                | ex -> 
                    // Handle any other exception
                    //printfn $"Error: {ex.Message}"
                    0.0001
            let wt_uom = 
                
                try 
                        (row[46].Trim())
                with

                | ex -> 
                    // Handle any other exception
                    //printfn $"Error: {ex.Message}"
                    "No UOM in SR"

            let qty = 
                
                    convertToFloat (row[30].Trim())
        
            let dato = 
                
                try 
                        DateTime.Parse(row[19])
                with

                | ex -> 
                    // Handle any other exception
                    //printfn $"Error: {ex.Message}"
                    DateTime.Now
            
            //63,111 and 42970 shipkeys for Jan-August
            //125227 and 64038 shipkeys for year2024
            
            let line_Wt_Lb = wt_lb * qty

            {
                ShipNumber = 0
                ShipKey = foundMode
                Delivery = delivery
                Warehouse = row[9].Trim().ToUpper()
                CustomerName = row[67].Trim().ToUpper()
                CustomerLoc = customerLoc
                ShipToNumber = row[97].Trim().ToUpper()
                ShipCity = row[70].Trim().ToUpper()
                ShipState = row[71].Trim().ToUpper()
                ItemNumber = itemNumber
                QuantityShipped = qty
                SalesOrderNumber = row[10].Trim() + " " + row[11].Trim()
                ShipmentDate = dato

                FreightTerm = row[42].Trim().ToUpper()
                FreightMode = row[56].Trim().ToUpper()
                CalculatedFreight = foundMode
                Waybill = wb
                Wt_Lb = wt_lb
                Wt_UOM = wt_uom
                LineItem_Wt_Lb = line_Wt_Lb
                Shipment_Wt_Lb = 0.0
                ShipMethod = row[98].Trim().ToUpper()
            } ) 

    printfn $"Number of shipping report rows {srRows |> Seq.length}"
    srRows

    
// let csvHeader = "ShipNumber,ShipKey,Delivery,Warehouse,CustomerName,CustomerLoc,ShipToNumber,ShipCity,ShipState,ItemNumber,QuantityShipped,SalesOrderNumber,ShipmentDate,FreightTerm,FreightMode,CalculatedFreight,Waybill,Wt_Lb,Wt_UOM,LineItem_Wt_Lb,Shipment_Wt_Lb,ShipMethod"
            
// let csvData =
//     srFilesData
//     |> Seq.map (fun row ->
//         // Convert each row to a CSV line
//         $"{row.ShipNumber},{row.ShipKey},{row.Delivery},{row.Warehouse},{row.CustomerName},{row.CustomerLoc},{row.ShipToNumber},{row.ShipCity},{row.ShipState},{row.ItemNumber},{row.QuantityShipped},{row.SalesOrderNumber},{row.ShipmentDate},{row.FreightTerm},{row.FreightMode},{row.CalculatedFreight},{row.Waybill},{row.Wt_Lb},{row.Wt_UOM},{row.LineItem_Wt_Lb},{row.Shipment_Wt_Lb},{row.ShipMethod}"
//     )

// // Combine header and data
// let csvLines = Seq.append [csvHeader] csvData

// // Save the results to a CSV file
// let outputFilePath = Path.Combine(inputFolderPath, "SR_Results.csv")

// saveResultsToFile outputFilePath csvLines

type SRWeight = { 
    ItemNumber: string
    SR_Weight: float 
    UOM: string
}

type ItemMasterRow =
        {
            ItemNumber: string
            BOD: string
            Wt_Lb: float
            Wt_Kg: float
            Origin: string
        }

type OracleWeight = {
    KCUItemNumber: string
    OracleWeight: float
}

let oracle = 
        let mutable numCols = 0
        //readLines (Path.Combine(inputFolderPath, "Oracle Weight.txt"))
        readLines (Path.Combine(inputFolderPath, "Oracle Weight - Old.txt"))
        |> Seq.map splitAtTabs
        |> Seq.skip 1
        |> Seq.choose (fun cols -> //This is for ignoring lines that are blank, usually at the end of the file
            match (cols[0].ToString().Trim().Length > 0) with //If something in the 1st column...
            | true -> Some cols
            | false -> None )
        |> Seq.map (fun row ->
            let wt = 
                try
                    convertToFloat(row[1].Trim().ToUpper())
                with 
                    | e -> 0.0
            {
                KCUItemNumber = row[0].Trim().ToUpper()
                OracleWeight = wt
            } )
        // |> Seq.map (fun row ->
        //     row.KCUItemNumber, row.OracleWeight)
        // |> Map


//pairs
let distinctPairs =
    srFilesData
    |> Seq.distinctBy (fun item -> (item.ItemNumber, item.Wt_Lb)) // Use a tuple to ensure distinct pairs
    |> Seq.map (fun item -> 
        ({ 
            ItemNumber = item.ItemNumber
            SR_Weight = item.Wt_Lb
            UOM = item.Wt_UOM
        })
        ) // Extract the pairs

printfn $"Number of distinct items in shipping report {distinctPairs |> Seq.length}"


// let csvPath = Path.Combine(inputFolderPath, "SR_Weights.csv")

// // Write the distinct pairs to the CSV file
// let writer = new StreamWriter(csvPath)
// writer.WriteLine("ItemNumber,ShippingReportWeight, SR_UOM") // Write the header
// distinctPairs 
// |> Seq.iter (fun (itemNumber, wt_lb, wt_u) -> writer.WriteLine($"{itemNumber},{wt_lb},{wt_u}"))
    
    
let itemMaster = 
        let mutable numCols = 0
        //readLines (Path.Combine(inputFolderPath, "NewItemMaster.txt"))
        readLines (Path.Combine(inputFolderPath, "NewItemMaster - Old.txt"))
        |> Seq.map splitAtTabs
        |> Seq.skip 1
        |> Seq.choose (fun cols -> //This is for ignoring lines that are blank, usually at the end of the file
            match (cols[0].ToString().Trim().Length > 0) with //If something in the 1st column...
            | true -> Some cols
            | false -> None )
        |> Seq.mapi (fun idx row ->
            if numCols = 0 then
                numCols <- row.Length
            else if numCols <> row.Length then
                let rowMsg =
                    row
                    |> Seq.mapi (fun col cell -> 
                        sprintf $"{col}: '{cell}'")
                    |> String.concat "\r\n"
                failwith $"Row {idx + 2} = {rowMsg}"
            let weightStr = 
                row[67].Trim()
                |> Seq.choose (fun c ->
                    let badChars = [',';'\"';' ']
                    match (badChars |> List.contains c) with
                    | false -> Some c
                    | _ -> None)
                |> String.Concat
            let weightStrKG = 
                row[20].Trim()
                |> Seq.choose (fun c ->
                    let badChars = [',';'\"';' ']
                    match (badChars |> List.contains c) with
                    | false -> Some c
                    | _ -> None)
                |> String.Concat
            let floatResult =
                match Double.TryParse (weightStr) with
                | true, n -> Some n
                | _ -> failwith $"Problem with row {idx+2}. Weight is {row[67]}."
            let floatResultKG =
                try 
                        convertToFloat weightStrKG
                with
                | ex -> 0.0001
            {
                ItemNumber = row[0].Trim().ToUpper()
                BOD = row[54].Trim().ToUpper()
                Wt_Lb = floatResult |> Option.defaultValue 0.0001
                Wt_Kg = floatResultKG  
                Origin = row[48].Trim().ToUpper()
            } )   

printfn $"Number of distinct items in itemMaster report {itemMaster |> Seq.length}"


// oracle

// itemMaster

// distinctPairs

let joined1 =
    distinctPairs
    |> Seq.map (fun sr ->
        let itemMasterMatch =
            itemMaster
            |> Seq.tryFind (fun im -> im.ItemNumber = sr.ItemNumber)
        match itemMasterMatch with
        | Some im -> (sr, Some im)
        | None -> (sr, None)
    )
printfn $"Number of distinct items in joined1 report {joined1 |> Seq.length}"

let finalJoined =
    joined1
    |> Seq.map (fun (sr, im) ->
        let oracleMatch =
            oracle
            |> Seq.tryFind (fun oracle -> oracle.KCUItemNumber = sr.ItemNumber)
        match oracleMatch with
        | Some oracle -> (sr, im, Some oracle)
        | None -> (sr, im, None)
    )
printfn $"Number of distinct items in finalJoined report {finalJoined |> Seq.length}"

let csvPath = Path.Combine(inputFolderPath, "Final_Joined_Weights.csv")
let writer = new StreamWriter(csvPath)

// Write the header
writer.WriteLine("ItemNumber,SR_Weight,UOM,BOD,Wt_Lb,Wt_Kg,Origin,OracleWeight")

// Write each row
finalJoined
|> Seq.iter (fun (sr, im, oracle) ->
    let bod = im |> Option.map (fun i -> i.BOD) |> Option.defaultValue "Missing"
    let wtLb = im |> Option.map (fun i -> i.Wt_Lb) |> Option.defaultValue 0.0001
    let wtKg = im |> Option.map (fun i -> i.Wt_Kg) |> Option.defaultValue 0.0001
    let origin = im |> Option.map (fun i -> i.Origin) |> Option.defaultValue "Missing"
    let oracleWeight = oracle |> Option.map (fun o -> o.OracleWeight) |> Option.defaultValue 0.0001
    writer.WriteLine($"{sr.ItemNumber},{sr.SR_Weight},{sr.UOM},{bod},{wtLb},{wtKg},{origin},{oracleWeight}")
)

printfn $"CSV files have been joined and saved to {csvPath}"

type joinedCSV = 
    {
        ItemNumber: string
        SR_Weight: float
        SR_UOM: string
        IM_BOD: string
        IM_weight_lb: float
        IM_weight_kg: float
        IM_Origin: string
        Oracle_Weight: float

    }