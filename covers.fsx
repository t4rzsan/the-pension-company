#load "age.fsx"

open Age

type Benefit = Benefit of decimal
type Premium = Premium of decimal
type PaidOut = PaidOut of decimal

type DefaultBasicCover =
| G165 of (Expiry * Guarantee)
| G415 of Expiry
| G211 of Expiry

type RetiredBasicCover =
| G210

type DisabledBasicCover =
| G215 of Expiry

type DefaultCover = {
    Benefit: Benefit;
    BasicCover: DefaultBasicCover;
}

type RetiredCover = {
    Benefit: Benefit;
    BasicCover: RetiredBasicCover;
}

type DisabledCover = {
    Benefit: Benefit;
    BasicCover: DisabledBasicCover;
}
