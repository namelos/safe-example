module Client

open Elmish
open Elmish.React
open Elmish.Bridge
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch
open Thoth.Json
open Shared
open Fulma

let init () =
    { Value = 1 }, Cmd.none

let update msg state =
    match msg with
    | Increment ->
        { state with Value = state.Value + 1 }, Cmd.none
    | Decrement ->
        Bridge.Send(Decrement)
        state, Cmd.none
    | DecrementServer ->
        { state with Value = state.Value - 1 }, Cmd.none

let view (model: Counter) (dispatch: Msg -> unit) =
    div [] [
        p [] [str (model.Value.ToString())]
        button [OnClick (fun _ -> dispatch Decrement)] [str "-"]
        button [OnClick (fun _ -> dispatch Increment)] [str "+"]
        ]

#if DEBUG
//open Elmish.Debug
//open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
//|> Program.withHMR
#endif
|> Program.withBridge Shared.endpoint
|> Program.withReact "elmish-app"
#if DEBUG
//|> Program.withDebugger
#endif
|> Program.run
