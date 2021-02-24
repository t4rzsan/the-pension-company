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

