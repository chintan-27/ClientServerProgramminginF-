namespace clientserver

open System
open System.Net
open System.Net.Sockets
open System.IO
open System.Text
open System.Collections.Generic
open exceptions
open operations
open client

module server = 

    // Define the port on which the server will listen
    let port = 12345

    // Create mutable dictionaries to store client names and client TCP connections
    let mutable clientNames: Dictionary<string,string> = new Dictionary<string, string>()
    let mutable clientIPs: Dictionary<string, TcpClient> = new Dictionary<string, TcpClient>()

    // Function to handle incoming messages from clients
    let handleString (message: string, clientSocket: TcpClient) =
        // Split the message into words
        let words = message.Split ' '
        let stream = clientSocket.GetStream()

        // Check if the message contains the command to set the client name
        if words.[0] = "SETNAME" then
            let name = String.Join(" ", words.[1..])
            let clientIP = (clientSocket.Client.RemoteEndPoint.ToString())
            clientNames.[(clientIP)] <- name
            Console.WriteLine(sprintf "Client %s set its name to: %s" (clientIP.ToString()) name)
            // Return 1 to indicate a name setting operation and the new name
            1, [|name|]
        else
            // Find the client name (or use the endpoint as a fallback)
            let clientIP = (clientSocket.Client.RemoteEndPoint.ToString())
            let clientName = 
                match clientNames.TryGetValue (clientIP) with
                | true, name -> name
                | _ -> (clientSocket.Client.RemoteEndPoint.ToString())

            // Print the message with the client's name
            printfn "Client %s said: %s" clientName message
            if words.Length >= 2 && words.[0] <> "SETNAME" then
                if exceptions.checkErrorCodes words < 0 then
                    // If there's an error, return the error code and client name
                    exceptions.checkErrorCodes words, [|clientName; "Error Code"|]
                else
                    // Perform operations on the message and return the result with client name
                    let res = operations.operate words
                    res, [|clientName; "Answer"|]
            else
                if words.Length = 0 then
                    // Handle cases where the message is empty
                    -1, [|clientName; "Error Code"|]
                else if words.Length = 1 then
                    if words.[0] = "bye" then
                        // Handle client termination request
                        -5, [|clientName; "Bye"|]
                    else if words.[0] = "terminate" then
                        // Handle server termination request
                        -5, [|clientName; "Terminate"|]
                    else 
                        -1, [|clientName; "Error Code"|]
                else 
                    0, [|clientName; ""|]

    // Function to terminate all clients
    let terminateAll(name: string) = 
        let clientIPs = new Dictionary<IPAddress, TcpClient>()
        let clientNames = new Dictionary<IPAddress, string>()
        0

    // Function to handle a specific client's communication
    let handleClient (clientSocket: TcpClient, listener: TcpListener) =
        let stream = clientSocket.GetStream()
        let reader = new StreamReader(stream)
        let writer = new StreamWriter(stream)

        // Add the client's IP and socket to dictionaries
        clientIPs.[(clientSocket.Client.RemoteEndPoint.ToString())] <- clientSocket

        let mutable running = true

        try
            while running do
                let message = reader.ReadLine()
                
                if String.IsNullOrWhiteSpace(message) then
                    printf ""
                else if message = "Hello!" then
                    // Respond to initial client connection with a welcome message
                    writer.WriteLine(sprintf "Hello and Welcome! Let's start with your name first.")
                    writer.Flush()
                else
                    let status, response = handleString (message.Trim(), clientSocket)
                    if status = 1 then 
                        // Handle successful name setting
                        printfn "Connect to Server %s Successfully" (clientSocket.Client.RemoteEndPoint.ToString())
                        writer.WriteLine(sprintf "Name set: %s" response.[0])
                        printfn "Writing to Client %s: \"Name set: %s\"" response.[0] response.[0]

                    else if status < 0 && response.[1] = "Error Code" then
                        // Handle error responses
                        writer.WriteLine(sprintf "Error Code: %d" status)
                        printfn "Writing to Client %s: \"Error Code: %d\"" response.[0] status

                    else if status = -5 && response.[1] = "Bye" then
                        // Handle client termination
                        let temp = clientNames.Remove((clientSocket.Client.RemoteEndPoint.ToString()))
                        let temp = clientIPs.Remove((clientSocket.Client.RemoteEndPoint.ToString()))
                        writer.WriteLine(sprintf "Exit Code: %d" status)
                        writer.WriteLine(sprintf "Bye! It was great working with you.")
                        printfn "Exiting Gracefully and bidding farewell to client %s" response.[0]

                    else if status = -5 && response.[1] = "Terminate" then
                        // Handle server termination
                        let temp = terminateAll (response.[0])
                        writer.WriteLine(sprintf "Exit Code: %d" status)
                        writer.WriteLine(sprintf "Bye! It was great working with you.")
                        printfn "Bidding farewell to all the clients"
                        printfn "Adios!"
                        running <- false
                        listener.Stop()

                    else if response.[1] = "Answer" then
                        // Handle responses with answers
                        writer.WriteLine(sprintf "Answer: %d" status)
                        printfn "Writing to Client %s: \"Answer: %d\"" response.[0] status

                    writer.Flush()
        with
        | :? IOException -> printf ""

    // Function to start the server and handle incoming client connections
    let startServer () =
        let listener = new TcpListener(IPAddress.Loopback, port)
        listener.Start()
        printfn "Server started on port %d" port

        // Function to handle multiple clients concurrently
        let rec handleClients () =
            try
                while true do
                    let client = listener.AcceptTcpClient()
                    Async.Start (async { return handleClient (client, listener)})
            with
            | :? SocketException -> printfn "Server stopped"

        // Start handling clients
        handleClients ()
