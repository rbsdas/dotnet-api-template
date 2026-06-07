# CLAUDE.md — Working with ApiTemplate in Claude Code

## Project Overview
ApiTemplate is a .NET 10 Clean Architecture Lite API template. Four source projects: Domain → Application ← Infrastructure ← Api.

## Architecture Rules (non-negotiable)
1. Domain has ZERO external dependencies — no NuGet packages, no project references beyond System.*.
2. Application references only Domain. Never references Infrastructure.
3. Infrastructure references Application and Domain.
4. Api references all layers (wiring only — no business logic in Program.cs or Controllers).

## Entity Naming — Avoid BCL / Framework Collisions

When naming domain entities, avoid names that collide with .NET BCL types, ASP.NET Core types,
or other common identifiers already in scope. Collisions cause CS0104 ambiguous-reference errors
or subtly shadow the wrong type.

**Banned entity names and their safe alternatives:**

| Banned name  | Conflicts with                          | Use instead         |
|--------------|-----------------------------------------|---------------------|
| `User`       | `ControllerBase.User` (ClaimsPrincipal) | `AppUser`           |
| `Task`       | `System.Threading.Tasks.Task`           | `WorkItem`, `Job`   |
| `File`       | `System.IO.File`                        | `Document`, `Asset` |
| `Action`     | `System.Action<T>`                      | `UserAction`        |
| `Controller` | `Microsoft.AspNetCore.Mvc.Controller`   | `DeviceController`  |
| `Exception`  | `System.Exception`                      | `AppException`      |
| `Thread`     | `System.Threading.Thread`               | `WorkThread`        |

The template already uses `AppUser` (not `User`) and `DomainValidationException` (not
`ValidationException`, which collides with `FluentValidation.ValidationException`) for this reason.

## Code Conventions
- All async methods must accept `CancellationToken ct = default` as last parameter.
- All configuration access goes through `IOptions<T>` — never inject `IConfiguration` directly into services.
- All request validation uses FluentValidation — never `ModelState.IsValid` in controllers.
- Controllers are thin: validate input, call service, return response. No business logic.
- Services throw domain exceptions (`NotFoundException`, `ConflictException`, `DomainValidationException`, etc.) — never return null or bool.
- All DateTime values must be UTC. Set `DateTimeKind.Utc` at system boundaries.

## Running the Project
- `make run` — starts API with hot reload
- `make docker-up` — starts API + PostgreSQL in Docker
- `make test` — runs all tests

## Database Migrations
- `make migrate` (prompts for name) — creates new migration
- `make db-update` — applies pending migrations
- Migrations auto-apply on startup (development and staging only; production requires explicit apply)

## Testing Strategy
- Unit tests: mock all dependencies with Moq; use FluentAssertions
- Integration tests: Testcontainers PostgreSQL; real HTTP stack via WebApplicationFactory
- Never mock DbContext directly — use integration tests for repository tests

## Adding a New Feature
1. Add domain entity to Domain/Entities/ (check the banned-names table above first)
2. Add repository interface to Application/Common/Interfaces/
3. Add DTOs and validators to Application/YourFeature/
4. Implement service in Application/YourFeature/
5. Implement repository in Infrastructure/Persistence/Repositories/
6. Add controller in Api/Controllers/V1/
7. Register in each layer's ServiceCollectionExtensions
   (Domain has no ServiceCollectionExtensions — it has zero NuGet dependencies by design)
8. Add unit tests + integration tests
9. Run `make migrate` to create DB migration

## Environment Variables
See `.env.example` for all required variables. Copy to `.env` for local Docker use.
Use `dotnet user-secrets` for local non-Docker development.

## Locked Decisions
- Database: PostgreSQL via EF Core (no Dapper, no MongoDB)
- Auth: JWT Bearer (no OAuth until multi-tenant required)
- Logging: Serilog (do not revert to Microsoft.Extensions.Logging console provider)
- Validation: FluentValidation (do not use DataAnnotations for business rules; domain validation errors throw `DomainValidationException`)
- ORM: EF Core code-first migrations (do not mix raw SQL unless performance-critical, then document why)
