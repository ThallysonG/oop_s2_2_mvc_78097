# 🍽️ Food Safety Inspection Tracker

## 📌 Overview

This project is an ASP.NET Core MVC web application designed to manage food safety inspections.

It allows users to:

* Manage food premises
* Record inspections
* Track follow-up actions
* Monitor system performance through a dashboard

---

## 🧱 Technologies Used

* ASP.NET Core MVC (.NET 8)
* Entity Framework Core (SQLite)
* ASP.NET Identity (Authentication & Authorization)
* Serilog (Logging)
* xUnit (Unit Testing)

---

## 👥 User Roles

| Role      | Permissions                 |
| --------- | --------------------------- |
| Admin     | Full access                 |
| Inspector | Create & manage inspections |
| Viewer    | Read-only access            |

---

## 🔐 Default Users

| Role      | Email                                           | Password |
| --------- | ----------------------------------------------- | -------- |
| Admin     | [admin@test.com](mailto:admin@test.com)         | 123456   |
| Inspector | [inspector@test.com](mailto:inspector@test.com) | 123456   |
| Viewer    | [viewer@test.com](mailto:viewer@test.com)       | 123456   |

---

## 📊 Features

* CRUD operations for:

  * Premises
  * Inspections
  * Follow-ups
* Role-based authorization
* Data validation rules
* Dashboard with statistics
* Logging system with Serilog
* Seed data initialization

---

## 🧪 Testing

Unit tests implemented using xUnit:

* DashboardService tests
* FollowUpService tests

Run tests:

```bash
dotnet test
```

---

## ⚙️ How to Run

### Option 1 (recommended)

```bash
cd FoodSafetyInspectionTracker
dotnet run
```

### Option 2

```bash
dotnet run --project .\FoodSafetyInspectionTracker\FoodSafetyInspectionTracker.csproj
```

Then open in browser:

```
Then open the URL displayed in the terminal, for example:

https://localhost:5200
```

---

## 🚀 CI/CD

GitHub Actions pipeline:

* Builds the project
* Runs unit tests automatically

---

## 📁 Project Structure

* **Controllers** → MVC Controllers
* **Models** → Domain models
* **Services** → Business logic
* **Views** → Razor UI
* **Data** → DbContext & Seed
* **Tests** → Unit tests

---

## 📌 Author

Thallyson Gama
