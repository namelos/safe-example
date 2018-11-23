module Shared

open Elmish

type Counter = { Value : int }

type Msg =
| Increment
| Decrement
| DecrementServer

let endpoint = "/socket"

