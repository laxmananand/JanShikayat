# Jan Shikayat — Public Grievance Management System

A full-stack grievance/complaint management application modeled on the Bihar Home
Department "Jan Shikayat" portal: citizens' complaints are registered by branch
officers, reviewed and forwarded through a multi-role hierarchy (Department Head →
Competent Authority), and tracked to closure with a full audit trail.

- **Frontend:** Angular 18 (standalone components, SCSS, reactive forms)
- **Backend:** ASP.NET Core 8 Web API (EF Core, ASP.NET Identity, JWT auth)
- **Database:** PostgreSQL 16+

## 1. Prerequisites

Install these on your machine before running the project:

| Tool | Version | Download |
|---|---|---|
| .NET SDK | 8.0 | https://dotnet.microsoft.com/download/dotnet/8.0 |
| Node.js | 20 LTS (or newer) | https://nodejs.org |
| Angular CLI | 18.x | `npm install -g @angular/cli@18` |
| PostgreSQL | 14+ | https://www.postgresql.org/download/ |
| (optional) Docker + Docker Compose | latest | https://www.docker.com |

Verify installs:
```bash
dotnet --version     # should print 8.x
node --version        # should print v20.x or newer
ng version             # should show Angular CLI 18.x
psql --version         # should print 14+ (or use pgAdmin / any GUI client)
```

## 2. Project layout

```
JanShikayat/
├── backend/
│   └── JanShikayat.Api/         .NET 8 Web API (controllers, EF Core models, JWT auth)
├── frontend/
│   └── jan-shikayat-ui/         Angular 18 app
├── docker-compose.yml           Optional one-command run (Postgres + API + UI)
└── README.md                     This file
```

## 3. Database setup

Create an empty PostgreSQL database and user matching the connection string in
`backend/JanShikayat.Api/appsettings.json` (defaults shown below — change them for
anything beyond local development):

```
Host=localhost;Port=5432;Database=janshikayat;Username=postgres;Password=postgres
```

```bash
# using psql
psql -U postgres -c "CREATE DATABASE janshikayat;"
```

You do **not** need to run `dotnet ef database update` manually — the API applies
pending EF Core migrations and seeds reference/demo data automatically every time it
starts (see `Program.cs` → `DbSeeder.SeedAsync`).

## 4. Run the backend

```bash
cd backend/JanShikayat.Api
dotnet restore
dotnet run
```

The API listens on `http://localhost:5080` by default (see `Properties/launchSettings.json`
or override with `ASPNETCORE_URLS=http://localhost:5080 dotnet run`). On first run it will:

1. Apply all EF Core migrations (creates every table).
2. Seed 42 branches, 5 departments, 8 competent-authority designations, and 4 demo
   user accounts (see credentials below).

Swagger UI (API explorer) is available at `http://localhost:5080/swagger` in
development mode.

## 5. Run the frontend

In a separate terminal:

```bash
cd frontend/jan-shikayat-ui
npm install
ng serve
```

The app opens at `http://localhost:4200`. It is pre-configured (see
`src/environments/environment.ts`) to call the backend at `http://localhost:5080/api`.

## 6. Demo login credentials

The backend seeds one account per role automatically:

| Role | Email | Password |
|---|---|---|
| Super Admin | `admin@janshikayat.gov.in` | `Admin@12345` |
| Branch Officer (Branch 1) | `branchofficer1@janshikayat.gov.in` | `Branch@12345` |
| Department Head (Home Dept) | `homedept.head@janshikayat.gov.in` | `Dept@12345` |
| Competent Authority (SDO) | `sdo1@janshikayat.gov.in` | `Authority@12345` |

The login screen has one-click "fill demo account" buttons for all four.

## 7. Role hierarchy & workflow

- **Branch Officer** — registers new complaints for their branch, attaches the
  original complaint PDF.
- **Department Head** — reviews complaints forwarded to their department, adds
  enquiry/official remarks, forwards to another department or to a Competent
  Authority for field enquiry, marks action taken, closes cases.
- **Competent Authority** (Commissioner / IG / DIG / CM Jaibodha Cell / SSP / SP /
  SDO / CO) — receives complaints forwarded for field enquiry, uploads the signed
  enquiry report, records the outcome, and can close the case.
- **Super Admin** — sees everything, can create/manage user accounts for all other
  roles (`User Management` screen), and can perform any action any role can.

Complaint lifecycle: `Pending → UnderReview → ForwardedToDepartment /
ForwardedForFieldEnquiry → ActionTaken → Disposed`. Every transition is written to
an immutable history/audit-trail table shown on the complaint detail screen.

## 8. Running with Docker Compose (optional, one command)

If you have Docker installed, you can start Postgres + the API + the Angular app
together:

```bash
docker compose up --build
```

- Frontend: http://localhost:4200
- Backend: http://localhost:5080/api
- Postgres: localhost:5432 (user/password `postgres`/`postgres`, database `janshikayat`)

## 9. Before deploying anywhere beyond your local machine

This project ships with development-only secrets so it runs out of the box. Before
using it outside your local machine, you MUST:

1. Change `Jwt:Key` in `appsettings.json` (or the `Jwt__Key` environment variable) to
   a long random secret — never reuse the placeholder value.
2. Change the PostgreSQL password in both the connection string and your database.
3. Change or remove the 4 seeded demo account passwords.
4. Set `Cors:AllowedOrigins` to your real frontend origin(s).
5. Set `ASPNETCORE_ENVIRONMENT=Production` and disable Swagger UI in production if
   desired.

## 10. Troubleshooting

- **`dotnet run` fails with a Postgres connection error** — confirm PostgreSQL is
  running and the connection string's host/port/user/password/database all match a
  real database you created.
- **Frontend shows a network error on login** — confirm the backend is running on
  port 5080 and that `src/environments/environment.ts` points at the right URL.
- **CORS error in the browser console** — confirm `Cors:AllowedOrigins` in
  `appsettings.json` includes the exact origin the frontend is served from (protocol
  + host + port).
- **Migrations conflict / want a clean slate** — drop and recreate the `janshikayat`
  database, then restart the backend; it will re-create all tables and reseed.
