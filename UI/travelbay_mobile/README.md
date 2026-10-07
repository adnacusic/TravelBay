# TravelBay — korisnička mobilna aplikacija (Flutter, Android)

Aplikacija za korisnike (rola `User`): preporuke, pretraga destinacija, planovi putovanja,
sačuvane destinacije i kolekcije, profil i notifikacije.

## Pokretanje

Preduslovi: Flutter SDK s Android toolchainom, pokrenut Android emulator (AVD) i API
(`docker compose up -d` u korijenu repozitorija).

```
flutter pub get
flutter run --dart-define=baseUrl=http://10.0.2.2:8080/
```

`10.0.2.2` je adresa računara gledano iz Android emulatora (`localhost` bi bio sam emulator).
`baseUrl` se zadaje isključivo preko `--dart-define`; bez njega se koristi `http://10.0.2.2:8080/`.

## Struktura

- `lib/models` — DTO modeli (`json_serializable`; nakon izmjene: `dart run build_runner build`)
- `lib/providers` — `ApiProvider` (Bearer token, obnavljanje tokena, poruke grešaka),
  `BaseProvider<T>` (paginirani CRUD) i provideri po resursu
- `lib/layouts` — `ContainerScreen` (donja navigacija, 5 tabova) i `MasterScreen` (stranice s "Back")
- `lib/screens` — ekrani po tabovima
