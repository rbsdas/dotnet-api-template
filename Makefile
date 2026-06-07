.PHONY: help build test test-unit test-int coverage run watch migrate db-update db-drop docker-up docker-down docker-build clean format restore

API_PROJECT   := src/ApiTemplate.Api
INFRA_PROJECT := src/ApiTemplate.Infrastructure
UNIT_TESTS    := tests/ApiTemplate.Unit.Tests
INT_TESTS     := tests/ApiTemplate.Integration.Tests
EF_STARTUP    := --project $(INFRA_PROJECT) --startup-project $(API_PROJECT)

help: ## List all available targets
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "  \033[36m%-15s\033[0m %s\n", $$1, $$2}'

build: ## Build the entire solution
	dotnet build ApiTemplate.sln

test: ## Run all tests
	dotnet test ApiTemplate.sln --no-build

test-unit: ## Run unit tests only
	dotnet test $(UNIT_TESTS)

test-int: ## Run integration tests only
	dotnet test $(INT_TESTS)

coverage: ## Run tests with code coverage and generate HTML report
	dotnet test ApiTemplate.sln --collect:"XPlat Code Coverage" --results-directory ./coverage
	reportgenerator -reports:./coverage/**/coverage.cobertura.xml -targetdir:./coverage/report -reporttypes:Html

run: ## Start the API with hot reload
	dotnet run --project $(API_PROJECT)

watch: ## Start the API with file watching
	dotnet watch --project $(API_PROJECT)

migrate: ## Create a new EF Core migration (prompts for name)
	@read -p "Migration name: " name; \
	dotnet ef migrations add $$name $(EF_STARTUP)

db-update: ## Apply pending EF Core migrations
	dotnet ef database update $(EF_STARTUP)

db-drop: ## Drop the development database
	dotnet ef database drop $(EF_STARTUP) --force

docker-up: ## Start API and PostgreSQL in Docker (detached)
	docker compose up -d

docker-down: ## Stop and remove Docker containers
	docker compose down

docker-build: ## Rebuild Docker images
	docker compose build

clean: ## Remove build artifacts
	dotnet clean ApiTemplate.sln
	find . -type d -name bin  -not -path './.git/*' | xargs rm -rf
	find . -type d -name obj  -not -path './.git/*' | xargs rm -rf

format: ## Format all source files
	dotnet format ApiTemplate.sln

restore: ## Restore NuGet packages
	dotnet restore ApiTemplate.sln
