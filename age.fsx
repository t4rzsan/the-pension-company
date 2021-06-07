open System

type Age = private | Age' of months: int
    with
        member me.Months = 
            let (Age' (months = value)) = me in value

        static member Of months =
            if months < 0 || months > 120 * 12 then
                None
            else
                Some (Age' months)

type Expiry = Expiry of Age option

type Guarantee = 
| Y10
| Y15
| Y20

type Birthday = Birthday of DateTime
