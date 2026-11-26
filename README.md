# Student Management System API

A RESTful Web API built with **ASP.NET Core (.NET 8)** that manages student records using a clean **3-Layer Architecture**. The API exposes full CRUD operations, grade analytics, and file upload capabilities, with **Swagger/OpenAPI** documentation for easy testing and integration.

---

## 🏗️ Solution Structure

```
StudentManagementSystem/
├── StudentApi/                        ← Presentation Layer (Web API)
│   ├── Controllers/
│   │   └── StudentAPIController.cs   ← All HTTP endpoints
│   ├── DataSimulation/
│   │   └── StudentDataSimulation.cs  ← (Legacy – replaced by SQL Server)
│   ├── Model/
│   │   └── Student.cs                ← (Legacy model – replaced by DTO)
│   ├── Program.cs                    ← App bootstrap + Swagger setup
│   └── appsettings.json
│
├── StudentAPIBusinessLayer/           ← Business Logic Layer
│   └── Student.cs                    ← Business entity + Save/Find logic
│
└── StudentDataAccessLayer/            ← Data Access Layer
    └── StudentData.cs                ← SQL Server stored procedure calls + StudentDTO
```

---

## ⚙️ Tech Stack

| Component        | Technology                        |
|------------------|-----------------------------------|
| Framework        | ASP.NET Core Web API (.NET 8)     |
| Database         | SQL Server (via Stored Procedures)|
| ORM / Data       | Microsoft.Data.SqlClient (ADO.NET)|
| API Docs         | Swagger / OpenAPI (Swashbuckle)   |
| Architecture     | 3-Layer (Presentation → BLL → DAL)|

---

## 🗄️ Database

The application connects to a SQL Server database named **`StudentsDB`** and communicates exclusively through **stored procedures**.

### Required Stored Procedures

| Stored Procedure      | Description                             |
|-----------------------|-----------------------------------------|
| `SP_GetAllStudents`   | Returns all student records             |
| `SP_GetPassedStudents`| Returns students with passing grades    |
| `SP_GetAverageGrade`  | Returns average grade across all students|
| `SP_GetStudentById`   | Returns one student by ID               |
| `SP_AddStudent`       | Inserts a new student, returns new ID via `@NewStudentId` OUTPUT param |
| `SP_UpdateStudent`    | Updates student fields by ID            |
| `SP_DeleteStudent`    | Deletes student by ID, returns rows affected |

### Connection String

The connection string is hardcoded in `StudentData.cs` (DAL):

```csharp
static string _connectionString =
    "Server=localhost;Database=StudentsDB;User Id=sa;Password=sa;TrustServerCertificate=True;";
```

> **Note:** Update this string to match your SQL Server instance before running.

---

## 🏛️ Architecture

### Data Transfer Object (DTO)

The `StudentDTO` class (defined in the DAL) is the data contract used across all layers, keeping the layers decoupled:

```csharp
public class StudentDTO
{
    public int    Id    { get; set; }
    public string Name  { get; set; }
    public int    Age   { get; set; }
    public int    Grade { get; set; }
}
```

### Business Layer — `Student` (BLL)

The `Student` class wraps the DTO, encapsulates Add/Update mode logic, and delegates all persistence to the DAL:

```csharp
public enum enMode { AddNew = 0, Update = 1 }

public bool Save()
{
    switch (Mode)
    {
        case enMode.AddNew:  return _AddNewStudent();
        case enMode.Update:  return _UpdateStudent();
    }
    return false;
}

public static Student Find(int Id)
{
    StudentDTO SDTO = StudentData.GetStudentById(Id);
    return SDTO != null ? new Student(SDTO, enMode.Update) : null;
}
```

The `SDTO` property provides a clean read-only projection of the entity back to a DTO for the API response — the controller never returns the full `Student` object.

### Data Access Layer — `StudentData` (DAL)

All database operations use **parameterized stored procedure calls** via `SqlCommand`:

```csharp
// Example: Add with OUTPUT parameter to retrieve new ID
var outputIdParam = new SqlParameter("@NewStudentId", SqlDbType.Int)
{
    Direction = ParameterDirection.Output
};
command.Parameters.Add(outputIdParam);
command.ExecuteNonQuery();
return (int)outputIdParam.Value;
```

---

## 🔌 API Endpoints

Base URL: `https://localhost:{port}/api/Students`

| Method   | Endpoint                   | Description                              | Success | Failure          |
|----------|----------------------------|------------------------------------------|---------|------------------|
| `GET`    | `/All`                     | Get all students                         | 200     | 404              |
| `GET`    | `/Passed`                  | Get students with passing grades         | 200     | 404              |
| `GET`    | `/AverageGrade`            | Get average grade across all students    | 200     | —                |
| `GET`    | `/{id}`                    | Get a single student by ID               | 200     | 400, 404         |
| `POST`   | `/`                        | Add a new student                        | 201     | 400, 500         |
| `PUT`    | `/{id}`                    | Update an existing student               | 200     | 400, 404, 500    |
| `DELETE` | `/{id}`                    | Delete a student by ID                   | 200     | 400, 404         |
| `POST`   | `/Upload`                  | Upload an image file for a student       | 200     | 400              |
| `GET`    | `/GetImage/{fileName}`     | Retrieve an uploaded image by filename   | 200     | 404              |

### Input Validation

The controller validates all incoming data before hitting the BLL:

```csharp
if (newStudentDTO == null 
    || string.IsNullOrWhiteSpace(newStudentDTO.Name) 
    || newStudentDTO.Age < 1 
    || newStudentDTO.Grade < 0 
    || newStudentDTO.Grade > 100)
{
    return BadRequest("Not accepted student data");
}
```

### POST Response — `201 Created`

A successful `AddStudent` returns HTTP **201** with a `Location` header pointing to the new resource:

```csharp
return CreatedAtRoute("GetStudentById", new { id = newStudentDTO.Id }, newStudentDTO);
```

---

## 📁 File Upload

The API includes endpoints to upload and retrieve student images:

- Files are saved to `C:\MyUploads\` on the server.
- Each file is renamed using a **GUID** to prevent collisions:
  ```csharp
  var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
  ```
- Retrieval uses a MIME-type resolver that supports `.jpg`, `.jpeg`, `.png`, and `.gif`.

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (Express or higher)
- Visual Studio 2022 or VS Code

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/student-management-system.git
   cd student-management-system
   ```

2. **Create the database**
   - Create a database named `StudentsDB` in SQL Server.
   - Create a `Students` table with columns: `Id`, `Name`, `Age`, `Grade`.
   - Create all required stored procedures listed above.

3. **Update the connection string**
   
   Open `StudentDataAccessLayer/StudentData.cs` and update:
   ```csharp
   static string _connectionString =
       "Server=YOUR_SERVER;Database=StudentsDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;";
   ```

4. **Run the API**
   ```bash
   cd StudentApi
   dotnet run
   ```

5. **Open Swagger UI**
   
   Navigate to `https://localhost:{port}/swagger` to explore and test all endpoints interactively.

---

## 📋 Development Notes

- **Data Simulation (removed):** The project originally used an in-memory list (`StudentDataSimulation.cs`) for development. This was replaced by real SQL Server connectivity and is preserved as commented-out code.
- **DTO pattern:** The API never exposes the full `Student` BLL object in responses — only the `StudentDTO` is serialized, keeping the API contract clean and decoupled from internal implementation.
- **Stored Procedures only:** No raw SQL strings anywhere in the codebase — all queries are routed through stored procedures, improving security and maintainability.

---

## 👨‍💻 Author

**ICHF** — Learning ASP.NET Core Web API development with clean architecture patterns.
