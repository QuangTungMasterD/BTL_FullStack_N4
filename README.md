# Course Management System

Fullstack microservices-based course management platform.

---

## Technologies

### Backend

* ASP.NET Core
* Entity Framework Core
* SQL Server

### Frontend

* Vue 3
* Vuetify
* Pinia
* Vue Router
* Axios
* Vite

---

## Project Structure

```txt
project-root/
├── backend/
│    ├── CourseScheduleService/
│    ├── PaymentReportService/
│    └── StudentAttendanceService/
│
├── client/
│
├── .gitignore
└── README.md
```

---

## Backend Services

### CourseScheduleService

Responsible for:

* Course management
* Schedule management
* Student enrollment

### PaymentReportService

Responsible for:

* Payment processing
* Financial reports

### StudentAttendanceService

Responsible for:

* Attendance tracking
* Attendance reports

---

## Run Frontend

```bash
cd client
npm install
npm run dev
```

---

## Run Backend

Example:

```bash
cd backend/CourseScheduleService
dotnet restore
dotnet run
```

---

## Technologies & Architecture

* REST API
* Microservices
* Repository Pattern
* Dependency Injection

---

## Future Features

* Authentication & Authorization
* API Gateway
* Kafka/Event-Driven Communication
* Docker Deployment
* CI/CD
