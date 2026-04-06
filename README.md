# 🍽️ Food Safety Inspection Tracker

## 📌 Overview
This project is an ASP.NET Core MVC application for managing food safety inspections.

It allows users to:
- Manage premises
- Record inspections
- Track follow-ups
- Monitor performance via dashboard

---

## 🧱 Technologies Used
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core (SQLite)
- ASP.NET Identity
- Serilog (logging)
- xUnit (unit testing)

---

## 👥 Roles
- **Admin** – Full access
- **Inspector** – Create and manage inspections
- **Viewer** – Read-only access

---

## 🔐 Default Users

| Role      | Email               | Password |
|----------|--------------------|----------|
| Admin     | admin@test.com     | 123456   |
| Inspector | inspector@test.com | 123456   |
| Viewer    | viewer@test.com    | 123456   |

---

## 📊 Features
- CRUD for Premises, Inspections, FollowUps
- Role-based authorization
- Validation rules
- Dashboard with statistics
- Logging system (Serilog)

---

## 🧪 Testing
Unit tests implemented using xUnit:
- DashboardService tests
- FollowUpService tests

Run tests:
```bash
dotnet test
