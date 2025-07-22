# PANDA - Patient Appointment Network Data Application

This is the backend API for PANDA, a CRUD application for tracking patients and their appointments.

---

## Tech Stack

- **.NET 8 Web API**
- **EF Core** (SQLite for fast MVP, swappable to SQL Server / PostgreSQL)
- **FluentValidation** for business validation (NHS number checksum, postcode)
- **AutoMapper** for DTO mapping
- **Swagger** for API exploration
- **XUnit / Moq** (for unit tests)
- **MVC WebAppFactory** (for integration tests)

---

## Features

- CRUD for patients  
- NHS number MOD11 checksum validation  
- UK postcode regex validation  
- Timezone-aware dates (`DateTimeOffset`)  
- Localizable error messages  
- Database-agnostic repository pattern  
- Swagger UI for easy testing

---

## How to run locally

1. **Clone the repository**
    ```bash
    git clone https://github.com/keke322/panda-api.git
    cd panda-api
    ```

2. **Restore packages**
    ```bash
    dotnet restore
    ```

3. **Run migrations**
    ```bash
    cd Panda
    dotnet ef database update
    ```

4. **Run the application**
    ```bash
    dotnet run
    ```
    The API will be available at:  
    ```
    http://localhost:5224
    ```
    Swagger UI available at:    
    ```
    http://localhost:5224/swagger/index.html
    ```
5. **Testing**
    ```bash
    cd ..
    dotnet run
    ```
---

## API Endpoints

| Method | URL                                      | Description              |
|--------|------------------------------------------|--------------------------|
| GET    | `/api/patients`                          | List all patients        |
| GET    | `/api/patients/{id}`                     | Get patient by ID        |
| POST   | `/api/patients`                          | Create a patient         |
| PUT    | `/api/patients/{id}`                     | Update a patient         |
| DELETE | `/api/patients/{id}`                     | Delete a patient         |
| GET    | `/api/appointment`                       | List all appointments    |
| GET    | `/api/appointment/{id}`                  | Get appointment by ID    |
| POST   | `/api/appointment`                       | Create an appointment    |
| PUT    | `/api/appointment/{id}`                  | Update an appointment    |
| POST   | `/api/appointment/{id}/cancel`           | Cancel an appointment    |
| GET    | `/api/appointment/analytics/byclinician`      | Analytics for missed appointments    |
| GET    | `/api/appointment/analytics/bydepartment`      | Analytics for missed appointments    |


## Design considerations

- Appointment model was changed to use patientID to relate to patient table instead of NHS number as foreign key because NHSnumber as a string is less efficient than GUID and if for any reason however unlikely NHSnumber changes, multiple updates to tables will be needed.
- For this project the priority of development focused on SOLID principles and testing automation.

## Future work

- Wide range of automated testing, including localised error messages and more edge cases.
- Extract the analytics controller into its own controller route in order to add more future data sets.
- Clinician information needs to be extracted into its own entity very much like Patient and so the appointment model will only contain a reference to clinician ID.
- GDPR considerations need to be extended to encrypt patient Data that could be patient identifiable.