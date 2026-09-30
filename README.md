# FUNewsManagementSystem – PRN232 Assignment 01

Tran Ba Son Tung – ASP.NET Core Web API (OData) + ASP.NET Core MVC client, .NET 8, SQL Server.

## Solution structure

```
03_TranBaSonTung_Assignment01.sln
├── 03_TranBaSonTung_Assignment01_BackEnd    ASP.NET Core Web API + OData
│   ├── BusinessObjects/   Entities + FUNewsManagementContext (connection string from appsettings.json)
│   ├── DataAccess/        DAOs – Singleton pattern
│   ├── Repositories/      Repository pattern (interfaces + implementations, call the DAOs)
│   ├── DTOs/              Request / response models with data-annotation validation
│   └── Controllers/       OData (read/search) + REST (create/update/delete, auth, report)
├── 03_TranBaSonTung_Assignment01_FrontEnd   ASP.NET Core MVC – calls the Web API through HttpClient
└── Database/FUNewsManagement.sql
```

Controllers never touch the database directly: Controller → Repository → DAO → DbContext.

## Run

1. Run `Database/FUNewsManagement.sql` on SQL Server 2019+.
2. Edit `ConnectionStrings:FUNewsManagementDB` in `03_TranBaSonTung_Assignment01_BackEnd/appsettings.json` if needed.
3. In Visual Studio: *Solution → Configure Startup Projects → Multiple startup projects* → start both BackEnd and FrontEnd (profile `http`).
   Or from a terminal:
   ```
   dotnet run --project 03_TranBaSonTung_Assignment01_BackEnd --launch-profile http
   dotnet run --project 03_TranBaSonTung_Assignment01_FrontEnd --launch-profile http
   ```
4. API + Swagger: http://localhost:5203/swagger — Web app: http://localhost:5236

The FrontEnd reads the API address from `ApiSettings:BaseUrl` in its `appsettings.json`.

## Accounts

| Role  | Email                              | Password     |
|-------|------------------------------------|--------------|
| Admin | admin@FUNewsManagementSystem.org   | @@abc123@@ (from appsettings.json) |
| Staff | IsabellaDavid@FUNewsManagement.org | @1           |

Staff = role 1, Lecturer = role 2 (lecturers cannot log in).

## API

| Endpoint | Access | Description |
|---|---|---|
| `GET /odata/NewsArticles` | Public (active only) / Staff, Admin (all) | `$filter`, `$orderby`, `$expand`, `$select`, `$top`, `$count` |
| `GET /odata/Categories`, `/odata/Tags` | Public | OData queries |
| `POST/PUT/DELETE /api/Tag` | Staff | Delete is refused if the tag is used by any news article |
| `GET /odata/SystemAccounts` | Admin | OData queries (password is not exposed) |
| `POST /api/Auth/login` | Public | Returns a JWT |
| `POST/PUT/DELETE /api/Account` | Admin | Delete is refused if the account created any news article |
| `POST/PUT/DELETE /api/Category` | Staff | Delete is refused if the category is used by any news article |
| `POST/PUT/DELETE /api/NewsArticle` | Staff | Includes tags (`tagIds`) |
| `GET/PUT /api/Profile` | Staff | Own profile |
| `GET /api/Report?startDate=&endDate=` | Admin | Statistics by created date, sorted descending |

## Web app features

- Public: browse and search active news, view details.
- Admin: account management (CRUD + search), statistics report by period.
- Staff: category management, tag management, news article management (with tags), profile, own news history.
- Create/Update run in popup dialogs; every Delete asks for confirmation.
