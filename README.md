# Bibliotek-API – Modul 8 repetition

[![CI](https://github.com/LucasTvilum/library-repetition/actions/workflows/ci.yml/badge.svg?branch=develop)](https://github.com/LucasTvilum/library-repetition/actions/workflows/ci.yml)

Et lille Web API i C#, hvor et medlem kan låne en bog. Projektet er lavet i par som repetition af emne 1–8 i Agil udvikling:
XP, Git Flow, unit tests, Moq, TDD, CI med GitHub Actions, Postman og Newman.

## Struktur

```
src/
  Library.Core/          Domænet: Loan, ILoanRepository, IsbnValidator, LoanService, FeeCalculator
  Library.Api/           Web API: POST /api/loans og GET /api/loans/{id} (data i hukommelsen)
tests/
  Library.Tests/         Unit tests med MSTest og Moq
postman/
  library.postman_collection.json   API-tests (Collection v2.1)
  local.postman_environment.json    Environment til Postman lokalt
  ci.postman_environment.json       Environment til Newman i CI
.github/workflows/ci.yml            CI-pipeline: build, unit tests og API-tests
```

## Kom i gang

```bash
dotnet build
dotnet test tests/Library.Tests
dotnet run --project src/Library.Api     # kører på http://localhost:5000
```

Kør API-testene fra kommandolinjen (API'et skal køre og være nystartet):

```bash
npx newman run postman/library.postman_collection.json -e postman/local.postman_environment.json
```

Gyldige ISBN-numre: `9780306406157`, `9781861972712`, `9780140449136`

> **Mac:** *AirPlay Receiver* lytter også på port 5000. Hvis API'et ikke svarer, eller du får `403 Forbidden`, kan du slå den fra under
> Systemindstillinger → Generelt → AirDrop & Handoff → AirPlay Receiver.

## API

| Metode | URL | Svar |
|---|---|---|
| `POST` | `/api/loans` med `{"memberId": 1, "isbn": "9780306406157"}` | **201** med lånet som JSON · **400** ugyldigt ISBN · **409** bogen er udlånt eller medlemmet har 3 lån |
| `GET` | `/api/loans/{id}` | **200** med lånet · **404** hvis det ikke findes |

Et lån har felterne `id`, `memberId`, `bookIsbn`, `loanDate`, `dueDate` (14 dage efter `loanDate`) og `returnedDate`.

## Opgaven – del for del

Hver del er lavet på sin egen feature-branch og merget ind i `develop` via en pull request, efter at navigatoren havde reviewet den og kørt testene.
Vi byttede driver og navigator ved hver ny del.

| Del | Branch | Pull request | Driver / Navigator | Hvad vi lavede |
|---|---|---|---|---|
| 1 – Git Flow og XP | `main`, `develop` | – | Lucas / Makker | Repo med `main` og `develop` lokalt og på GitHub |
| 2 – Unit tests | `feature/unit-tests` | [#1](https://github.com/LucasTvilum/library-repetition/pull/1) | Lucas / Makker | 4 tests af `IsbnValidator` med Arrange–Act–Assert |
| 3 – Moq | `feature/moq` | [#2](https://github.com/LucasTvilum/library-repetition/pull/2) | Makker / Lucas | 2 tests af `LoanService` med et mocket repository |
| 4 – TDD | `feature/fee-calculator` | [#3](https://github.com/LucasTvilum/library-repetition/pull/3) | Lucas / Makker | `FeeCalculator` bygget med red-, green- og refactor-commits |
| 5 – CI | `feature/ci` | [#4](https://github.com/LucasTvilum/library-repetition/pull/4) | Makker / Lucas | GitHub Actions workflow og branch protection på `develop` |
| 5 – CI (blokeret PR) | `feature/ci-fejlende-test` | [#5](https://github.com/LucasTvilum/library-repetition/pull/5) | Makker / Lucas | Bevidst fejlende test, der blokerede merge, og som derefter blev rettet |
| 6 – Postman | `feature/api-tests` | [#6](https://github.com/LucasTvilum/library-repetition/pull/6) | Lucas / Makker | Collection med 5 requests og 15 tests |
| 7 – Newman i CI | `feature/newman` | [#7](https://github.com/LucasTvilum/library-repetition/pull/7) | Makker / Lucas | API'et startes i pipelinen, og Newman kører collection'en |

### Del 2 – Unit tests
`IsbnValidator` fjerner bindestreger, kræver 13 cifre og vægter dem skiftevis med 1 og 3. Summen skal gå op i 10.

| Input | Forventet | Test |
|---|---|---|
| `9780306406157` | true | `IsValid_GyldigtIsbn13_ReturnererTrue` |
| `978-0-306-40615-7` | true | `IsValid_GyldigtIsbnMedBindestreger_ReturnererTrue` |
| `9780306406158` | false | `IsValid_ForkertKontrolciffer_ReturnererFalse` |
| `978030640615` | false | `IsValid_KunTolvCifre_ReturnererFalse` |

### Del 3 – Moq
`LoanService` får `ILoanRepository` ind via constructoren (dependency injection), så vi kan sende en `Mock<ILoanRepository>` ind i stedet for en database.
- Medlem med 3 aktive lån → `InvalidOperationException`, og `Add` kaldes aldrig (`Times.Never`).
- Bog allerede udlånt → `InvalidOperationException`, og `Add` kaldes aldrig.

**Faldgrube:** En mock svarer `false` som standard. Hvis man glemmer `IsBookAvailable(...).Returns(true)` i testen for 3 lån, fejler den på "bogen er udlånt" i stedet for på lånegrænsen. Testen er grøn, men af den forkerte grund. Derfor tjekker vi også fejlbeskeden.

### Del 4 – TDD
Krav: 5 kr. pr. dag for sent, dog højst 100 kr. Til tiden eller før er gratis.
Se `git log` på `feature/fee-calculator` (eller [PR #3](https://github.com/LucasTvilum/library-repetition/pull/3/commits)). Hvert trin har sit eget commit:

```
red:      afleveret til tiden giver 0 kr.          ← kompileringsfejl: klassen fandtes ikke
green:    FeeCalculator returnerer 0 kr.           ← return 0m;
test:     afleveret før forfald giver 0 kr.        ← var grøn med det samme
red:      1 dag for sent giver 5 kr.
green:    for sent aflevering koster 5 kr.         ← mindste kode: return 5m;
refactor: fælles calculator og forfaldsdato i testene
red:      10 dage for sent giver 50 kr.
green:    5 kr. pr. dag for sent                   ← (returned.Date - due.Date).Days * 5m
refactor: konstant FeePerDay
red:      gebyret er højst 100 kr.
green:    loft på 100 kr.                          ← Math.Min(..., 100m)
refactor: konstant MaxFee og forklarende kommentarer
```

### Del 5 – CI med GitHub Actions
`.github/workflows/ci.yml` kører ved push og pull request til `develop` og `main`: checkout → .NET 8 → restore → build → `dotnet test`.
`develop` er beskyttet med branch protection, så status-checket `build-and-test` skal være grønt, før en pull request kan merges.
I [PR #5](https://github.com/LucasTvilum/library-repetition/pull/5) tilføjede vi en test med en bevidst forkert forventning (15 kr. i stedet for 10 kr.). Jobbet blev rødt, og GitHub afviste merge med beskeden *"the base branch policy prohibits the merge"*. Derefter rettede vi testen.

### Del 6 – Postman
| # | Request | Forventet | Hvad testes ud over statuskoden |
|---|---|---|---|
| 1 | `POST /api/loans` `{"memberId": 1, "isbn": "9780306406157"}` | 201 | alle felter, medlem og bog, frist på 14 dage, svartid < 500 ms. Gemmer `loanId` |
| 2 | `GET /api/loans/{{loanId}}` | 200 | samme id og samme `bookIsbn` som i POST, svartid < 500 ms |
| 3 | `POST /api/loans` `{"memberId": 1, "isbn": "123"}` | 400 | fejlsvar har feltet `error` |
| 4 | `POST /api/loans` samme ISBN | 409 | fejlsvar har feltet `error` |
| 5 | `GET /api/loans/99999` | 404 | der returneres intet lån |

Sådan kører du den i Postman: importér `postman/library.postman_collection.json` og `postman/local.postman_environment.json`, vælg environment **Local**, og kør collection'en i **Collection Runner**.
Data ligger kun i hukommelsen, så **genstart API'et før hver kørsel**.

### Del 7 – Newman i CI
Efter unit testene starter pipelinen API'et i baggrunden (`dotnet run ... &`), venter med `curl --retry`, til det svarer, installerer Newman og kører collection'en med CI-environmentet.
I [PR #7](https://github.com/LucasTvilum/library-repetition/pull/7) ændrede vi bevidst den forventede statuskode i request 4 fra 409 til 200. Newman fejlede med *expected response to have status code 200 but got 409*, og jobbet blev rødt. Derefter rettede vi den tilbage.

## Refleksion

**XP og pair programming.** At bytte driver og navigator ved hver del holdt os begge skarpe. Navigatoren fangede fejl undervejs, som en test, der kunne blive grøn af den forkerte grund (Del 3). Det er også fælles kodeejerskab i praksis: vi kender begge hele koden.

**Git Flow.** Én branch pr. del gjorde hver pull request lille og let at reviewe. `develop` var altid i en fungerende tilstand, fordi alt kom ind via en pull request med grøn CI. Til gengæld giver Git Flow mange branches og merges, og til et lille projekt som dette kunne en enklere strategi (fx GitHub Flow) også have været nok.

**Unit tests.** Navngivningen `Metode_Scenarie_ForventetResultat` gør, at man kan se, hvad der gik galt, uden at åbne testen. Testene kører på millisekunder og deler ingen tilstand (F.I.R.S.T.).

**Moq.** Mocks gør det muligt at teste `LoanService` uden database. Vi lærte, at man skal være omhyggelig med opsætningen: standardsvar fra en mock kan få en test til at bestå af den forkerte grund. `Verify(..., Times.Never)` er godt til at sikre, at intet bliver gemt ved en fejl.

**TDD.** Det føltes langsomt at skrive `return 5m;`, men det tvang os til at lade testene drive designet. Loftet på 100 kr. kom først, da en test krævede det. En test (afleveret før forfald) var grøn med det samme. Den beholdt vi, fordi den dokumenterer et krav og beskytter mod regressioner.

**CI.** Pipelinen giver hurtig og automatisk feedback. Det var tydeligt at se en pull request blive blokeret af en fejlende test. Så kan ingen ved et uheld merge noget, der er i stykker.

**Postman og Newman.** API-testene tester systemet udefra via HTTP, hvor unit testene tester logikken indefra. De fangede fx, at fejlsvar har det rigtige format. Collection'en skal kunne køre fra en tom start, fordi rækkefølgen og data betyder noget. Det er en vigtig forskel fra unit tests, der skal være uafhængige af hinanden.

**Testpyramiden.** Vi har mange hurtige unit tests (13), færre API-tests (5 requests) og ingen E2E-tests, fordi der ikke er en frontend. Med en frontend ville næste skridt være nogle få E2E-tests med fx Playwright.
