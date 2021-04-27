#load "kleisli.fsx"
#load "age.fsx"
#load "covers.fsx"
#load "events.fsx"

open System

open Kleisli
open Age
open Covers
open Events

let (>=>) x y = Optional.Compose x y

let getCovers filter (covers: DefaultCover seq) =
    covers 
    |> Seq.choose filter
    |> Ok
    |> Result.bind (fun disabledCovers ->
        if disabledCovers |> Seq.isEmpty then
            Error ("There are no covers for disability.")
        else
            Ok (covers, disabledCovers)
    )

let getDisabledCovers =
    let filter (cover: DefaultCover) = 
        match cover.BasicCover with
        | G415 expiry -> Some({ Benefit = cover.Benefit; BasicCover = (G215 expiry)})
        | _ -> None

    getCovers filter

let createEventDisabledFromCovers (covers, disabledCovers) =
    Ok (Disabled (disabledCovers, covers))

let createEventDisabled previousEvent =
    let getAndCreateDisabledCoversAndEvent = 
        getDisabledCovers 
        >=> createEventDisabledFromCovers
        
    match previousEvent with
    | InForce (_, covers) -> covers |> getAndCreateDisabledCoversAndEvent
    | PaidUp covers -> covers |> getAndCreateDisabledCoversAndEvent
    | Surrendered _ -> Error ("Surrendered cannot be changed to disabled.")
    | Disabled _ -> Error ("Disabled cannot be changed to disabled.")
    | Reactivated (_, covers) -> covers |> getAndCreateDisabledCoversAndEvent
    | Retired _ -> Error ("Retired cannot be changed to disabled.")
    | Dead _ -> Error ("Dead cannot be changed to disabled.")

let changePolicy eventCreator policy =
    let newEventResult = 
        policy.Events
        |> Seq.last
        |> eventCreator

    match newEventResult with
    | Ok newEvent -> Ok { policy with Events = policy.Events |> (addEvent newEvent) }
    | Error msg -> Error msg

let savePolicy (policy: Policy) =
    Ok policy

let reCalculate (policy: Policy) =
    Ok policy

let getPolicy (policyNumber: PolicyNumber) =
    let covers = [ 
        { DefaultCover.Benefit = 100m; BasicCover = G165 ((Expiry1 (create (65 * 12))), Y10) };
        { Benefit = 100m; BasicCover = G415 ((Expiry1 (create (65 * 12)))) };
        { Benefit = 100m; BasicCover = G211 ((Expiry1 (create (65 * 12)))) };
    ]

    {
        PolicyNumber = policyNumber;
        Birthday = (Birthday (DateTime(1992, 8, 2)));
        Events = [
            InForce ((Premium 1000m), covers |> Seq.ofList)
        ];
    } |> Ok

let workflow change =
    getPolicy
    >=> change
    >=> reCalculate
    >=> savePolicy

let disabilityWorkflow = workflow (changePolicy createEventDisabled) 

match disabilityWorkflow (PolicyNumber "Pol12345") with 
| Ok policy -> printfn "Ok: %A" (policy.Events |> Seq.last)
| Error msg -> printfn "There was an error: %s" msg
