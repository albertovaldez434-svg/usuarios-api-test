# .NET Services API

This project contains a set of RESTful services built with ASP.NET Core.

## 🚀 Overview

The API provides basic endpoints using:
- **ASP.NET Core Web API**
- **Entity Framework Core** for data access
- A test controller for **Dapper** (currently in progress, im learning ot use it properly)

The goal of this project is to serve as the backend layer for the [https://github.com/albertovaldez434-svg/usuarios-test] application while also exploring different data access approaches.

---

## 🧱 Architecture

- **Controllers**: Handle HTTP requests and responses
- **Entity Framework Core**: Main ORM used for database operations
**ORM = Object-Relational Mapping, oh nice (just found out what it means) **
- **Dapper (WIP)**: Experimental controller to explore lightweight or more complex data access, if required
** My previous short experiences was with VB and ADO.Net**

---

## 📦 Endpoints

Basic REST structure is implemented:

- `GET /api/[controller]` → Retrieve data
- `GET /api/[controller]/{id}` → Retrieve a single item
- `POST /api/[controller]` → Create new records
- `PUT /api/[controller]/{id}` → Update existing records
- `DELETE /api/[controller]/{id}` → Delete records

---

## 🧪 Dapper Controller (Work in Progress)

There is a controller named `DapperController` intended for testing and learning purposes using Dapper.

> **Note:** This controller is currently under development and does not yet contain full implementations. It exists as part of an ongoing learning process.

---

## ⚙️ Getting Started

1. Clone the repository
2. Configure your database connection in `appsettings.json`
3. Run migrations (if applicable)
4. Start the project:

```bash
dotnet run