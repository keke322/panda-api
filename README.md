# PANDA - Patient Appointment Network Data Application

This is the backend API for PANDA, a CRUD application for tracking patients and their appointments.

---

## Tech Stack

- **.NET 7 Web API**
- **EF Core** (SQLite by default, easily swappable to SQL Server / PostgreSQL)
- **FluentValidation** for business validation (NHS number checksum, postcode)
- **AutoMapper** for DTO mapping
- **Swagger** for API exploration
- **XUnit / Moq** (optional for unit tests)

---

## Features

✅ CRUD for patients  
✅ NHS number MOD11 checksum validation  
✅ UK postcode regex validation  
✅ Timezone-aware dates (`DateTimeOffset`)  
✅ Localizable error messages ready  
✅ Database-agnostic repository pattern  
✅ Swagger UI for easy testing

---

## How to run locally

1. **Clone the repository**
    ```bash
    git clone https://github.com/your-org/panda-api.git
    cd panda-api
    ```

2. **Restore packages**
    ```bash
    dotnet restore
    ```

3. **Run migrations**
    ```bash
    dotnet ef migrations add InitialCreate
    dotnet ef database update
    ```

4. **Run the application**
    ```bash
    dotnet run
    ```
    The API will be available at:  
    ```
    https://localhost:5001
    ```

---

## API Endpoints

| Method | URL                  | Description              |
|--------|----------------------|--------------------------|
| GET    | `/api/patients`      | List all patients        |
| GET    | `/api/patients/{id}` | Get patient by ID        |
| POST   | `/api/patients`      | Create a patient         |
| PUT    | `/api/patients/{id}` | Update a patient         |
| DELETE | `/api/patients/{id}` | Delete a patient         |

Swagger UI available at:

Latests version of AutoMapper was not used as it now requires a service key