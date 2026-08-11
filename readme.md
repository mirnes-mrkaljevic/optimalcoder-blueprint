# ASP.NET Core Web API Light Architecture

A production-oriented ASP.NET Core Web API foundation designed to provide a clean starting point without introducing unnecessary architectural complexity.

The blueprint demonstrates how to structure an application around clear responsibilities while keeping the number of projects and abstractions reasonable.

## What's included

* ASP.NET Core Web API
* API versioning and Swagger
* JWT authentication and refresh tokens
* Identity & access management
* FluentValidation
* Global exception handling
* Structured logging with Serilog
* Configurable logging targets
* EF Core persistence
* Database migrations
* Rate limiting
* Dependency injection through module extensions
* Unit tests
* Domain, infrastructure, and shared modules

## Solution structure

```text
OptimalCoder.Blueprint/
├── OptimalCoder.Blueprint.API/
├── OptimalCoder.Blueprint.DB/
├── OptimalCoder.Blueprint.DB.Migrations/
├── OptimalCoder.Blueprint.Domain/
├── OptimalCoder.Blueprint.IAM/
├── OptimalCoder.Blueprint.Infra/
├── OptimalCoder.Blueprint.Shared/
└── OptimalCoder.Blueprint.Tests/
```

The projects are intentionally kept focused. Not every concern is extracted into its own project or abstraction. Components should be separated when their responsibility, complexity, or reuse justifies the additional boundary.

## Getting started

1. Clone the repository.
2. Configure the connection string and JWT settings in `appsettings.json` or your preferred secrets/configuration provider.
3. Build the solution.
4. Run the API project.
5. Open Swagger in the development environment.

The configuration values included in the repository are examples only. Replace secrets and environment-specific settings before using the blueprint in a real application.

## Documentation

For the architectural decisions and implementation details, see:

**ASP.NET Core Web API Light Architecture**
https://optimalcoder.net/net-core-web-api-light-architecture-blueprint/

The complete source code is provided as the accompanying community blueprint.

## Demo authentication

The blueprint includes a seeded development user so the JWT authentication
flow can be tested immediately.

> **Development credentials only**
>
> Username: `optimalcoderdemo`
> Password: `Optimalcoderdemo1!`

Do not use these credentials in a deployed environment. Change or remove the
seeded user before using the application outside local development.

## License

This project is licensed under the MIT License. See [LICENSE](license.txt) for details.
