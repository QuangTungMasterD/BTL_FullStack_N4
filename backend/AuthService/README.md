AuthService

This service issues JWT tokens for the microservices system. Changes made:

- Added EF Core `AppDbContext` with `Users` table (demo seeding).
- Seeded demo users:
  - admin@example.com / Password123 (Admin)
  - teacher@example.com / Password123 (Teacher)
  - student@example.com / Password123 (Student)
- Login now validates credentials against the `Users` table and returns `role` in the response.
- Token includes `role` claim.

Run locally:

```bash
cd backend/AuthService
dotnet run
```

Docker Compose: `docker-compose up --build` will start the shared SQL Server and services; `AuthService` reads connection string from `ConnectionStrings:AuthDb` environment variable.
