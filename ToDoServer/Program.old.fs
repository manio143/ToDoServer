namespace ToDoServer

open System

module Main =

    type Storage() as this =
        
        static member val tasks = new System.Collections.Generic.List<string>()

    type Home() =
        inherit Nancy.NancyModule()

        do this.Init()

        member this.body () = base.Request.Body.ToString()

        member this.Init() =
            do base.Get.["/"] <- new System.Func<Object, Object>(fun _ -> "Hello World!" :> Object)
            do base.Get.["/all"] <- fun _ -> Newtonsoft.Json.JsonConvert.SerializeObject(Storage.tasks) :> Object
            do base.Post.["/task"] <- fun _ -> Storage.tasks.Add(this.body()) |> ignore; "Ok" :> Object


    [<EntryPoint>]
    let main argv = 
        let nancyHost = 
            new Nancy.Hosting.Self.NancyHost(new Uri("http://localhost:8080/"))
        do nancyHost.Start()

        Console.WriteLine("Nancy now listening on http://localhost:8080/. \nPress enter to stop...")
        Console.ReadKey() |> ignore

        nancyHost.Stop()

        Console.WriteLine("Stopped. Good bye!")

        0