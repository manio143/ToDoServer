namespace ToDoServer

module Main =

    open System
    open Suave
    open Suave.Filters
    open Suave.Operators
    open Suave.Successful
    open Suave.RequestErrors
    open Newtonsoft.Json

    [<EntryPoint>]
    let main argv =

        let tasks = new System.Collections.Generic.List<string>()

        let app =
          choose
            [ GET >=> choose
                [ path "/" >=> OK "Hello World!"
                  path "/all" >=> request (fun httpReq -> OK (JsonConvert.SerializeObject(tasks))) ]
              POST >=> path "/task" >=> request (fun httpReq -> 
                                                    tasks.Add(System.Text.Encoding.UTF8.GetString(httpReq.rawForm));
                                                    NO_CONTENT) 
              DELETE >=> pathScan "/task/%s" (fun task -> if tasks.Remove(task) then NO_CONTENT else NOT_FOUND "No task to remove")
              NOT_FOUND "Invalid request."
            ]

        let config = 
            { defaultConfig with
               bindings = [ HttpBinding.createSimple HTTP "0.0.0.0" 80 ]
            }

        startWebServer defaultConfig app
        0
        