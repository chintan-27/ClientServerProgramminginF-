namespace clientserver

open System
open System.Net.Sockets
open System.IO
open System.Threading

module client = 

    // Define server address and port
    let serverAddress = "127.0.0.1"
    let port = 12345

    // Create a CancellationTokenSource for managing cancellation
    let cancellationTokenSource = new CancellationTokenSource()

    // Function to connect to the server
    let connectToServer (client: TcpClient) =
        async {
            try
                // Get a NetworkStream from the TcpClient
                let stream: NetworkStream = client.GetStream()
                // Create StreamReader and StreamWriter for reading and writing to the stream
                let reader: StreamReader = new StreamReader(stream)
                let writer: StreamWriter = new StreamWriter(stream)

                // Initialize a flag to control setting the user's name
                let mutable setName = true

                // Main loop for communication with the server
                while not cancellationTokenSource.Token.IsCancellationRequested do
                    try
                        // Prompt the user to set their name if not already set
                        if setName then
                            writer.WriteLine(sprintf "Hello!")
                            writer.Flush()
                            let response = reader.ReadLine()
                            printfn "Server response: %s" response
                            printf "Enter your name: "
                            let name = Console.ReadLine()
                            writer.WriteLine(sprintf "SETNAME %s" name)
                            writer.Flush()
                            let response = reader.ReadLine()
                            printfn "Server response: %s" response
                            setName <- false
                            printfn "Connected to Server"

                    
                        // Prompt the user for a message or to exit
                        printf "Enter a message (or press Enter to exit): " 
                        let message = Console.ReadLine()
                        if String.IsNullOrWhiteSpace(message) then
                            // Do nothing on empty input
                            printf ""
                        else
                            // Send the user's message to the server
                            writer.WriteLine(message)
                            writer.Flush()
                            let response = reader.ReadLine()
                            let reswords = response.Split ":"
                            printfn "Server Response: %s" response
                            // Check if the server requested an exit
                            if reswords.[0] = "Error Code" && reswords.[1] = " -1" then
                                printfn "Incorrect operation command."
                            else if reswords.[0] = "Error Code" && reswords.[1] = " -2" then
                                printfn "Number of inputs in less than two"
                            else if reswords.[0] = "Error Code" && reswords.[1] = " -3" then
                                printfn "Number of inputs in more than four"
                            else if reswords.[0] = "Error Code" && reswords.[1] = " -4" then
                                printfn "One or more of the inputs contains non-numbers"
                            else if reswords.[0] = "Exit Code" && reswords.[1] = " -5" then
                                printfn "Exiting smoothly!"
                                cancellationTokenSource.Cancel()
                    with
                    | :? IOException -> printfn "Sorry! Server is already terminated"
                with
                | :? NullReferenceException -> printfn "Sorry! Server is already terminated" 

        } |> Async.Ignore

    // Function to check server activity
    let checkServerActivity (client: TcpClient) =
        async {
            while not cancellationTokenSource.Token.IsCancellationRequested do
                try
                    // Attempt to ping the server by creating a new TcpClient
                    let pingClient = new TcpClient(serverAddress, port)
                    // Get a NetworkStream and StreamWriter for the pingClient
                    let pingStream: NetworkStream = client.GetStream()
                    let writer: StreamWriter = new StreamWriter(pingStream)
                    // Send an empty message to the server
                    writer.WriteLine(sprintf "")
                    printf ""
                with
                | :? SocketException ->
                    // Handle SocketException if the server is not active
                    printfn ""
                    printfn "Server is not active. Killing threads."
                    printfn "Press Enter to Exit"
                    cancellationTokenSource.Cancel()
                | :? IOException ->
                    // Handle IOException if the server is not active
                    printfn ""
                    printfn "Server is not active. Killing threads."
                    printfn "Press Enter to Exit"
                    cancellationTokenSource.Cancel()
                // Pause the loop for 1 second
                do! Async.Sleep(1000)

        }

    // Main entry point of the client program
    let main () =
        try
            // Create a TcpClient to connect to the server
            let client: TcpClient = new TcpClient(serverAddress, port)
            // Create two threads for running the communication and server activity checking functions
            let t1 = new Thread(ThreadStart(fun () -> Async.RunSynchronously (connectToServer (client))))
            let t2 = new Thread(ThreadStart(fun () -> Async.Start (checkServerActivity (client))))

            // Start the threads
            t1.Start()
            t2.Start()
        with
            | :? SocketException -> printfn "Server Not Active"
