namespace clientserver
open System

module operations =

    // Function to perform addition on a list of integers
    let add (words: string[]) = 
        let mutable result = 0
        for word in words do
            result <- result + (word |> int)
        result

    // Function to perform subtraction on a list of integers
    let subtract (words: string[]) = 
        let mutable result = words.[0] |> int
        for word in words[1..] do
            result <- result - (word |> int)
        result
    
    // Function to perform multiplication on a list of integers
    let multiply (words: string[]) = 
        let mutable result = 1
        for word in words do
            result <- result * (word |> int)
        result
    
    // Function to select and perform the appropriate operation based on the input
    let operate (words: string[]) = 
        let mutable result = 0
        if words.[0] = "add" then
            result <- add words[1..]
        else if words.[0] = "sub" then
            result <- subtract words[1..]
        else if words.[0] = "mul" then
            result <- multiply words[1..]
        result
