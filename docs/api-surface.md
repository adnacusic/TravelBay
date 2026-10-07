# TravelBay — API površina (Faza 2, nacrt)

> Plan za potvrdu prije implementacije (Korak 1 Faze 2). Ništa od ovoga još nije implementirano.
> Legenda autorizacije: **Public** = `[AllowAnonymous]`, **Auth** = `[Authorize]` (bilo koja
> prijavljena rola), **Admin** = `[Authorize(Roles="Admin")]`. "Current-user" = `userId` se čita
> isključivo iz JWT-a (`IAuthenticatedUserAccessor`), nikad iz rute/body-ja.

## 0. Prioritetni popravci prije bilo čega (A0)

1. **`BaseReadService`**: `IEnumerable<T>` → `IQueryable<T>` kroz `ApplyFilters`/
   `IncludeRelatedEntitiesAsync`/`GetAllAsync`, sa `ToListAsync()`/`CountAsync()`. Ovo je preduslov
   za SVE ispod — bez toga nijedan list endpoint stvarno ne filtrira u bazi.
2. **`AccessController.Register`**: nakon `_userService.InsertAsync(request)`, dodati
   `UserRole` zapis sa `RoleId` koji odgovara `RoleNames.User` (nikad iz request body-ja).
3. **Novi custom exceptioni** (`TravelBay.Model.Exceptions`), mapirani u `ExceptionFilter`:
   - `NotFoundException` → **404** (zamjenjuje `KeyNotFoundException` koji trenutno pada u 500 granu — bag iz templatea)
   - `BusinessException` → **409 Conflict** (nedozvoljeni state-machine prelaz, npr. "recenzija je već odobrena")
   - `ClientException` (postojeći) i `FluentValidation.ValidationException` ostaju → 400
4. **`UsersController.ChangePassword`**: trenutno `UserPasswordChangeRequest` nosi `Id` iz
   body-ja — mijenja se da `userId` dolazi isključivo iz JWT-a (kršenje hard pravila inače).

## 1. Tabela svih endpointa

### AccessController — `/Access`
| Metoda | Ruta | Auth | Opis |
|---|---|---|---|
| POST | `/Access/Register` | Public | Registracija; dodjeljuje rolu `User` serverski, ne prima role/isAdmin |
| POST | `/Access/Login` | Public | Login, vraća access+refresh token |
| POST | `/Access/LoginWithRefreshToken` | Public | Obnova tokena |

### UsersController — `/Users`
| Metoda | Ruta | Auth | Current-user / Admin | Opis |
|---|---|---|---|---|
| GET | `/Users` | Admin | Admin-nad-drugima | Lista svih korisnika, paginirano, filter po Email/Username/Name/IsActive |
| GET | `/Users/{id}` | Admin | Admin-nad-drugima | Detalj bilo kojeg korisnika |
| GET | `/Users/Me` | Auth | Current-user | Vlastiti profil |
| PUT | `/Users/Me` | Auth | Current-user | Izmjena vlastitog profila (ime, prezime, email, korisničko ime, telefon) — BEZ lozinke i slike |
| GET | `/Users/Me/Activity` | Auth | Current-user | Brojači za profil (recenzije po statusu, planovi, završeni planovi, kolekcije, sačuvano, pregledi) |
| GET | `/Users/Me/ProfileImage` | Auth | Current-user | Sadržaj slike profila (404 ako je nema) |
| PUT | `/Users/Me/ProfileImage` | Auth | Current-user | Nova slika profila (base64, samo `image/*`, max 5 MB); stari Asset se briše |
| DELETE | `/Users/Me/ProfileImage` | Auth | Current-user | Uklanja sliku profila i njen Asset |
| PUT | `/Users/{id}` | Admin | Admin-nad-drugima | Admin izmjena tuđeg profila |
| DELETE | `/Users/{id}` | Admin | Admin-nad-drugima | Deaktivacija (`IsActive=false`), NE hard delete |
| PUT | `/Users/Me/ChangePassword` | Auth | Current-user | Mijenja vlastitu lozinku — potvrđuje staru |
| PUT | `/Users/{id}/ResetPassword` | Admin | Admin-nad-drugima | Admin postavlja novu lozinku korisniku — BEZ stare |

### AssetsController — `/Assets`
| Metoda | Ruta | Auth | Opis |
|---|---|---|---|
| POST | `/Assets` | Auth | Upload blob-a (slika profila ide kroz `PUT /Users/Me/ProfileImage`) |
| GET | `/Assets` | Admin | Pregled svih (debug/admin) |
| GET | `/Assets/{id}` | Auth | Detalj |
| PUT | `/Assets/{id}` | Auth | Izmjena |
| DELETE | `/Assets/{id}` | Admin | Brisanje (FK na njega je `SetNull`, sigurno) |

### CategoriesController — `/Categories` (referentni podaci)
| Metoda | Ruta | Auth | Opis |
|---|---|---|---|
| GET | `/Categories` | Public | Lista (dropdown izvor klijentima) |
| GET | `/Categories/{id}` | Public | Detalj |
| POST | `/Categories` | Admin | Kreiranje |
| PUT | `/Categories/{id}` | Admin | Izmjena |
| DELETE | `/Categories/{id}` | Admin | Brisanje — blokirano ako postoje Destination/UserPreference zapisi (FK Restrict, jasna poruka) |

### CountriesController — `/Countries` (NOVO, referentni podaci)
| Metoda | Ruta | Auth | Opis |
|---|---|---|---|
| GET | `/Countries` | Public | Lista |
| GET | `/Countries/{id}` | Public | Detalj |
| POST | `/Countries` | Admin | Kreiranje |
| PUT | `/Countries/{id}` | Admin | Izmjena |
| DELETE | `/Countries/{id}` | Admin | Blokirano ako postoje City zapisi |

### CitiesController — `/Cities` (NOVO, referentni podaci)
| Metoda | Ruta | Auth | Opis |
|---|---|---|---|
| GET | `/Cities` | Public | Lista, filter po `CountryId` (dropdown "grad za odabranu državu") |
| GET | `/Cities/{id}` | Public | Detalj |
| POST | `/Cities` | Admin | Kreiranje (`CountryId` req) |
| PUT | `/Cities/{id}` | Admin | Izmjena |
| DELETE | `/Cities/{id}` | Admin | Blokirano ako postoje Destination zapisi |

### DestinationsController — `/Destinations`
| Metoda | Ruta | Auth | Opis |
|---|---|---|---|
| GET | `/Destinations` | Public | Filter Name/Description/CategoryId/CityId, paginirano |
| GET | `/Destinations/{id}` | Public | Detalj: slike, keywords, kategorija, `averageRating`/`reviewCount` iz Approved recenzija |
| POST | `/Destinations` | Admin | Kreiranje |
| PUT | `/Destinations/{id}` | Admin | Izmjena |
| DELETE | `/Destinations/{id}` | Admin | Soft delete (već implementirano u Fazi 1) |

### ReviewsController — `/Reviews`
| Metoda | Ruta | Auth | Current-user / Admin | Opis |
|---|---|---|---|---|
| GET | `/Reviews` | Auth | Miješano | Non-admin: Approved tuđe + sve svoje. Admin: filter po UserId/DestinationId/Status bez ograničenja |
| GET | `/Reviews/{id}` | Auth | Miješano | Isto pravilo vidljivosti |
| POST | `/Reviews` | Auth | Current-user | Kreira svoju recenziju, uvijek `Status=Pending` (ignoriše klijent status ako pošalje) |
| PUT | `/Reviews/{id}` | Auth | Current-user (vlasništvo) | Izmjena vlastite recenzije |
| DELETE | `/Reviews/{id}` | Auth | Current-user (vlasništvo) | Soft delete vlastite |
| POST | `/Reviews/{id}/Approve` | Admin | — | State machine: Pending→Approved |
| POST | `/Reviews/{id}/Reject` | Admin | — | State machine: Pending→Rejected, zahtijeva `ModerationReason` u body-ju |

### TripPlansController — `/TripPlans` (NOVO, master-details)
| Metoda | Ruta | Auth | Current-user / Admin | Opis |
|---|---|---|---|---|
| GET | `/TripPlans` | Auth | Miješano | Non-admin: samo svoji. Admin: filter po UserId |
| GET | `/TripPlans/{id}` | Auth | Vlasništvo ili Admin | Detalj plana |
| POST | `/TripPlans` | Auth | Current-user | Kreira svoj plan, uvijek `Status=Draft` |
| PUT | `/TripPlans/{id}` | Auth | Vlasništvo ili Admin | Izmjena Name/StartDate/EndDate (NE statusa) |
| DELETE | `/TripPlans/{id}` | Auth | Vlasništvo ili Admin | Soft delete |
| POST | `/TripPlans/{id}/Activate` | Auth | Vlasništvo ili Admin | State machine: Draft→Active |
| POST | `/TripPlans/{id}/Complete` | Auth | Vlasništvo ili Admin | State machine: Active→Completed |
| POST | `/TripPlans/{id}/Cancel` | Auth | Vlasništvo ili Admin | State machine: Draft/Active→Cancelled |
| GET | `/TripPlans/{id}/Items` | Auth | Vlasništvo ili Admin | Lista stavki (master-details) |
| POST | `/TripPlans/{id}/Items` | Auth | Vlasništvo ili Admin | Dodaj destinaciju (DayNumber, OrderIndex, Notes) |
| PUT | `/TripPlans/{id}/Items/{itemId}` | Auth | Vlasništvo ili Admin | Izmjena/preuređivanje stavke |
| DELETE | `/TripPlans/{id}/Items/{itemId}` | Auth | Vlasništvo ili Admin | Uklanjanje stavke |

### CollectionsController — `/Collections` (NOVO, master-details)
| Metoda | Ruta | Auth | Current-user / Admin | Opis |
|---|---|---|---|---|
| GET | `/Collections` | Auth | Miješano | Non-admin: samo svoje |
| GET | `/Collections/{id}` | Auth | Vlasništvo ili Admin | Detalj |
| POST | `/Collections` | Auth | Current-user | Kreira svoju kolekciju |
| PUT | `/Collections/{id}` | Auth | Vlasništvo ili Admin | Preimenovanje |
| DELETE | `/Collections/{id}` | Auth | Vlasništvo ili Admin | Hard delete (nema IsDeleted u modelu; cascade briše CollectionItems) |
| GET | `/Collections/{id}/Items` | Auth | Vlasništvo ili Admin | Lista destinacija u kolekciji |
| POST | `/Collections/{id}/Items` | Auth | Vlasništvo ili Admin | Dodaj destinaciju |
| DELETE | `/Collections/{id}/Items/{itemId}` | Auth | Vlasništvo ili Admin | Ukloni destinaciju |

### SavedDestinationsController — `/SavedDestinations` (NOVO)
| Metoda | Ruta | Auth | Opis |
|---|---|---|---|
| GET | `/SavedDestinations` | Auth | Current-user: lista sačuvanih destinacija, paginirano, filter `DestinationId`; svaka stavka nosi `CityName` i naslovnu `ImageUrl` |
| POST | `/SavedDestinations` | Auth | Sačuvaj (`DestinationId`); 409 ako je već sačuvana (unique) |
| DELETE | `/SavedDestinations/{id}` | Auth | Ukloni iz sačuvanih (vlasništvo) |

### UserPreferencesController — `/UserPreferences` (NOVO)
| Metoda | Ruta | Auth | Opis |
|---|---|---|---|
| GET | `/UserPreferences` | Auth | Current-user: lista preferiranih kategorija |
| PUT | `/UserPreferences` | Auth | Current-user: zamjenjuje cijeli set preferiranih `CategoryId`-eva |

### ViewHistoriesController — `/ViewHistories` (NOVO)
| Metoda | Ruta | Auth | Opis |
|---|---|---|---|
| POST | `/ViewHistories` | Auth | Current-user: zapiši pregled destinacije (koristi Faza 3 recommender) |
| GET | `/ViewHistories` | Auth | Current-user: vlastita historija, paginirano (sa `CityName` i naslovnom `ImageUrl`) |

### NotificationsController — `/Notifications` (NOVO)
| Metoda | Ruta | Auth | Opis |
|---|---|---|---|
| GET | `/Notifications` | Auth | Current-user: lista, filter `IsRead`, paginirano |
| PUT | `/Notifications/{id}/MarkAsRead` | Auth | Current-user: označi pročitanim (vlasništvo) |
| PUT | `/Notifications/MarkAllAsRead` | Auth | Current-user: označi sve pročitanim |

### NewsController — `/News` (NOVO)
| Metoda | Ruta | Auth | Opis |
|---|---|---|---|
| GET | `/News` | Public | Lista, paginirano |
| GET | `/News/{id}` | Public | Detalj |
| POST | `/News` | Admin | Kreiranje |
| PUT | `/News/{id}` | Admin | Izmjena |
| DELETE | `/News/{id}` | Admin | Hard delete (nema IsDeleted u modelu) |

### AuditLogsController — `/AuditLogs` (NOVO, read-only — prijedlog van striktne liste iz uputstva)
| Metoda | Ruta | Auth | Opis |
|---|---|---|---|
| GET | `/AuditLogs` | Admin | Pregled audit traga, filter `EntityName`/`EntityId`, paginirano. **Samo čitanje** — upisuju isključivo state machine servisi interno. |

### NotificationHub (SignalR) — `/hubs/notifications` (NOVO)
- Zahtijeva JWT (token preko query stringa pri konekciji — standardni SignalR obrazac za WS).
- Server pri konekciji pridružuje vezu u grupu `user-{userId}` (iz JWT claim-a, nikad iz query parametra).
- Servisi koji kreiraju `Notification` (Review/TripPlan state machine, eventualno News) nakon upisa u
  bazu pozivaju `Clients.Group($"user-{userId}").SendAsync("ReceiveNotification", dto)`.
- REST (`GET /Notifications`) ostaje kao fallback/početno punjenje liste; auto-refresh na klijentu je Faza 5/6.

## 2. State machine — Review

| Iz | U | Ko | Šta se dešava pri prelazu |
|---|---|---|---|
| Pending | Approved | Admin | `ModeratedByUserId`=admin, `ModeratedAt`=UtcNow; `AuditLog` ("Review", id, "Approved"); `Notification` autoru (`ReviewApproved`) + SignalR push |
| Pending | Rejected | Admin | Isto + OBAVEZAN `ModerationReason` (prazan → 400 validacija prije state machine); `Notification` (`ReviewRejected`) |
| Approved | * | — | Nedozvoljeno → `BusinessException` ("Review already moderated") |
| Rejected | * | — | Nedozvoljeno → `BusinessException` |

## 3. State machine — TripPlan

| Iz | U | Ko | Šta se dešava pri prelazu |
|---|---|---|---|
| Draft | Active | Vlasnik ili Admin | `UpdatedAt`=UtcNow; `AuditLog` ("TripPlan", id, "StatusChanged:Active"); `Notification` (`TripStatusChanged`) |
| Active | Completed | Vlasnik ili Admin | Isto, "StatusChanged:Completed" |
| Draft | Cancelled | Vlasnik ili Admin | Isto, "StatusChanged:Cancelled" |
| Active | Cancelled | Vlasnik ili Admin | Isto, "StatusChanged:Cancelled" |
| Completed | * | — | Terminalno → `BusinessException` |
| Cancelled | * | — | Terminalno → `BusinessException` |
| Draft | Completed | — | Nedozvoljeno direktno (mora preko Active) → `BusinessException` |

## 4. Otvorena pitanja / napomene (nisu blokirajuća, samo za svjesnost)

1. **`AuditLogsController`** nije eksplicitno tražen u uputstvu — dodajem ga kao read-only Admin
   endpoint jer admin desktop klijent (Faza 5) realno treba uvid u audit trag. Javi ako ga NE
   želiš u ovoj fazi, pa ga izbacujem iz implementacije.
   Fazi.
2. **`Collection` i `News` nemaju `IsDeleted`** po `docs/domain-model.md` (Faza 1) — DELETE na
   njima je hard delete. Ako želiš soft delete i za njih, treba dodati kolonu (mala migracija).
3. **Admin nad TripPlan/Collection/Review tuđih zapisa**: dajem Adminu puni CRUD pristup (ownership
   check preskače se ako je pozivalac Admin), ne samo čitanje — razumna podrazumijevana postavka za
   moderaciju/podršku, ali reci ako želiš Admina ograničiti samo na čitanje tuđih planova/kolekcija.
4. **`BusinessException` → 409 Conflict** (ne 400): nedozvoljeni state-machine prelaz je sukob sa
   trenutnim stanjem resursa, ne loš unos. Javi ako preferiraš da i to ide kao 400 radi
   jednostavnosti klijenta.

---

**Čekam potvrdu (i odgovore na §4) prije implementacije Koraka 2.**
