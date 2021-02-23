#load "kleisli.fsx"

open Kleisli
let (>=>) x y = Kleisli.Optional.Compose x y

module Age = 
    open System

    type Age = private Age of int
    type Expiry1 = Expiry1 of Age
    type Guarantee = 
    | Y10
    | Y15
    | Y20
    type Birthday = Birthday of DateTime

    let create months =
        if months < 0 || months > 110 * 12 then
            failwith "Age has to be between 0 and 110 years."

        (Age months)
    let value (Age age) = age

open Age

module Covers =
    type DefaultBasicCover =
    | G165 of (Expiry1 * Guarantee)
    | G415 of Expiry1
    | G211 of Expiry1

    type RetiredBasicCover =
    | G210

    type DisabledBasicCover =
    | G215 of Expiry1

    type DefaultCover = {
        Benefit: decimal;
        BasicCover: DefaultBasicCover;
    }

    type RetiredCover = {
        Benefit: decimal;
        BasicCover: RetiredBasicCover;
    }

    type DisabledCover = {
        Benefit: decimal;
        BasicCover: DisabledBasicCover;
    }

    type Benefit = Benefit of decimal
    type Premium = Premium of decimal
    type PaidOut = PaidOut of decimal

module Events =
    open Covers 

    type Event =
    | InForce of (Premium * DefaultCover seq)
    | PaidUp of DefaultCover seq
    | Surrendered of PaidOut
    | Disabled of (DisabledCover seq * DefaultCover seq)
    | Reactivated of (Premium * DefaultCover seq)
    | Retired of RetiredCover
    | Dead of PaidOut

    type Policy = {
        Birthday: Birthday;
        Events: Event seq;
    }

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

open Events

type System = {
    Policies: Policy seq;
}