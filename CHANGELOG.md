# Changelog

## [1.0.0] – 2026-11-16

Første release af bibliotek-API'et.

### Tilføjet
- `POST /api/loans` opretter et lån (201), og `GET /api/loans/{id}` henter et lån (200/404).
- Validering: ugyldigt ISBN giver 400. Udlånt bog eller 3 aktive lån giver 409.
- `FeeCalculator`: 5 kr. pr. dag for sent, dog højst 100 kr. (udviklet med TDD).
- 13 unit tests med MSTest og Moq.
- Postman-collection med 5 API-tests.
- CI med GitHub Actions: build, unit tests og API-tests med Newman ved hvert push og pull request.
