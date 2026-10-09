# TravelBay

Seminarski rad iz predmeta **Razvoj softvera II** (FIT UNMO) — Adna Ćušić, IB220042.

TravelBay je aplikacija za turističke destinacije u BiH i regiji: pretraga i detalji destinacija,
recenzije s moderacijom, planovi putovanja po danima (itinerar), sačuvane destinacije i kolekcije,
preporuke s objašnjenjem i AI obogaćivanje sadržaja (ključne riječi i slike).

- **Desktop aplikacija (Windows)** — administrator: dashboard, destinacije i slike, korisnici,
  moderacija recenzija, kategorije, države i gradovi, novosti, PDF izvještaji, AI agenti.
- **Mobilna aplikacija (Android)** — korisnik: početna s preporukama, pretraga (naziv, mjesto,
  kategorija, ocjena), detalji (recenzije, sačuvaj, kolekcija, plan, dijeljenje), putovanja,
  sačuvano/kolekcije, novosti, profil (podaci, slika, preferencije, historija, lozinka), notifikacije,
  zaboravljena lozinka.

## Tehnologije

| Dio | Tehnologija |
|---|---|
| REST API | C# / ASP.NET Core 9, slojevi Model / Services / WebAPI / Common.Services |
| Baza | SQL Server 2022 (Docker), EF Core Code First + migracije i seed, baza `220042` |
| Ostalo u API-ju | JWT (access + refresh, odjava s opozivom tokena), Mapster, FluentValidation, QuestPDF, Swagger/Scalar |
| Poruke | RabbitMQ 3.13 |
| Worker (drugi servis) | Python 3.12, odvojen kontejner: AI agenti i slanje emailova (RabbitMQ consumer) |
| AI | Groq (`openai/gpt-oss-20b`) za ključne riječi, Wikimedia Commons API za slike |
| Klijenti | Flutter (desktop Windows + mobilna Android), provider pattern |

Arhitektura u Dockeru (`docker-compose.yml`): `sqlserver`, `rabbitmq`, `api`, `worker`.
API objavljuje poslove na RabbitMQ (`travelbay.ai-agents`, `travelbay.emails`), worker ih obrađuje
i piše rezultat u istu bazu. Detalji: `docs/domain-model.md`, `docs/api-surface.md`,
`recommender-dokumentacija.md`.

## Preduslovi

- **Docker Desktop** (s `docker compose`)
- Za pokretanje klijenata iz koda: **Flutter SDK** (3.4x) s Windows desktop i Android toolchainom,
  te Android emulator (AVD). Gotovi buildovi (APK i Windows) su u GitHub Release-u — vidi dolje.

## Pokretanje

1. **`.env`** — raspakujte `.env` iz priložene ZIP arhive (šifra je na DLWMS-u) u korijen repozitorija,
   pored `docker-compose.yml`. `.env` se nikad ne nalazi u Gitu; spisak varijabli je u `.env.example`.

2. **Backend** (baza, RabbitMQ, API, worker) — u korijenu repozitorija:

   ```
   docker compose up -d
   ```

   Ništa se ne pokreće ručno: API pri startu sam kreira bazu `220042`, primijeni migracije i seed
   podatke (prvi start traje ~1 min dok se SQL Server ne podigne). Provjera: `docker compose ps`
   (4 servisa `Up`), API na `http://localhost:8080`, RabbitMQ konzola na `http://localhost:15672`.
   Dokumentacija API-ja (u Development okruženju iz `.env`): `http://localhost:8080/swagger` i
   `http://localhost:8080/scalar/v1`.

3. **Desktop aplikacija** — iz Release-a pokrenite `travelbay_desktop.exe`, ili iz koda:

   ```
   cd UI/travelbay_desktop
   flutter pub get
   flutter run -d windows --dart-define=API_BASE_URL=http://localhost:8080/
   ```

4. **Mobilna aplikacija** — pokrenite Android emulator, instalirajte APK iz Release-a (prevucite ga
   na emulator), ili iz koda:

   ```
   cd UI/travelbay_mobile
   flutter pub get
   flutter run --dart-define=API_BASE_URL=http://10.0.2.2:8080/
   ```

**Adrese:** `10.0.2.2` je računar gledano iz Android emulatora (`localhost` bi bio sam emulator);
desktop koristi `localhost`. API radi na **http** (bez https), port `8080`. Adresa se zadaje
isključivo preko `--dart-define=API_BASE_URL=...`; bez nje desktop koristi `http://localhost:8080/`,
a mobilna `http://10.0.2.2:8080/`, pa Release buildovi rade bez ikakvih izmjena.

## Kredencijali

Lozinka za sve korisnike iz seeda je **`test`**.

| Aplikacija | Korisničko ime | Lozinka | Uloga |
|---|---|---|---|
| Desktop | `desktop` | `test` | Admin |
| Mobilna | `mobile` | `test` | User |
| Mobilna (dodatni korisnici) | `amina`, `marko`, `elena`, `ivan`, `lara` | `test` | User |

Desktop pušta samo ulogu Admin; mobilna je za korisnike (User). Korisnik `mobile` ima pripremljene
podatke: aktivan i završen plan putovanja, recenziju, sačuvane destinacije, preferencije i notifikacije.

## AI agenti (desktop → "AI Agenti")

- **AIAgentKeywords** — za destinacije bez ključnih riječi Groq LLM (`GROQ_MODEL`) generiše 3–5
  engleskih ključnih riječi; upisuju se u `Destination.Keywords` kao lista odvojena zarezom.
- **AIAgentSlike** — za destinacije bez slike traži slobodnu fotografiju na Wikimedia Commons (upit iz
  prve ključne riječi, pa naziv + grad), ocjenjuje relevantnost (naziv/mjesto/vrsta) i upisuje URL i
  izvor (licenca + autor) u `DestinationImage` s oznakom `IsAiGenerated`.

Pokretanje samo šalje poruku na RabbitMQ; obradu radi kontejner `worker`, a ekran sam prikazuje
status, napredak i log. **Demo:** seed već ima ključne riječi i slike za sve destinacije, pa agenti
javljaju da nema posla. Za demonstraciju na desktopu dodajte novu destinaciju bez ključnih riječi i
bez slike, pa pokrenite AIAgentKeywords, zatim AIAgentSlike.

## Zaboravljena lozinka (mobilna)

Na ekranu prijave "Zaboravljena lozinka?" → email računa → kod od 6 cifara stiže emailom (šalje ga
worker preko SMTP-a iz `.env`) → unos koda i nove lozinke. Kod vrijedi 15 minuta, jednokratan je i
zaključava se nakon 5 pogrešnih pokušaja. Ako SMTP nije podešen u `.env`, aplikacija to jasno javlja.

## Napomene o odstupanjima od prijave

- **Slike: Wikimedia Commons umjesto Google Custom Search.** Google je zatvorio Custom Search JSON API
  za nove projekte (i s ispravnim ključem i uključenim API-jem vraća `403 PERMISSION_DENIED — "This
  project does not have the access to Custom Search JSON API"`). AIAgentSlike zato koristi Wikimedia
  Commons API (slobodne slike, bez ključa); logika agenta je ista, promijenjen je samo izvor.
- **Model: `openai/gpt-oss-20b` umjesto Llama 3.1.** Groq je povukao `llama-3.1-8b-instant`
  (404 `model_not_found`) i više ne nudi Llama chat modele; model je konfiguracija (`GROQ_MODEL`).
- **Ključne riječi** se čuvaju kao tekst odvojen zarezom (CSV) u `Destination.Keywords`; klijenti ih
  prikazuju kao oznake.
- **Notifikacije** se osvježavaju pollingom (mobilna svakih 15 s), bez ručnog osvježavanja.

## Release

Buildovi (Android APK i Windows desktop) objavljuju se isključivo kao GitHub **immutable Release**
(nisu u Git historiji). Na DLWMS-u je link na tačan Release tag i šifra ZIP arhive s `.env`.
