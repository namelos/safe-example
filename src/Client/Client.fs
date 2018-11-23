module Client

open Elmish
open Elmish.React
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch
open Thoth.Json
open Shared
open Fulma

type Msg =
| Increment
| Decrement
| InitialCountLoaded of Result<Counter, exn>

let initialCounter = fetchAs<Counter> "/api/init" (Decode.Auto.generateDecoder())

type Model = { Counter: Counter option }

let init() =
    { Counter = None }, Cmd.ofPromise initialCounter [] (Ok >> InitialCountLoaded) (Error >> InitialCountLoaded)

let update msg currentModel =
    match currentModel.Counter, msg with
    | Some counter, Increment ->
        { currentModel with Counter = Some { Value = counter.Value + 1 } }, Cmd.none
    | Some counter, Decrement ->
        { currentModel with Counter = Some { Value = counter.Value - 1 } }, Cmd.none
    | _, InitialCountLoaded (Ok initialCount) ->
        { Counter = Some initialCount }, Cmd.none
    | _ -> currentModel, Cmd.none

let show model =
    match model with
    | { Counter = Some counter } -> string counter.Value
    | { Counter = None } -> "loading"

let view model (dispatch: Msg -> unit) =
    div [] [
        p [] [str (show model)]
        button [OnClick (fun _ -> dispatch Decrement)] [str "-"]
        button [OnClick (fun _ -> dispatch Increment)] [str "+"]
        ]

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
