# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Full-stack internship portal with a React frontend and ASP.NET Core 8 backend, backed by PostgreSQL. UI strings are in Croatian.

## Commands

### Backend (.NET)
```bash
cd InternshipPortal.API
dotnet restore InternshipPortal.API.sln
dotnet build InternshipPortal.API.sln
dotnet run --project InternshipPortal.API/InternshipPortal.API.csproj
# Swagger UI: http://localhost:5072/swagger
```

Run a single test project:
```bash
dotnet test InternshipPortal.API.UnitTests/InternshipPortal.API.UnitTests.csproj
dotnet test InternshipPortal.API.IntegrationTests/InternshipPortal.API.IntegrationTests.csproj
```

### Frontend (npm)
```bash
cd internship-frontend
npm install
npm run dev       # Vite dev server at http://localhost:5173
npm run build     # Production build → dist/
npm run lint      # ESLint
npm run preview   # Preview production build
```

## Architecture

### Backend layers (inside `InternshipPortal.API/`)
```
Controllers/   → thin HTTP handlers (CQRS-split: read vs. command controllers)
Services/      → business logic, uses repositories
Repositories/  → data access via EF Core
Data/EF/       → DbContext + entity models + Migrations
```

The `InternshipPortal.BL` project is a shared library holding DTOs (`DTOi/`) referenced by both the API and tests.

### Controller split pattern
Internships and Categories have separate controllers for reads vs. writes (e.g. `InternshipsController` + `InternshipCommandsController`, `CategoriesController` + `CategoryCreateController`). Follow this convention when adding new endpoints.

### Services sub-patterns
- `Services/Internships/Factories/` — factory pattern for creating internship objects
- `Services/Internships/Search/` — search strategy implementations
- `Services/Internships/Sorting/` — sorting strategy implementations

### Frontend
Single-page React app (`src/`). `api.js` is the central API client module. Auth token is stored in `localStorage` and sent as a Bearer header.

The frontend can be served as static files from the backend's `wwwroot/` (copy the `dist/` build output there).

## Configuration

| Setting | Value |
|---|---|
| Backend HTTP | `http://localhost:5072` |
| Backend HTTPS | `https://localhost:7027` |
| Frontend dev | `http://localhost:5173` |
| Frontend API URL | `VITE_API_BASE_URL` env var, defaults to `http://localhost:7027` |

Dev secrets (JWT key, DB connection string) live in `appsettings.Development.json`. The DB is PostgreSQL hosted on Render.com — connection string is already set for development use.

## Testing

- **Unit tests:** xUnit + Moq + FluentAssertions + EF In-Memory DB
- **Integration tests:** xUnit + `WebApplicationFactory`
- CI (GitHub Actions) runs only unit tests on push/PR to `master`

## CI/CD

`.github/workflows/main.yml` — runs on `ubuntu-latest`, builds Release, executes unit tests. Docker multi-stage build is defined in `InternshipPortal.API/InternshipPortal.API/Dockerfile`.
