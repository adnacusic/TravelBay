# TravelBay — admin desktop (Flutter, Windows)

Administratorska aplikacija za TravelBay API. Prijava je dozvoljena samo korisnicima s ulogom `Admin`.

## Pokretanje

Preduslovi: Flutter SDK (Windows desktop), uključen Windows Developer Mode, pokrenut API
(`docker compose up -d` u korijenu repozitorija).

```
flutter pub get
flutter run -d windows --dart-define=baseUrl=http://localhost:8080/
```

`baseUrl` se zadaje isključivo preko `--dart-define`; bez njega se koristi `http://localhost:8080/`.

## Struktura

- `lib/models` — DTO modeli (`json_serializable`; nakon izmjene: `dart run build_runner build`)
- `lib/providers` — `BaseProvider<T>` (paginirana lista, get/insert/update/remove, Bearer token,
  automatsko obnavljanje tokena) i provideri po resursu
- `lib/layouts/master_screen.dart` — bočni meni i zaglavlje stranice
- `lib/screens` — ekrani po modulima
