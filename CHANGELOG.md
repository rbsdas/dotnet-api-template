# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0] - 2026-06-07
### Added
- Initial template release
- Clean Architecture Lite structure (Domain, Application, Infrastructure, Api)
- JWT authentication with BCrypt password hashing
- EF Core + PostgreSQL with auto-migrations on startup
- FluentValidation for request validation
- Serilog structured logging with correlation IDs
- Global exception middleware with Problem Details (RFC 7807)
- API versioning (URL segment, v1)
- Rate limiting (fixed window, .NET built-in)
- Health check endpoint at `/health`
- Unit tests (xUnit + Moq + FluentAssertions)
- Integration tests (WebApplicationFactory + Testcontainers PostgreSQL)
- Multi-stage Dockerfile + docker-compose
- GitHub Actions CI/CD workflows
- Makefile with common development commands
- Central NuGet package management
