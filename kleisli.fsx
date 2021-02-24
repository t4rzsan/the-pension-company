type Optional =
    static member Compose a b x =
        match a x with
        | Ok v -> b v
        | Error v  -> Error v

    static member Identity x =
        Ok(x)

let tryCatch f x =
    try
        f x |> Ok
    with
    | ex -> Error ex.Message

let unit f x =
    match f x with
    | Ok _ -> Ok()
    | Error s -> Error s
