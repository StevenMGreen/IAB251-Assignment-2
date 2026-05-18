# HR System API – Specification

**Version:** 1.0  
**Base URL:** `https://localhost:5001/api`  
**Format:** All responses are JSON  
**Authentication:** None (internal enterprise network)

---

## Overview

The HR System is an existing enterprise application that manages employee and department
records. Your Quotation Module must consume this API to retrieve employee information —
for example, to associate a sales representative with a quotation.

You do **not** have access to the HR database directly. You must use these API endpoints.
This is standard enterprise practice: systems communicate through APIs, not shared databases.

---

## Endpoints

### 1. List All Employees

**GET** `/api/employees`

Returns a summary list of every employee in the organisation.

**Example Request (C#):**

```csharp
var client = new HttpClient();
var response = await client.GetAsync("https://localhost:5001/api/employees");
var json = await response.Content.ReadAsStringAsync();
```

**Example Response:**

```json
[
  {
    "employeeId": 1,
    "firstName": "Sarah",
    "lastName": "Mitchell",
    "email": "s.mitchell@company.com",
    "jobTitle": "Sales Manager",
    "departmentName": "Sales"
  },
  {
    "employeeId": 2,
    "firstName": "James",
    "lastName": "Cooper",
    "email": "j.cooper@company.com",
    "jobTitle": "Sales Representative",
    "departmentName": "Sales"
  }
]
```

**Status Codes:**

| Code | Meaning             |
|------|---------------------|
| 200  | Success             |

---

### 2. Get Employee by ID

**GET** `/api/employees/{id}`

Returns the full details of a single employee.

**Example Request (C#):**

```csharp
var client = new HttpClient();
var response = await client.GetAsync("https://localhost:5001/api/employees/1");
var json = await response.Content.ReadAsStringAsync();
```

**Example Response:**

```json
{
  "employeeId": 1,
  "firstName": "Sarah",
  "lastName": "Mitchell",
  "email": "s.mitchell@company.com",
  "phone": "07 3001 1001",
  "jobTitle": "Sales Manager",
  "hireDate": "2019-03-15T00:00:00",
  "departmentId": 1,
  "departmentName": "Sales"
}
```

**Status Codes:**

| Code | Meaning                          |
|------|----------------------------------|
| 200  | Success                          |
| 404  | Employee with that ID not found  |

---

### 3. Search Employees by Name

**GET** `/api/employees/search?name={searchTerm}`

Searches employees by first name or last name (partial match, case-insensitive).

**Example Request (C#):**

```csharp
var client = new HttpClient();
var response = await client.GetAsync("https://localhost:5001/api/employees/search?name=sarah");
var json = await response.Content.ReadAsStringAsync();
```

**Example Response:**

```json
[
  {
    "employeeId": 1,
    "firstName": "Sarah",
    "lastName": "Mitchell",
    "email": "s.mitchell@company.com",
    "jobTitle": "Sales Manager",
    "departmentName": "Sales"
  }
]
```

**Status Codes:**

| Code | Meaning                                   |
|------|-------------------------------------------|
| 200  | Success (may return an empty array [])     |
| 400  | Missing or empty 'name' query parameter   |

---

### 4. List All Departments

**GET** `/api/departments`

Returns all departments, each including their list of employees.

**Example Request (C#):**

```csharp
var client = new HttpClient();
var response = await client.GetAsync("https://localhost:5001/api/departments");
var json = await response.Content.ReadAsStringAsync();
```

**Example Response:**

```json
[
  {
    "departmentId": 1,
    "name": "Sales",
    "location": "Level 3, Building A",
    "employees": [
      {
        "employeeId": 1,
        "firstName": "Sarah",
        "lastName": "Mitchell",
        "email": "s.mitchell@company.com",
        "jobTitle": "Sales Manager",
        "departmentName": "Sales"
      }
    ]
  }
]
```

---

### 5. Get Department by ID

**GET** `/api/departments/{id}`

Returns a single department with its employees.

**Example Request (C#):**

```csharp
var client = new HttpClient();
var response = await client.GetAsync("https://localhost:5001/api/departments/1");
var json = await response.Content.ReadAsStringAsync();
```

**Status Codes:**

| Code | Meaning                            |
|------|------------------------------------|
| 200  | Success                            |
| 404  | Department with that ID not found  |

---

## Sample Data

The HR System is pre-loaded with the following employees:

| ID | Name             | Job Title                  | Department       |
|----|------------------|----------------------------|------------------|
| 1  | Sarah Mitchell   | Sales Manager              | Sales            |
| 2  | James Cooper     | Sales Representative       | Sales            |
| 3  | Priya Sharma     | Sales Representative       | Sales            |
| 4  | Liam Nguyen      | Senior Developer           | Engineering      |
| 5  | Emma Johnson     | Developer                  | Engineering      |
| 6  | Chen Wei         | Developer                  | Engineering      |
| 7  | Olivia Brown     | HR Manager                 | Human Resources  |
| 8  | Daniel Smith     | HR Officer                 | Human Resources  |
| 9  | Aisha Patel      | Finance Manager            | Finance          |
| 10 | Tom Williams     | Accountant                 | Finance          |
| 11 | Megan Taylor     | Marketing Manager          | Marketing        |
| 12 | Raj Kumar        | Digital Marketing Analyst  | Marketing        |

---

## How to Call the API from Your Quotation Module

### Option A: Using HttpClient (Recommended)

Add a service class to your Quotation project:

```csharp
using System.Net.Http.Json;

public class HRApiService
{
    private readonly HttpClient _httpClient;

    public HRApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<EmployeeSummaryDto>> GetAllEmployeesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<EmployeeSummaryDto>>(
            "api/employees") ?? new List<EmployeeSummaryDto>();
    }

    public async Task<EmployeeDetailDto?> GetEmployeeByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<EmployeeDetailDto>(
            $"api/employees/{id}");
    }
}
```

Register in `Program.cs` of your Quotation project:

```csharp
builder.Services.AddHttpClient<HRApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5001");
});
```

### Option B: Using RestSharp (Alternative)

Install the NuGet package `RestSharp`, then:

```csharp
var client = new RestClient("https://localhost:5001");
var request = new RestRequest("api/employees", Method.Get);
var employees = await client.GetAsync<List<EmployeeSummaryDto>>(request);
```

---

## Interactive Testing with Swagger

The HR System includes **Swagger UI**. When the HR API is running, open your browser to:

```
https://localhost:5001/swagger
```

This gives you an interactive page where you can test every endpoint without writing code.

---

## Important Notes

- The HR System must be **running** before your Quotation Module can call it.
- Run the HR System first, then run your Quotation Module.
- Both projects can run simultaneously in Visual Studio using multiple startup projects
  (right-click Solution → Properties → Multiple startup projects).
- If you get a connection error, check that the HR System is still running and that
  the port number matches.
