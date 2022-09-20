namespace Gaming

open System

type Gaming = 
    {
        Name: string
        Cost: int
    }



module Gaming = 
    let consoles = 
        [
            {Name = "Xbox S"; Cost = 300}
            {Name = "Xbox X"; Cost = 500}
            {Name = "Switch OLED"; Cost = 250}
            {Name = "PS5"; Cost = 550}
            {Name = "PS5 Disc"; Cost = 400}

        ]

    let totalSum = List.sumBy ( fun item -> item.Cost) consoles

    //consoles.[0].Cost




    //|> List. ( fun item -> item.Name = "PS5") consoles 

    



    








