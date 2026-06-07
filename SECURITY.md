# Security Policy

## Supported Versions

| Version | Supported |
|---------|-----------|
| 1.x     | ✅        |

## Reporting a Vulnerability

Do NOT open a public GitHub issue for security vulnerabilities.

Email: davidasolomon3@gmail.com

Include:
- Description of the vulnerability
- Steps to reproduce
- Potential impact
- Suggested fix (optional)

We will respond within 48 hours and aim to release a patch within 7 days for critical issues.

## Security Features

- JWT tokens validated with HMAC-SHA256, zero clock skew
- Passwords hashed with BCrypt (work factor 12)
- All database queries parameterized via EF Core
- Rate limiting on all endpoints
- HTTPS enforced in production
- No secrets committed — use environment variables
