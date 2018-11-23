open System.IO
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Giraffe
open Saturn
open Elmish
open Elmish.Bridge
open Shared

let publicPath = Path.GetFullPath "../Client/public"
let port = 8085us

let getInitCounter() : Task<Counter> = task { return { Value = 42 } }

let init dispatch () =
    { Value = 1 }, Cmd.none

let update dispatch msg currentModel =
    match msg with
    | Decrement ->
        dispatch DecrementServer
        currentModel, Cmd.none
    | _ ->
        currentModel, Cmd.none

let server = Bridge.mkServer Shared.endpoint init update
                |> Bridge.run Giraffe.server

let web = router {
    get "/api/init" (fun next ctx ->
        task {
            let! counter = getInitCounter()
            return! Successful.OK counter next ctx
        })
}

let webApp =
    choose [
        server
        web
    ]

let configureSerialization (services:IServiceCollection) =
    services.AddSingleton<Giraffe.Serialization.Json.IJsonSerializer>(Thoth.Json.Giraffe.ThothSerializer())

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router webApp
    memory_cache
    use_static publicPath
    service_config configureSerialization
    app_config Giraffe.useWebSockets
    use_gzip
}

run app
