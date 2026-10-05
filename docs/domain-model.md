# TravelBay — domenski model (Faza 1, nacrt)

> Ovo je **plan** za potvrdu prije implementacije (Korak 1 Faze 1). Ništa od ovoga još nije
> promijenjeno u kodu. Trenutno stanje baze je ono iz Faze 0 (eCommerce skelet, baza `220042`).

## 0. Trenutno stanje koda (provjereno čitanjem, radi konteksta)

- **CryptoService** (`TravelBay.Common.Services/CryptoService/CryptoService.cs`): hash = PBKDF2
  (`Rfc2898DeriveBytes`, HMAC-SHA256, 10 000 iteracija, 20 bajtova izlaza), salt = 16 nasumičnih
  bajtova preko `RNGCryptoServiceProvider` (zastarjela klasa — build je već upozorava na to, ali
  i dalje koristi kriptografski siguran generator, ne `System.Random`). **Zaključak: ovo je PBKDF2,
  zadovoljava hard pravilo.** Jedino što bih predložio (sitna čistka, ne mijenja format hash-a):
  zamijeniti `RNGCryptoServiceProvider` sa `RandomNumberGenerator.Fill(...)` da nestane warning —
  izlaz je identičan pa ne utiče na postojeći seed hash format.
- **User** već ima `PasswordHash` + `PasswordSalt` (odvojena polja) i `ProfileImageBase64`
  (string, direktno base64 na User-u — ovo mijenjamo, vidi 3.1).
- **Asset** je trenutno čvrsto vezan za `Product` (`ProductId` required FK, cascade). Pošto
  `Destination` (bivši `Product`) prema tvojoj specifikaciji nema `Assets` kolekciju nego svoju
  `DestinationImage` tabelu, `Asset` treba postati generički blob-store bez vlasničkog FK-a (vidi 3.2).
- **Category** trenutno ima hijerarhiju (`ParentCategoryId`/`ChildCategories`) i N-N vezu sa
  `Product` preko `ProductCategory`. Tvoja specifikacija traži ravnu listu od 5 kategorija i
  `Destination.CategoryId` kao obično 1-N, pa `ProductCategory`, `ParentCategoryId` i
  `ChildCategories` otpadaju (vidi 3.3).

## 1. Referentne tabele (ne broje se u "min 10")

| Tabela | Polja | Napomena |
|---|---|---|
| **Country** | `Id`, `Name` (req) | Seed: 3 (BiH, Hrvatska, Crna Gora) |
| **City** | `Id`, `Name` (req), `CountryId` (FK req) | Seed: ~10-12, vezano za seedovane destinacije |
| **Category** | `Id`, `Name` (req), `IconName` (nullable) | Seed: tačno 5 fiksnih (Plaže, Planine, Historija, Hrana, Priroda). Hijerarhija i N-N sa Destination **uklonjeni** (vidi 3.3) |
| **Role** | `Id`, `Name` (req), `Description`, `CreatedAt`, `IsActive` | Postojeće, nepromijenjeno strukturno. Seed: `Admin`, `User` (predlog preimenovanja sa `Customer` → `User` radi konzistentnosti s CLAUDE.md terminologijom "mobile = korisnik" — **pitanje za potvrdu**, vidi §7) |
| **UserRole** | `Id`, `UserId` (FK), `RoleId` (FK), `DateAssigned` | Postojeće, nepromijenjeno |
| **RefreshToken** | `Id`, `Token`, `ExpiresAt`, `UserId` (FK) | Postojeće, nepromijenjeno. Nije seedovan (generiše se runtime-om pri loginu) |

## 2. Auth — User (prošireno)

| Polje | Tip | Napomena |
|---|---|---|
| Id | int | PK |
| FirstName | string, req | |
| LastName | string, req | |
| Email | string, req | `[EmailAddress]`; **pitanje**: dodati unique index? (vidi §7) |
| Username | string, req | **pitanje**: unique index? (vidi §7) |
| PasswordHash | string | zadržano (PBKDF2 izlaz) |
| PasswordSalt | string | zadržano — `CryptoService.Verify` ga zahtijeva |
| PhoneNumber | string, nullable | |
| **ProfileImageId** | int?, FK → Asset, nullable | **NOVO** — zamjenjuje `ProfileImageBase64`; koristi postojeći Asset mehanizam |
| CreatedAt | DateTime (UTC) | |
| IsActive | bool = true | |
| LastLoginAt | DateTime?, nullable | zadržano iz postojećeg (korisno, nije eksplicitno traženo ali ne škodi) |

Brisano: `ProfileImageBase64` (zamijenjeno sa `ProfileImageId`).

## 3. Adaptacija postojećih eCommerce entiteta

### 3.1 Product → Destination (rename + redizajn polja)

Infrastruktura (entitet, DbSet, servis, DTO-i, SearchObject, validator, kontroler, Mapster mape,
DI) se preimenuje `Product*` → `Destination*`, ali skup polja se mijenja (ne 1:1 rename):

| Destination polje | Tip | Napomena |
|---|---|---|
| Id | int | PK |
| Name | string, req | |
| Description | string, req | |
| CategoryId | int, FK req → Category | **NOVO** (zamjenjuje N-N `ProductCategories`) |
| CityId | int, FK req → City | **NOVO** |
| Keywords | string?, nullable | puni AI agent (worker faza) |
| CreatedAt | DateTime (UTC) | |
| UpdatedAt | DateTime?, nullable | |
| IsDeleted | bool = false | soft delete + global query filter |

Uklonjeno iz Product-a: `Price`, `StockQuantity`, `SKU`, `Weight`, `ProductTypeId`/`ProductType`,
`UnitOfMeasureId`/`UnitOfMeasure`, `ProductState`, `OrderItems`, `CartItems`, `Assets` (nav —
zamijenjeno sa `DestinationImage`), `ProductCategories` (N-N — zamijenjeno sa `CategoryId` FK).

### 3.2 ProductReview → Review (rename + redizajn polja)

| Review polje | Tip | Napomena |
|---|---|---|
| Id | int | PK |
| DestinationId | int, FK req → Destination | |
| UserId | int, FK req → User | |
| Rating | int, req, 1-5 | |
| Comment | string?, nullable | |
| Status | `ReviewStatus` enum, default `Pending` | zamjenjuje `IsApproved bool` |
| CreatedAt | DateTime (UTC) | |
| ModeratedByUserId | int?, FK nullable → User | |
| ModeratedAt | DateTime?, nullable | |
| ModerationReason | string?, nullable | |
| IsDeleted | bool = false | soft delete + global query filter |

Uklonjeno: `OrderId`/`Order` (nema više narudžbi), `IsApproved` (zamijenjeno sa `Status`).

### 3.3 Category (pojednostavljenje)

Uklanja se: `ParentCategoryId`, `ChildCategories` (hijerarhija), `ProductCategories` (N-N).
Dodaje se: `IconName` (string?, nullable) i navigaciona kolekcija `Destinations` (1-N).
Ostaje: `Id`, `Name`, `IsActive`, `CreatedAt`, `UpdatedAt` iz postojećeg (zadržano, nije smetnja).

### 3.4 Asset (generički blob-store)

Uklanja se vlasnički `ProductId`/`Product` FK (required, cascade). `Asset` ostaje: `Id`,
`FileName`, `ContentType`, `Base64Content`, `CreatedAt` — bez ikakvog owning FK-a. Referencira ga
`User.ProfileImageId` (nullable, `DeleteBehavior.Restrict` ili `SetNull` — vidi §7).

### 3.5 Brisanje (eCommerce specifično, TravelBay nema)

Entiteti: `Order`, `OrderItem`, `Cart`, `CartItem`, `ProductCategory`, `ProductType`,
`UnitOfMeasure`. DTO-i: `CheckoutRequest`, `CheckoutLineRequest`, i svi Order/Cart/ProductType/
UnitOfMeasure Request/Response/SearchObject. Servisi/kontroleri/validatori/DI registracije za sve
navedeno. `ProductStateMachine` folder u cijelosti (`BaseProductState`, `InitialProductState`,
`DraftProductState`, `ActiveProductState`) — Destination nema status-tok (Review/TripPlan ga imaju,
rade se u Fazi 2).

## 4. Novi glavni entiteti (13 — zahtjev je min. 10 bez referentnih)

| # | Entitet | Polja |
|---|---|---|
| 1 | **Destination** | vidi §3.1 |
| 2 | **DestinationImage** | `Id`, `DestinationId` (FK req), `ImageUrl` (req), `Source` (nullable), `OrderIndex` (int), `IsAiGenerated` (bool), `CreatedAt` |
| 3 | **Review** | vidi §3.2 |
| 4 | **TripPlan** | `Id`, `UserId` (FK req), `Name` (req), `StartDate`, `EndDate`, `Status` (`TripPlanStatus` enum, default `Draft`), `CreatedAt`, `UpdatedAt` (nullable), `IsDeleted` (bool=false) |
| 5 | **TripPlanItem** | `Id`, `TripPlanId` (FK req), `DestinationId` (FK req), `DayNumber` (int), `OrderIndex` (int), `Notes` (nullable) |
| 6 | **Collection** | `Id`, `UserId` (FK req), `Name` (req), `CreatedAt` |
| 7 | **CollectionItem** | `Id`, `CollectionId` (FK req), `DestinationId` (FK req), `AddedAt` |
| 8 | **SavedDestination** | `Id`, `UserId` (FK req), `DestinationId` (FK req), `SavedAt` — **unique (UserId, DestinationId)** |
| 9 | **UserPreference** | `Id`, `UserId` (FK req), `CategoryId` (FK req) — **unique (UserId, CategoryId)** |
| 10 | **ViewHistory** | `Id`, `UserId` (FK req), `DestinationId` (FK req), `ViewedAt` |
| 11 | **Notification** | `Id`, `UserId` (FK req), `Title` (req), `Message` (req), `Type` (`NotificationType` enum), `IsRead` (bool=false), `CreatedAt` |
| 12 | **News** | `Id`, `Title` (req), `Content` (req), `ImageUrl` (req), `CreatedAt` |
| 13 | **AuditLog** | `Id`, `EntityName` (req), `EntityId` (int), `Action` (req), `PerformedByUserId` (FK nullable → User), `PerformedAt`, `Details` (nullable) |

## 5. Enumi (`TravelBay.Model/Enums/`)

```csharp
enum ReviewStatus { Pending, Approved, Rejected }
enum TripPlanStatus { Draft, Active, Completed, Cancelled }
enum NotificationType { ReviewApproved, ReviewRejected, TripStatusChanged, News, General }
```

## 6. Relacije — pregled

**1-N (FK):**
- City → Country (`CityId.CountryId`)
- Destination → Category, Destination → City
- DestinationImage → Destination
- Review → Destination, Review → User (autor), Review → User (moderator, nullable)
- TripPlan → User
- TripPlanItem → TripPlan, TripPlanItem → Destination
- Collection → User
- CollectionItem → Collection, CollectionItem → Destination
- SavedDestination → User, SavedDestination → Destination
- UserPreference → User, UserPreference → Category
- ViewHistory → User, ViewHistory → Destination
- Notification → User
- AuditLog → User (nullable, izvršilac)
- User → Asset (`ProfileImageId`, nullable)

**N-N (modelovano kroz asocijativne entitete, isti obrazac kao postojeći `UserRole`):**
- User ↔ Role (postojeće, `UserRole`)
- User ↔ Destination "sačuvano" (`SavedDestination`, + `SavedAt`, unique par)
- User ↔ Category "preferencije" (`UserPreference`, unique par)
- TripPlan ↔ Destination (`TripPlanItem`, + `DayNumber`/`OrderIndex`/`Notes`)
- Collection ↔ Destination (`CollectionItem`, + `AddedAt`)

**Unique ograničenja:**
- `SavedDestination`: (`UserId`, `DestinationId`)
- `UserPreference`: (`UserId`, `CategoryId`)
- (otvoreno pitanje: `User.Email`, `User.Username` — vidi §7)

**DeleteBehavior (po tvojoj uputi — Cascade SAMO za vlasničku djecu, sve ostalo Restrict):**
- Cascade: `Destination → DestinationImage`, `TripPlan → TripPlanItem`, `Collection → CollectionItem`
- Restrict: sve ostale FK veze gore (uključujući referentne — Category/City/Country)

**Global query filter (`IsDeleted == false`):** `Destination`, `TripPlan`, `Review`.

## 7. Otvorena pitanja (molim potvrdu/odgovor prije Koraka 2)

1. **Role nazivi**: preimenovati seed `Customer` → `User` (radi konzistentnosti s "mobile =
   korisnik" iz CLAUDE.md), ili zadržati `Customer`? Predlažem `User`.
2. **Unique index na `User.Email` i `User.Username`**: nije eksplicitno traženo u tvojoj poruci,
   ali standardna je praksa za auth. Dodati?
3. **`Asset.DeleteBehavior`** za `User.ProfileImageId → Asset`: `Restrict` (ne dozvoli brisanje
   Asseta dok ga User koristi) ili `SetNull` (obriši sliku, User ostaje bez profilne)? Predlažem
   `SetNull` jer Asset ovdje nije "vlasnički" kritičan podatak.
4. **AuditLog seed**: seedovati par zapisa koji prate seed-ovane moderacije recenzija (da admin
   odmah ima šta vidjeti u audit logu), ili ostaviti praznu tabelu (puni se tek runtime
   moderacijom u Fazi 2)? Predlažem par seed zapisa radi konzistentnosti sa seed-ovanim
   Approved/Rejected recenzijama.
5. **CryptoService `RNGCryptoServiceProvider` warning**: ukloniti ga zamjenom za
   `RandomNumberGenerator.Fill` kao sitnu čistku u ovoj fazi (ne mijenja hash format, samo
   uklanja build warning), ili ostaviti za kasniju fazu?

## 8. Plan seed podataka (okvirni broj zapisa po tabeli)

| Tabela | ~Broj zapisa | Napomena |
|---|---|---|
| Country | 3 | BiH, Hrvatska, Crna Gora |
| City | 10-12 | Sarajevo, Mostar, Trebinje, Neum, Blagaj, Počitelj, Jajce, Konjic, Bihać, Foča (BiH); Dubrovnik, Split (Hrvatska); Kotor, Budva, Perast (Crna Gora) |
| Category | 5 | fiksno: Plaže, Planine, Historija, Hrana, Priroda |
| Destination | 22-25 | min 3 po kategoriji; npr. Stari Most/Mostar, Blagaj, Počitelj, Sarajevo (Baščaršija), Bjelašnica, Vrelo Bosne, Kravica, Jajce, Konjic, Lukomir, Una NP, Sutjeska, Dubrovnik, Split, Kotor, Budva, Perast, Neum, Trebinje... |
| DestinationImage | ~15-18 | na ~12-14 destinacija; **ostatak destinacija namjerno bez slike** (zadatak za AI agente u worker fazi) |
| Role | 2 | Admin, User |
| User | 7 | `desktop`/`test` (Admin), `mobile`/`test` (User), + 5 običnih korisnika |
| UserRole | 7 | 1 po korisniku |
| Review | ~30-35 | raspoređeno po više destinacija; većina `Approved`, par `Pending`, min. 1 `Rejected` |
| TripPlan | 5-6 | različiti statusi (Draft/Active/Completed/Cancelled) |
| TripPlanItem | 15-20 | 2-4 stavke po planu |
| Collection | 5-6 | |
| CollectionItem | 15-20 | |
| SavedDestination | 15-20 | |
| UserPreference | 10-12 | 2-3 kategorije po "običnom" korisniku (za recommender) |
| ViewHistory | 30-40 | realna historija pregleda (za recommender) |
| Notification | 10-12 | miks tipova |
| News | 5 | svaka sa `ImageUrl` |
| AuditLog | 3-5 ili 0 | vidi pitanje §7.4 |
| Asset | 0-2 | opciono, profilna slika za 1-2 test korisnika |

**Napomena o prosjeku ocjene**: `Destination` NEMA polje za prosječnu ocjenu — računa se iz
`Review` zapisa (`Status == Approved`) u recommenderu/API-ju, ne seeduje se kao denormalizovana
vrijednost.

---

**Čekam potvrdu (i odgovore na §7) prije bilo kakve implementacije.**
