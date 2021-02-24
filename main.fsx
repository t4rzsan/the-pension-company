#load "kleisli.fsx"
#load "age.fsx"
#load "covers.fsx"
#load "events.fsx"

open Kleisli
open Covers
open Events

let (>=>) x y = Optional.Compose x y

let createEventInForce previousEvent newPremium =
    match previousEvent with
    | InForce _ -> Ok previousEvent
    | PaidUp covers -> Ok (InForce (newPremium, covers))
    | Surrendered _ -> Error ("Surrendered cannot be changed to in force.")
    | Disabled _ -> Error ("Disabled cannot be changed to in force.")
    | Reactivated (_, covers) -> Ok (Reactivated (newPremium, covers))
    | Retired _ -> Error ("Retired cannot be changed to in force.")
    | Dead _ -> Error ("Dead cannot be changed to in force.")

let getDisabledCovers (covers: DefaultCover seq) =
    covers 
    |> Seq.choose (fun cover -> 
        match cover.BasicCover with
        | G415 expiry -> Some(cover.Benefit, expiry)
        | _ -> None
    )
    |> Seq.map (fun (benefit, expiry) ->
        { Benefit = benefit; BasicCover = (G215 expiry)}
    )
    |> Ok
    |> Result.bind (fun disabledCovers ->
        if disabledCovers |> Seq.isEmpty then
            Ok (covers, disabledCovers)
        else
            Error ("There are no covers for disability.")
    )

let createEventDisabledFromCovers (covers, disabledCovers) =
    Ok (Disabled (disabledCovers, covers))

let recalculate previousEvent =
    Ok previousEvent

let createEventDisabled previousEvent =
    let getAndCreateDisabledCoversAndEvent = 
        getDisabledCovers 
        >=> createEventDisabledFromCovers
        >=> recalculate
        
    match previousEvent with
    | InForce (_, covers) -> covers |> getAndCreateDisabledCoversAndEvent
    | PaidUp covers -> covers |> getAndCreateDisabledCoversAndEvent
    | Surrendered _ -> Error ("Surrendered cannot be changed to disabled.")
    | Disabled _ -> Error ("Disabled cannot be changed to disabled.")
    | Reactivated (_, covers) -> covers |> getAndCreateDisabledCoversAndEvent
    | Retired _ -> Error ("Retired cannot be changed to disabled.")
    | Dead _ -> Error ("Dead cannot be changed to disabled.")

type System = {
    Policies: Policy seq;
}
