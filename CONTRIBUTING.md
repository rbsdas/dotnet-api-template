# Contributing to ApiTemplate

Thank you for your interest in contributing!

## Code of Conduct

Be respectful. Follow the [Contributor Covenant](https://www.contributor-covenant.org/).

## Reporting a Bug

1. Check existing [issues](../../issues) first.
2. Open a new issue using the **Bug report** template.
3. Include reproduction steps and environment details.

## Suggesting a Feature

Open an issue using the **Feature request** template. Describe the problem and your proposed solution.

## Development Setup

```bash
git clone https://github.com/your-org/ApiTemplate.git
cd ApiTemplate
dotnet restore ApiTemplate.sln
make db-update
make run
```

## Pull Request Process

1. Fork the repository.
2. Create a branch: `git checkout -b feat/your-feature` or `fix/your-bug`.
3. Make your changes following the conventions below.
4. Commit using [Conventional Commits](#commit-messages).
5. Push and open a pull request against `main`.
6. Fill in the PR template.

## Commit Messages

Use [Conventional Commits](https://www.conventionalcommits.org/):

| Prefix | Use for |
|--------|---------|
| `feat:` | New feature |
| `fix:` | Bug fix |
| `docs:` | Documentation only |
| `chore:` | Build process, tooling |
| `refactor:` | Code change that is not a feature or fix |
| `test:` | Adding or updating tests |

Examples:
```
feat: add pagination to examples endpoint
fix: prevent duplicate user registration
docs: update README quick start
```

## Code Style Requirements

- Follow `.editorconfig` rules — run `make format` before committing.
- No compiler warnings (`TreatWarningsAsErrors = true`).
- New features require unit tests; new endpoints require integration tests.
- All public API surfaces need `/// <summary>` XML doc comments.
- No `// TODO` stubs — finish the implementation or open a follow-up issue.
