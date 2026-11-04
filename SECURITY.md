# Security policy

This repository is a learning project and is **not production-ready**.

- Do not commit SQL Server credentials, connection strings containing secrets, or local `.env` files.
- Configure connection strings through .NET user secrets (local development) or environment variables.
- Disable Entity Framework Core sensitive-data logging before using real user data.
- Review authentication, password storage, session security, CSRF protections, and authorization before deployment.

If you discover a security issue, please avoid publishing exploitation details or real user data in a public issue. Contact the repository owner privately.
