#load "age.fsx"
#load "covers.fsx"

open Age
open Covers 

type Event =
| InForce of (Premium * DefaultCover seq)
| PaidUp of DefaultCover seq
| Surrendered of PaidOut
| Disabled of (DisabledCover seq * DefaultCover seq)
| Reactivated of (Premium * DefaultCover seq)
| Retired of RetiredCover
| Dead of PaidOut

type PolicyNumber = PolicyNumber of string

type Policy = {
    PolicyNumber: PolicyNumber;
    Birthday: Birthday;
    Events: Event seq;
}

let addEvent (events: Event seq) event =
    Ok (events |> Seq.append [event])
