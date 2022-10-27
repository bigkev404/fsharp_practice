namespace Practice
open System

module sayHello = 

    let peopleArray = 
        [|"Kitty"; "Levin"; "  "; "Eve"; "Tolstoy"|]
    
    let sayHello person = 
        printfn "Hello, %s!" person

    let isValid person = 
        String.IsNullOrWhiteSpace person |> not

    let notAllowed person = 
        person <> "Eve"

    let print_people = 
        if peopleArray.Length > 0 then
            peopleArray 
            |> Array.filter isValid
            |> Array.filter notAllowed
            |> Array.iter sayHello
                
        else
            printfn "Hello, stranger."
        printfn "Nice to meet you."













// module grantCollections = 
//         //Collection Code Examples
//     //WebEx Recording Links
//     //Session 1: https://simulationdynamics.webex.com/simulationdynamics/ldr.php?RCID=8820f0da8cd1834c519373b42072ca1a
//     //Session 2: https://simulationdynamics.webex.com/simulationdynamics/ldr.php?RCID=85ba32d3ad7041a79913cff96a8ccc11
//     //Session 3: https://drive.google.com/file/d/1aCsAWmKH7nw7FuHkWmAU4Md3VE6zqat4/view?usp=sharing
//     open System
//     open System.Collections.Generic
//     //#time "on"
//     let numberSeq = seq{1 .. 1_000_000}
//     let numberArray = [|1 .. 1_000_000|]
//     let numberList = [1 .. 1_000_000]
//     let numArray1 = Array.init 5000 (fun i -> i |> float)
//     let numArray2 = [|5000. .. 10_000.|]
//     let combinedArray = Array.append numArray1 numArray2
    
//     //combinedArray[5]

//     let lengthOfArray = Array.length combinedArray
//     let newArray =
//         combinedArray
//         |> Array.map(fun num -> num * 2.)
//         |> Array.filter(fun num -> num % 3. = 0)
//         |> Array.rev
//         |> Array.sort
//         |> Array.skip 1000
//         |> Array.chunkBySize 100
//         |> Array.collect(id)
//     let isDivisbleBy5 =
//         newArray
//         |> Array.groupBy(fun num -> num % 5. = 0)
//     let avgOfArray = newArray |> Array.average
//     let minOfArray = newArray |> Array.min
//     let maxOfArray = newArray |> Array.max
//     let sumOfArray = newArray |> Array.sum
//     let simpleArray = [|1 .. 10|]
//     simpleArray[0] <- 99
//     Array.fill simpleArray 2 3 12
//     let countOfValues =
//         simpleArray
//         |> Array.countBy(id) //fun i -> i
//     //simpleArray |> Array.distinct

//     let findExample =
//         simpleArray |> Array.findIndex(fun num -> num > 50)
//     let tryFindExample =
//         simpleArray |> Array.tryFind(fun num -> num > 50)
//     let rollingWindow =
//         simpleArray |> Array.windowed 4
//     let pairwiseWindow =
//         simpleArray
//         |> Array.pairwise
//         |> Array.map(fun (before, after) -> after - before)
    
//     //[||] |> Array.pairwise

//     //Array.empty<int> |> Array.pairwise
    
//     let does8Exist =
//         simpleArray |> Array.exists(fun num -> num = 8)
//     let doesItContain9 =
//         simpleArray |> Array.contains 9
//     let add x y = x + y
    
//     Array.fold add 0 [|1 .. 20|]
//     Array.scan add 0 [|1 .. 20|]
//     (0, [|1 .. 20|])
//     ||> Array.fold add
//     Array.fold Array.append [||] [|[|1 .. 10|]; [|11 .. 20|]; [|21 .. 30|]|]
//     Array.scan Array.append [||] [|[|1 .. 10|]; [|11 .. 20|]; [|21 .. 30|]|]
//     simpleArray |> Array.toSeq |> Array.ofSeq
    
    
//     //Lists
//     let plants = ["Boston"; "Dallas"; "Nashville"]
//     let newPlants = ["Knoxville"; "Austin"]
//     let combinedPlants = plants @ newPlants
//     let finalPlants = "Houstin" :: combinedPlants
//     let rec plantStringBuilder (acc: string) (plants: string list) =
//         match plants with
//         | [] -> acc
//         | hd::[] ->
//             let newAcc = acc + $"{hd}"
//             newAcc
//         | hd::tl ->
//             let newAcc = acc + $"{hd}, "
//             plantStringBuilder newAcc tl
//     let plantString = plantStringBuilder "" finalPlants


//     //Seq
//     let generatedSeq =
//         Seq.unfold(fun state -> Some (state, state+1)) 0
//         |> Seq.skip 1_000_000
//         |> Seq.take 100
//     [1 .. 20]
//     |> Seq.filter(fun i -> i < 10)



//     //Sets
//     let seta = set [3;1;2]
//     let setstring = set ["a";"c";"b"]
//     let a = set [1 ..2.. 10]
//     let b = set [1 ..3.. 10]
//     let c = Set.intersect a b
//     let d = Set.union a b // a + b
//     let e = Set.difference a b // b - a
//     let f = Set.remove 3 a // a.Remove 3
//     let g = Set.add 10 a
//     let naCountriesLookup = set ["USA"; "Canada"; "Mexico"]
//     let loadedCountries1 = set ["USA"; "Germany"; "Canada"; "France"; "Brazil"]
//     let loadedCountries2 = set ["Chile"; "Italy"; "Germany"; "Ukraine"; "China"]
//     let validNACountries = Set.intersect naCountriesLookup loadedCountries1
//     let nonNACountries = Set.difference loadedCountries1 naCountriesLookup
//     let nonIncludedNACountries = Set.difference naCountriesLookup loadedCountries1
//     let uniqueCountriesLoaded = Set.union loadedCountries1 loadedCountries2
//     let isUSAIncluded = Set.contains "USA" naCountriesLookup // naCountriesLookup.Contains "USA"
//     //depending on the record field declaration order, the comparable can change
//     //Age first(int comparison) vs Name first(string comparison)
    
    
    
//     type Chicken = {
//         Age: int
//         Name: string
//     }
//     let chickenSet = set [
//             {
//                 Name = "Jim"
//                 Age = 50
//             }
//             {
//                 Name = "Kim"
//                 Age = 20
//             }
//         ]
//     let tupleSet = set [(1,"z");(2,"a")]
//     let tupleSet2 = set [("z",1);("a",2)]
//     tupleSet.MinimumElement
//     tupleSet2.MinimumElement


//     //Maps and Dictionaries
//     let largeArray = Array.init 1_000_000 (fun i -> $"{i}", i)
//     let largeArrayMap = Map.ofArray largeArray
//     largeArrayMap.["750000"]
//     largeArray |> Array.find(fun (key, value) -> key = "750000")
//     let employees = [|"John"; "Bill"; "Mary"|]
//     let salaries = [|50000; 35000; 65000|]
//     let employeeSalaryPairs = Array.zip employees salaries
//     let employees2, salaries2 = Array.unzip employeeSalaryPairs
//     let employeeSalaryMap = employeeSalaryPairs |> Map.ofArray
//     let dictArgument = (employeeSalaryPairs |> dict) //could also use readOnlyDict for not mutable dictionary
//     let employeeSalaryDictionary = Dictionary<string,int>dictArgument
//     employeeSalaryMap["Mary"]
//     employeeSalaryDictionary["Mary"]
//     employeeSalaryMap["Lisa"]
//     employeeSalaryDictionary["Lisa"]


//     let salaryLookupMap (salaryMap: Map<string,int>) (name: string) =
//         let result = salaryMap.TryFind name
//         match result with
//         | Some salary -> salary
//         | None -> failwith $"Salary data for employee {name} does not exist."
//     let salaryLookupDict (salaryDict: Dictionary<string,int>) (name: string) =
//         match salaryDict.TryGetValue name with
//         | true, salary -> salary
//         | false, _ -> failwith $"Salary data for employee {name} does not exist."


//     salaryLookupMap employeeSalaryMap "Lisa"
//     salaryLookupDict employeeSalaryDictionary "Lisa"


//     let updatedEmployeeSalaryMap = employeeSalaryMap.Add ("Lisa", 75000)
//     employeeSalaryDictionary.Add ("Lisa", 75000)
            