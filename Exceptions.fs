namespace clientserver

module exceptions = 
    // Function to check for various error conditions based on the input words
    let checkErrorCodes (words: string[]) = 
        // Check if the first word is a valid operation ('add', 'sub', or 'mul')
        if not (words.[0] = "add" || words.[0] = "sub" || words.[0] = "mul") then
            -1  // Return -1 to indicate an invalid operation code
        
        // Check if there are too few operands (less than 2)
        else if words[1..].Length < 2 then
            -2  // Return -2 to indicate insufficient operands
        
        // Check if there are too many operands (more than 4)
        else if words[1..].Length > 4 then
            -3  // Return -3 to indicate too many operands
        
        else
            let mutable result = 0  // Initialize the result to 0

            // Iterate through the remaining words (operands)
            for word in words.[1..] do
                match System.Int32.TryParse word with
                | false, v -> result <- -4  // If parsing fails, set result to -4 (invalid operand)
                | true, _ -> ()  // If parsing succeeds, do nothing (valid operand)
            
            result  // Return the result, which may indicate an error code or 0 if everything is valid
