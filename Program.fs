namespace clientserver
open server
open client

module Program =

    // Entry point of the program
    [<EntryPoint>] 
    let main argv =
        // Check the command line arguments to determine if the program should run as a server or client
        
        if argv.[0] = "server" then
            // If the argument is "server," start the server
            server.startServer ()
            0
        else if argv.[0] = "client" then
            // If the argument is "client," start the client program and ignore the result
            client.main() |> ignore
            0
        else
            // If the argument is not recognized, exit with a return code of 0
            0
