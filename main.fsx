module Age = 
    type Age = private Age of int
    type Expiry1 = Expiry1 of Age
    type Guarantee = 
    | Y10
    | Y15
    | Y20

    let create months =
        if months < 0 || months > 110 * 12 then
            failwith "Age has to be between 0 and 110 years."

        (Age months)
    let value (Age age) = age

open Age

module Covers =
    type BasicCover =
    | G165 of (Expiry1 * Guarantee)
    | G415 of Expiry1
    | G211 of Expiry1
    | G210
    | G215 of Expiry1

    type Cover = {
        Benefit: decimal;
        BasicCover: BasicCover;
    }
