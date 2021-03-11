# the-pension-company

## Design
* Selskabet tilbyder livrente (211), rate ved død (175) og invalidepension (415).
* En police kan være i kraft, fripolice, genkøbt, invalid, reaktiveret, pensioneret eller død.
* En police er defineret som en række af hændelser svarende til ovenstående liste af tilstande.  Ikke alle overgange er tilladte.  F.eks. kan man ikke gå fra død til reaktiveret.
* Den sidste hændelse i rækken definerer policens nuværende tilstand.

## Undtagelser

* Vi implementerer ikke persistence til en database.
* Vi implementerer ikke kontofremregning.
* Vi implementerer ikke historik med frem- og tilbagerulning.

Sådanne små detaljer overlader vi til læseren som en øvelse.
