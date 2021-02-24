#load "age.fsx"

open Age

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
