# BulletinBoardAPI

ASP.NET Core Web API med JWT authentication. Skapa användare, poster, och kommentarer på andra användares poster.

---

## Stack

- ASP.NET Core 10
- Entity Framework Core + SQL Server
- JWT Authentication
- BCrypt password hashing
- Swagger UI

---

## Setup

**Prerequisites:** .NET 10, Docker Desktop

# 1. Starta SQL Server
> docker run -e ACCEPT_EULA=Y -e SA_PASSWORD=pnutBAdmin123! \
  -p 1433:1433 --name sqlserver -d mcr.microsoft.com/azure-		sql-edge:latest

# 2. Klona repot, kör
> git clone https://github.com/je-suis-paradis/BulletinBoardAPI.git
> cd BulletinBoardAPI
> dotnet restore
> dotnet ef database update
> dotnet run
>
> Swagger UI: `http://localhost:5239/swagger`

---

## Authentication

1. `POST /api/auth/register` → create account
2. `POST /api/auth/login` → get token
3. I Swagger: click **Authorize** → enter `Bearer <token>`
## Features

- JWT authentication med 7 dagars giltighet
- BCrypt password hashing
- 403 Forbidden om användare försöker komentera på egen post
- Användare kan bara editera och radera sitt eget
- Interface + Repository pattern for data access
- Swagger UI with JWT authorization support
