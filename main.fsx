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
        RetiredBasicCover: DisabledBasicCover;
    }

    type Benefit = Benefit of decimal
    type PaidOut = PaidOut of decimal

open Covers 

type Event =
| InForce of DefaultCover seq
| PaidUp of DefaultCover seq
| Surrendered of PaidOut
| Disabled of (DisabledCover * (DefaultCover seq))
| Retired of RetiredCover
| Dead of PaidOut

let ev = InForce([
    { Benefit = 1000m; BasicCover = (create (65 * 12) |> Expiry1 |> G211) }
])

type Policy = {
    Birthday: Birthday;
    Events: Event seq;
}

type System = {
    Policies: Policy seq;
}