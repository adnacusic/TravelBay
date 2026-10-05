# TravelBay — dokumentacija sistema preporuke

> Ovaj dokument je izvor istine za `RecommendationService`. Implementacija prati tačno ono što
> piše ovdje (formula, težine, normalizacija, pravila, tekst objašnjenja). Sve numeričke
> konstante žive na jednom mjestu u kodu — `RecommenderSettings` (vidi §3.5).

## 1. Pristup

**Content-based filtering + popularity**, bez treniranja modela. Svaka destinacija dobija
**score u rasponu 0–1** kao težinsku sumu tri signala, a korisniku se vraćaju top-N
destinacije sortirane po tom scoreu, svaka sa `MatchPercent` i tekstom `Explanation`.

Zašto ne collaborative filtering: aplikacija ima malo korisnika i malo interakcija, pa bi matrica
korisnik–destinacija bila preslaba da bi "slični korisnici" imali smisla; osim toga collaborative
pristup traži treniranje/faktorizaciju i teško ga je objasniti korisniku. Content-based sa
signalima koje korisnik direktno kontroliše (preference) ili ostavlja kroz ponašanje (pregledi)
radi od prvog dana i svaka preporuka se može objasniti jednom rečenicom.

Scoring se računa **u trenutku zahtjeva** za trenutnog korisnika (`userId` isključivo iz JWT-a);
ništa se ne predračunava niti pohranjuje po korisniku, pa svaka promjena preferencija, historije,
recenzija ili sačuvanih destinacija odmah mijenja rezultat.

## 2. Signali i izvor svakog

Svi signali dolaze iz podataka koji se stvarno upisuju u bazu tokom korištenja aplikacije; nijedan
signal se ne skuplja a da se ne koristi.

| Signal | Izvor (tabela) | Ko ga puni | Kako se dobija |
|---|---|---|---|
| **Eksplicitna preferenca** `pref(c)` | `UserPreference` | korisnik (`PUT /UserPreferences`) | `1` ako je kategorija `c` među korisnikovim preferiranim, inače `0` |
| **Ponašanje** `behavior(c)` | `ViewHistory` | aplikacija pri otvaranju detalja (`POST /ViewHistories`) | broj korisnikovih pregleda destinacija kategorije `c`, podijeljen s brojem pregleda u njegovoj najgledanijoj kategoriji (raspon 0–1) |
| **Ocjena** `rating(d)` | `Review` | korisnici + moderacija | prosječna ocjena destinacije iz **samo `Approved`** recenzija (nikad uskladištena kolona), zaglađena i normalizovana — vidi 3.3 |
| **Popularnost** `pop(d)` | `ViewHistory` + `Review` | svi korisnici | **broj jedinstvenih posjetilaca** (različitih korisnika iz `ViewHistory`) **+ broj `Approved` recenzija**, log-skalirano i min–max normalizovano — vidi 3.4 |

## 3. Formula

### 3.1 Ukupni score

```
score(d) = 0.5 · category(d) + 0.3 · rating(d) + 0.2 · popularity(d)        // svi članovi u [0, 1]
MatchPercent(d) = round(score(d) · 100), zaokruživanje "away from zero", ograničeno na [0, 100]
```

**Težine 0.5 / 0.3 / 0.2 i obrazloženje:**
- **0.5 category** — to je jedini signal koji govori o *ovom* korisniku (šta voli i šta gleda).
  Ocjena i popularnost su isti za sve korisnike, pa ako bi category imao manju težinu, preporuke
  bi se razlikovale među korisnicima tek marginalno i sistem bi bio "top lista" s nalijepljenim
  imenom.
- **0.3 rating** — kvalitet je najjači objektivni pokazatelj da će se destinacija svidjeti, ali
  broj recenzija je malen, pa ga ne dižemo iznad personalizacije.
- **0.2 popularity** — služi kao tiebreaker i za cold-start; ne smije nadjačati ni kvalitet ni
  korisnikove interese (mnogo posjetilaca ≠ dobra destinacija za mene).

### 3.2 Category signal (preference + ponašanje)

Za destinaciju `d` u kategoriji `c`, prilagodljivo prema tome šta o korisniku znamo:

| Korisnik ima | `category(d)` |
|---|---|
| preference **i** historiju | `0.7 · pref(c) + 0.3 · behavior(c)` |
| samo preference | `pref(c)` |
| samo historiju | `behavior(c)` |
| ništa | cold-start, vidi 5 |

Ponašanje tako **dodatno podiže** kategorije koje korisnik gleda: ako preferira "Historiju" a
često gleda "Plaže", Plaže dobijaju do `0.3` category scorea umjesto `0`, a Historija ostaje na
`0.7 + 0.3 · behavior`. Gledanje stvarno mijenja redoslijed, a eksplicitna preferenca i dalje
ostaje dominantna (`0.7`).

`behavior(c) = views(user, c) / max_k views(user, k)` — normalizacija na korisnikovu najgledaniju
kategoriju, pa je raspon uvijek 0–1 bez obzira na to koliko je korisnik aktivan. Ovdje se broje
svi korisnikovi pregledi (redovi `ViewHistory`), jer je to njegov lični signal interesa.

### 3.3 Rating signal

Sirova prosječna ocjena s jednom recenzijom (5★) ne smije pobijediti destinaciju s 30 recenzija
(4.7★), zato se koristi **Bayesovo zaglađivanje**:

```
smoothed(d) = (n · avg(d) + m · G) / (n + m)        n = broj Approved recenzija, avg = njihova prosječna ocjena
rating(d)   = (smoothed(d) − 1) / 4                 // ocjene 1–5 → 0–1
```

- `m = 3` (težina "prethodnog mišljenja": tri recenzije su potrebne da destinacija počne odudarati
  od prosjeka).
- `G` = globalna prosječna ocjena svih `Approved` recenzija (ako ih nema ni jedne, `G = 3.0`).
- Destinacija bez recenzija (`n = 0`) dobija `smoothed = G` — neutralnu, ne kaznenu, vrijednost.

### 3.4 Popularity signal

```
raw(d) = uniqueVisitors(d) + approvedReviews(d)
pop(d) = (ln(1 + raw(d)) − ln(1 + rawMin)) / (ln(1 + rawMax) − ln(1 + rawMin))     // [0, 1]
pop(d) = 0   ako je rawMax == rawMin
```

- `uniqueVisitors(d)` = **broj jedinstvenih posjetilaca** destinacije, tj. broj *različitih
  korisnika* koji su je barem jednom pogledali (`COUNT(DISTINCT UserId)` u `ViewHistory`), a ne
  sirov broj redova — jedan korisnik osvježavanjem stranice ne može napumpati popularnost.
- `approvedReviews(d)` = broj `Approved` recenzija destinacije.
- `ln(1 + x)` ublažava dugi rep: destinacija sa raw = 100 nije "10× popularnija" od one sa raw = 10.
- `rawMin`/`rawMax` se računaju nad svim ne-obrisanim destinacijama; vrijednost `pop` se uvijek
  ograničava na [0, 1].

### 3.5 Imenovane konstante (`RecommenderSettings`)

Nijedan broj iz ovog dokumenta nije razbacan po kodu; svi su imenovane konstante u jednoj klasi.

| Konstanta | Vrijednost | Značenje |
|---|---|---|
| `CategoryWeight` | 0.5 | težina category signala |
| `RatingWeight` | 0.3 | težina rating signala |
| `PopularityWeight` | 0.2 | težina popularity signala |
| `PreferenceShare` | 0.7 | udio preference u category signalu (kad postoje i preference i historija) |
| `BehaviorShare` | 0.3 | udio ponašanja u category signalu (kad postoje i preference i historija) |
| `RatingPriorWeight` | 3 | `m` u Bayesovom zaglađivanju |
| `DefaultGlobalAverageRating` | 3.0 | `G` kad nema nijedne `Approved` recenzije |
| `MinRating` / `MaxRating` | 1 / 5 | granice ocjene za normalizaciju |
| `HighRatingThreshold` | 4.0 | prag sirove prosječne ocjene za frazu "visoko ocijenjena" |
| `HighPopularityThreshold` | 0.6 | prag `pop` za frazu "popularna među putnicima" |
| `FrequentViewThreshold` | 0.5 | prag `behavior` za frazu "često pregledaš" |
| `MaxCandidates` | 200 | najviše kandidata koji ulaze u scoring |
| `DefaultPageSize` / `MaxPageSize` | 10 / 50 | veličina stranice |
| `StatsCacheMinutes` | 5 | TTL keša globalnih vrijednosti (`rawMin`, `rawMax`, `G`) |

## 4. Objašnjive preporuke (`Explanation`)

Tekst se generiše **iz stvarnih vrijednosti i doprinosa** iste formule, ne iz fraza koje ne zavise
od scorea. Fraza se pojavljuje samo ako je njena tvrdnja istinita za tu destinaciju i tog korisnika.

1. **Kategorijska fraza** (najviše jedna), samo ako korisnik ima preference ili historiju:

| Uslov | Fraza |
|---|---|
| `pref(c) = 1` i `behavior(c) ≥ 0.5` | `voliš i često pregledaš kategoriju „{Kategorija}“` |
| `pref(c) = 1` | `voliš kategoriju „{Kategorija}“` |
| `behavior(c) ≥ 0.5` | `često pregledaš kategoriju „{Kategorija}“` |

2. **Atributi destinacije** (nula do dva), poredani po stvarnom doprinosu scoreu opadajuće
   (`doprinos_rating = wRating · rating(d)`, `doprinos_pop = wPop · pop(d)`):

| Uslov | Atribut |
|---|---|
| `n ≥ 1` i sirova prosječna ocjena `avg ≥ 4.0` | `visoko ocijenjena ({avg:0.0}★)` |
| `pop ≥ 0.6` | `popularna među putnicima` |

3. **Sastavljanje rečenice:**

| Šta postoji | Rečenica |
|---|---|
| kategorijska fraza + atributi | `Preporučeno jer {kategorijska fraza}, a destinacija je {atribut1} i {atribut2}.` |
| samo kategorijska fraza | `Preporučeno jer {kategorijska fraza}.` |
| samo atributi | `Preporučeno jer je destinacija {atribut1} i {atribut2}.` |
| ništa | `Preporučeno na osnovu ocjena i popularnosti.` |

(Za jedan atribut nema veznika `i`.) Ocjena se piše s tačkom kao decimalnim separatorom.

Primjer: `Preporučeno jer voliš kategoriju „Historija“, a destinacija je visoko ocijenjena (4.5★) i popularna među putnicima.`

## 5. Cold-start

Korisnik **bez preferencija i bez historije** nema šta da se poklapa po kategoriji, pa se category
član izbacuje, a njegova težina se proporcionalno prepušta ostalima:

```
score(d) = (0.3 · rating(d) + 0.2 · popularity(d)) / 0.5  =  0.6 · rating(d) + 0.4 · popularity(d)
```

`MatchPercent` se i dalje računa kao `round(score · 100)` (kao i za ostale korisnike) i tada je
mjera kvaliteta i popularnosti. Objašnjenje je iskreno: pošto korisnik nema ni preferencija ni
historije, kategorijska fraza se **nikad** ne generiše (nema "jer voliš X"), a rečenica sadrži samo
atribute koji stvarno vrijede, npr. `Preporučeno jer je destinacija popularna među putnicima i
visoko ocijenjena (4.5★).` — ili, ako nijedan prag nije zadovoljen,
`Preporučeno na osnovu ocjena i popularnosti.` Čim korisnik postavi preference ili pogleda prvu
destinaciju, prelazi na personalizovanu formulu — bez posebnog "prelaza".

## 6. Pravila i edge-case ponašanja

- **Isključuju se samo** destinacije koje je korisnik već **sačuvao** (`SavedDestination`) — poznate su mu.
- Pregledane, a nesačuvane destinacije **ostaju** u preporukama (korisnik ih je vidio, ali nije
  pokazao da ih "ima"); njihov pregled već podiže kategoriju.
- Soft-obrisane destinacije (`IsDeleted`) ne ulaze (global query filter), kao ni destinacije u
  kategoriji sa `IsActive = false`.
- Samo **`Approved`** recenzije ulaze u rating i popularnost; `Pending`/`Rejected` nikad.
- Remis u scoreu rješava se po `Id` rastuće (deterministički redoslijed među stranicama).
- Ako nakon isključivanja nema kandidata, vraća se prazna lista (200 OK, `items: []`).
- Zahtjev bez validnog JWT-a → 401; `userId` se ni iz čega drugog ne čita.

## 7. Implementacija (performanse)

- Jedan upit kandidata sa **agregacijama u SQL-u** (`COUNT`, `SUM`, `COUNT(DISTINCT)` kao
  korelisani poduupiti nad `Review` i `ViewHistory`) — nema N+1 i ne učitava se cijela baza po
  zahtjevu. Isključivanje sačuvanih je `NOT EXISTS` u istom upitu.
- Kandidati: ne-obrisane destinacije u aktivnim kategorijama, bez sačuvanih, sortirane po
  `raw` popularnosti opadajuće i ograničene na `MaxCandidates` (200); scoring i sortiranje
  nad tih ≤ 200 lagih redova radi se u memoriji. Pun `DestinationResponse` (slike, kategorija) se
  učitava jednim upitom **samo za destinacije na traženoj stranici**.
- Korisnikove preference i pregledi po kategoriji čitaju se jednim upitom po signalu
  (`GROUP BY CategoryId`).
- `IMemoryCache` (TTL `StatsCacheMinutes`) čuva samo globalne vrijednosti koje su iste za sve
  korisnike: `rawMin`, `rawMax`, `G`. Korisnikovi signali i `raw` po destinaciji se **ne keširaju**,
  pa promjena preferencija/pregleda djeluje odmah; granice normalizacije mogu kasniti najviše 5 minuta.

## 8. API ugovor

`GET /Recommendations?Page=1&PageSize=10` — `[Authorize]`, current-user.
`PageSize` default 10; vrijednosti veće od 50 svode se na 50. Odgovor
(`PageResult<RecommendationResponse>`; `totalCount` je broj rangiranih kandidata, najviše 200):

```json
{
  "items": [
    {
      "destination": { "id": 1, "name": "Stari Most", "categoryId": 3, "...": "..." },
      "matchPercent": 88,
      "explanation": "Preporučeno jer voliš kategoriju „Historija“, a destinacija je visoko ocijenjena (4.5★) i popularna među putnicima."
    }
  ],
  "totalCount": 23
}
```

`destination` je isti `DestinationResponse` kao u `GET /Destinations/{id}` (kategorija, slike,
`averageRating`/`reviewCount` iz `Approved` recenzija).

Pregledi se bilježe preko postojećeg `POST /ViewHistories` (Faza 2) i taj isti `ViewHistory`
zapis koristi recommender.

## 9. Ilustrativan proračun

Korisnik preferira kategoriju „Historija“, nema historiju. Destinacija „Stari Most“ (Historija) ima
2 `Approved` recenzije (5★ i 4★ → `avg = 4.5`); neka je (ilustrativno) `G = 4.3`, `rawMin = 2`,
`rawMax = 20`, `raw(Stari Most) = 9`.

```
category = pref = 1.0                                   (samo preference)
smoothed = (2·4.5 + 3·4.3) / 5 = 4.38  →  rating = (4.38 − 1)/4 = 0.845
pop      = (ln 10 − ln 3) / (ln 21 − ln 3) = 0.619
score    = 0.5·1.0 + 0.3·0.845 + 0.2·0.619 = 0.877   →   MatchPercent = 88
```

Doprinosi: category 0.500, rating 0.254, popularity 0.124; oba praga su zadovoljena
(`avg 4.5 ≥ 4.0`, `pop 0.619 ≥ 0.6`) pa objašnjenje ima kategorijsku frazu i oba atributa (§4, primjer).

## 10. Potvrđene odluke i ograničenja

Potvrđeno (vlasnik projekta):
1. Popularnost = broj **jedinstvenih posjetilaca** (različiti korisnici iz `ViewHistory`) + broj
   `Approved` recenzija.
2. Težine 0.5 / 0.3 / 0.2 i sve unutrašnje konstante (`0.7/0.3`, `m = 3`, pragovi `4.0` i `0.6`)
   ostaju kako su; žive kao imenovane konstante u `RecommenderSettings`.
3. Cold-start zadržava `MatchPercent`, a objašnjenje je iskreno (bez "jer voliš X").
4. Isključuju se samo sačuvane destinacije; pregledane ostaju.

Poznata ograničenja: svi pregledi se računaju jednako bez obzira na starost (nema vremenskog
opadanja); nema diverzifikacije liste (nekoliko istih kategorija može biti zaredom); popularnost
je globalna, ne lokalna za region korisnika; `totalCount` je ograničen na `MaxCandidates`.
