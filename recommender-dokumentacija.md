# TravelBay — dokumentacija sistema preporuke

> Ovaj dokument je izvor istine za `RecommendationService`. Implementacija prati tačno ono što
> piše ovdje (formula, težine, normalizacija, pravila, tekst objašnjenja).

## 1. Pristup

**Content-based filtering + popularity**, bez treniranja modela. Svaka destinacija dobija
**score u rasponu 0–1** kao težinska suma tri signala, a korisniku se vraćaju top-N
destinacija sortirane po tom scoreu, svaka sa `MatchPercent` i tekstom `Explanation`.

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
| **Popularnost** `pop(d)` | `ViewHistory` + `Review` | svi korisnici | broj pregleda + broj `Approved` recenzija, log-skalirano i min–max normalizovano — vidi 3.4 |

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
  korisnikove interese (mnogo pregleda ≠ dobra destinacija za mene).

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
kategoriju, pa je raspon uvijek 0–1 bez obzira na to koliko je korisnik aktivan.

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
raw(d)        = viewers(d) + approvedReviews(d)
pop(d)        = (ln(1 + raw(d)) − ln(1 + rawMin)) / (ln(1 + rawMax) − ln(1 + rawMin))     // [0, 1]
pop(d)        = 0   ako je rawMax == rawMin
```

- `viewers(d)` = broj **različitih korisnika** koji su pogledali destinaciju (`COUNT(DISTINCT UserId)`
  u `ViewHistory`), a ne sirov broj redova — da jedan korisnik ne može osvježavanjem stranice
  napumpati popularnost (vidi pitanje 1 u §10).
- `ln(1 + x)` ublažava dugi rep: destinacija sa 100 pregleda nije "10× popularnija" od one sa 10.
- `rawMin`/`rawMax` se računaju nad svim ne-obrisanim destinacijama.

## 4. Objašnjive preporuke (`Explanation`)

Tekst se generiše **iz stvarnih doprinosa** iste formule, ne iz šablona nezavisnog od scorea.

1. Izračunaju se doprinosi: `cat = 0.5·category`, `rat = 0.3·rating`, `pop = 0.2·popularity`.
2. Faktor ulazi u objašnjenje samo ako prelazi prag da bi tvrdnja bila tačna:

| Faktor | Uslov | Fraza |
|---|---|---|
| Kategorija (preferenca) | `pref(c) = 1` | `voliš kategoriju „{Kategorija}“` |
| Kategorija (ponašanje) | `behavior(c) ≥ 0.5` | `često pregledaš kategoriju „{Kategorija}“` |
| Ocjena | `n ≥ 1` **i** sirovi `avg ≥ 4.0` | `visoko je ocijenjeno ({avg:0.0}★)` |
| Popularnost | `pop ≥ 0.6` | `popularno je među putnicima` |

3. Fraze se sortiraju po doprinosu opadajuće i uzimaju se najviše **tri**; spajaju se kao
   `Preporučeno jer {f1}, {f2} i {f3}.` (dvije: `{f1} i {f2}`; jedna: `{f1}`).
   Ako je kategorija zadovoljila i preferencu i ponašanje, to je jedna fraza:
   `voliš i često pregledaš kategoriju „{Kategorija}“`.
4. Ako nijedan faktor ne prelazi prag, koristi se `Preporučeno na osnovu ocjena i popularnosti.`
5. Cold-start: `Popularna destinacija među korisnicima TravelBay-a` (+ `, visoko je ocijenjeno ({avg}★)`
   kad ocjena prelazi prag).

Primjer: `Preporučeno jer voliš kategoriju „Historija“, visoko je ocijenjeno (4.5★) i popularno je među putnicima.`

## 5. Cold-start

Korisnik **bez preferencija i bez historije** nema šta da se poklapa po kategoriji, pa se category
član izbacuje, a njegova težina se proporcionalno prepušta ostalima:

```
score(d) = (0.3 · rating(d) + 0.2 · popularity(d)) / 0.5  =  0.6 · rating(d) + 0.4 · popularity(d)
```

`MatchPercent` se i dalje računa kao `round(score · 100)` ali je tada čisto mjera
kvaliteta/popularnosti; objašnjenje to jasno kaže (tačka 5 iz §4). Čim korisnik postavi
preference ili pogleda prvu destinaciju, prelazi na personalizovanu formulu — bez posebnog "prelaza".

## 6. Pravila i edge-case ponašanja

- **Isključuju se** destinacije koje je korisnik već **sačuvao** (`SavedDestination`) — poznate su mu.
- Soft-obrisane destinacije (`IsDeleted`) ne ulaze (global query filter), kao ni destinacije u
  kategoriji sa `IsActive = false`.
- Pregledane, a nesačuvane destinacije **ostaju** kandidati (korisnik ih je vidio, ali nije
  pokazao da ih "ima"); njihov pregled već podiže kategoriju.
- Samo **`Approved`** recenzije ulaze u rating i popularnost; `Pending`/`Rejected` nikad.
- Remis u scoreu rješava se po `Id` rastuće (deterministički redoslijed među stranicama).
- Ako nakon isključivanja nema kandidata, vraća se prazna lista (200 OK, `items: []`).
- Nevaljane `preference` (obrisana kategorija) ne mogu postojati — FK je `Restrict`.
- Zahtjev bez validnog JWT-a → 401; `userId` se ni iz čega drugog ne čita.

## 7. Implementacija (performanse)

- Jedan upit kandidata sa **agregacijama u SQL-u** (`COUNT`, `AVG`, `COUNT(DISTINCT)` kao
  korelisani poduupiti/`GroupBy` nad `Review`, `ViewHistory`) — nema N+1 i ne učitava se
  cijela baza po zahtjevu.
- Kandidati: ne-obrisane destinacije u aktivnim kategorijama, bez sačuvanih, sortirane po
  `raw` popularnosti opadajuće i ograničene na **200** (`MaxCandidates`); scoring i sortiranje
  nad tih ≤ 200 redova radi se u memoriji.
- Korisnikove preference i pregledi po kategoriji čitaju se jednim upitom po signalu
  (`GROUP BY CategoryId`).
- `IMemoryCache` (TTL **5 min**) čuva samo globalne vrijednosti koje su iste za sve korisnike:
  `rawMin`, `rawMax`, `G`. Korisnikovi signali se **ne keširaju**, pa promjena preferencija/
  pregleda djeluje odmah; popularnost drugih korisnika može kasniti najviše 5 minuta.

## 8. API ugovor

`GET /Recommendations?Page=1&PageSize=10` — `[Authorize]`, current-user.
`PageSize` default 10, maksimum **50**. Odgovor (`PageResult<RecommendationResponse>`):

```json
{
  "items": [
    {
      "destination": { "id": 1, "name": "Stari Most", "categoryId": 3, "...": "..." },
      "matchPercent": 88,
      "explanation": "Preporučeno jer voliš kategoriju „Historija“, visoko je ocijenjeno (4.5★) i popularno je među putnicima."
    }
  ],
  "totalCount": 23
}
```

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

Doprinosi: category 0.500, rating 0.254, popularity 0.124; svi pragovi su zadovoljeni
(`avg 4.5 ≥ 4.0`, `pop 0.619 ≥ 0.6`) pa objašnjenje ima tri fraze (§4, primjer).

## 10. Ograničenja i pitanja za potvrdu

Poznata ograničenja: svi pregledi se računaju jednako bez obzira na starost (nema vremenskog
opadanja); nema diverzifikacije liste (nekoliko istih kategorija može biti zaredom); popularnost
je globalna, ne lokalna za region korisnika.

**Za potvrdu prije implementacije:**
1. **Popularnost = različiti gledaoci, ne sirovi pregledi** (§3.4) — tako popularnost ne može
   napumpati jedan korisnik osvježavanjem. Odstupa od doslovnog "broj pregleda"; potvrdi ili
   preferiraš sirov `COUNT(*)`.
2. **Težine 0.5 / 0.3 / 0.2** i interne podjele (`0.7/0.3` pref/ponašanje, `m = 3`, prag ocjene
   `4.0`, prag popularnosti `0.6`) — spremno za podešavanje.
3. **Cold-start** daje `MatchPercent` iz čiste popularnosti/ocjene (§5) — OK, ili radije ne
   prikazivati postotak za takve korisnike?
4. **Pregledana a nesačuvana** destinacija ostaje u preporukama (§6) — OK?
