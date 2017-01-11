namespace ToDoServer

module Main =

    open System
    open Suave
    open Suave.Filters
    open Suave.Operators
    open Suave.Successful
    open Suave.RequestErrors
    open Suave.Json

    type Task = {Id: DateTime; Task: string}
              
    let addTask (list:Collections.Generic.List<Task>) =
        let stringFromChoice c = defaultArg (Option.ofChoice c) ""
        let helper (req:HttpRequest) =
            list.Add(
                {Id = Convert.ToDateTime(stringFromChoice(req.formData "id"));
                 Task = stringFromChoice(req.formData "task")}
            )
            NO_CONTENT
        request helper

    let removeTask (list:Collections.Generic.List<Task>) (dateStr:string) =
        let date = Convert.ToDateTime(dateStr)
        let t = list |> Seq.choose (fun x -> if x.Id = date then Some x else None) |> Seq.first
        match t with
        | Some task -> list.Remove(task) |> ignore; NO_CONTENT
        | None -> NOT_FOUND "No task to remove"

    [<EntryPoint>]
    let main argv =

        let tasks = new System.Collections.Generic.List<Task>()

        let app =
          choose
            [ GET >=> choose
                [ path "/" >=> OK "Hello World!"
                  path "/all" >=> request (fun _ -> toJson tasks |> Text.Encoding.UTF8.GetString |> OK) ]
              POST >=> path "/task" >=> addTask tasks
              DELETE >=> pathScan "/task/%s" (removeTask tasks)
              NOT_FOUND "Invalid request."
            ]

        let config = 
            { defaultConfig with
               bindings = [ HttpBinding.createSimple HTTP "0.0.0.0" 80 ]
            }

        match System.Environment.GetCommandLineArgs() |> Seq.tryPick (fun s ->
            if s.StartsWith("port=") then Some(int(s.Substring("port=".Length)))
            else None ) with
        | Some port ->
            let serverConfig =
                { Web.defaultConfig with
                    bindings = [ HttpBinding.createSimple HTTP "127.0.0.1" port ] }
            Web.startWebServer serverConfig app
        | _ -> Web.startWebServer config app
        0
        