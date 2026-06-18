# Student Management System

## Overview

Student Management System (SMS) is a desktop application developed using WPF (.NET 8) and SQL Server. The system helps educational institutions manage students, lecturers, subjects, enrollments, grades, and user accounts through a role-based access control mechanism.

This project is developed as a course project for PRN212.

---

# Objectives

* Manage student information.
* Manage lecturer information.
* Manage classes and subjects.
* Support subject enrollment.
* Manage student grades.
* Provide academic statistics and reports.
* Demonstrate WPF, Entity Framework Core, LINQ, and SQL Server integration.

---

# Technologies

### Frontend

* WPF
* XAML

### Backend

* C#
* .NET 8
* Entity Framework Core
* LINQ

### Database

* SQL Server

### Architecture

* 3-Layer Architecture
* Repository Pattern
* Service Layer

---

# User Roles

## Student

Students can:

* Register an account.
* Login to the system.
* View personal information.
* View available subjects.
* Register subjects.
* Cancel subject registration.
* View grades.
* View GPA.

---

## Lecturer

Lecturers can:

* Login to the system.
* View assigned subjects.
* View enrolled students.
* Enter grades.
* Update grades.
* View class performance statistics.

---

## Admin

Administrators can:

### Student Management

* Add students.
* Update students.
* Delete students.
* Search students.

### Lecturer Management

* Add lecturers.
* Update lecturers.
* Delete lecturers.
* Search lecturers.

### Class Management

* Add classes.
* Update classes.
* Delete classes.

### Subject Management

* Add subjects.
* Update subjects.
* Delete subjects.

### User Management

* Manage user accounts.
* Assign roles.
* Activate/Deactivate accounts.

### Dashboard

* View system statistics.
* Monitor academic performance.

---

# Functional Requirements

## Authentication

### Register

* Create student accounts.
* Validate account information.

### Login

* Authenticate users.
* Redirect users based on role.

---

## Student Module

### Profile

* View personal profile.

### Subject Enrollment

* Register subjects.
* Cancel subject registration.

### Academic Results

* View grades.
* View GPA.

---

## Lecturer Module

### Grade Management

* View student list.
* Enter grades.
* Update grades.

### Statistics

* View class performance.

---

## Admin Module

### Student Management

* Full CRUD operations.

### Lecturer Management

* Full CRUD operations.

### Class Management

* Full CRUD operations.

### Subject Management

* Full CRUD operations.

### User Management

* Manage system users.

### Dashboard

* Total Students
* Total Lecturers
* Total Classes
* Total Subjects
* Average GPA

---

# Database Design

## User

| Column   | Type          |
| -------- | ------------- |
| UserId   | INT           |
| Username | NVARCHAR(50)  |
| Password | NVARCHAR(255) |
| FullName | NVARCHAR(100) |
| Role     | NVARCHAR(20)  |
| Status   | NVARCHAR(20)  |

---

## Student

| Column      | Type          |
| ----------- | ------------- |
| StudentId   | INT           |
| StudentCode | NVARCHAR(20)  |
| FullName    | NVARCHAR(100) |
| Gender      | NVARCHAR(10)  |
| DateOfBirth | DATE          |
| Email       | NVARCHAR(100) |
| Phone       | NVARCHAR(20)  |
| GPA         | DECIMAL(3,2)  |

---

## Lecturer

| Column       | Type          |
| ------------ | ------------- |
| LecturerId   | INT           |
| LecturerCode | NVARCHAR(20)  |
| FullName     | NVARCHAR(100) |
| Email        | NVARCHAR(100) |
| Phone        | NVARCHAR(20)  |

---

## Class

| Column    | Type          |
| --------- | ------------- |
| ClassId   | INT           |
| ClassCode | NVARCHAR(20)  |
| ClassName | NVARCHAR(100) |

---

## Subject

| Column      | Type          |
| ----------- | ------------- |
| SubjectId   | INT           |
| SubjectCode | NVARCHAR(20)  |
| SubjectName | NVARCHAR(100) |
| Credit      | INT           |

---

## Enrollment

| Column       | Type         |
| ------------ | ------------ |
| EnrollmentId | INT          |
| StudentId    | INT          |
| SubjectId    | INT          |
| Semester     | NVARCHAR(20) |

---

## Grade

| Column          | Type         |
| --------------- | ------------ |
| GradeId         | INT          |
| EnrollmentId    | INT          |
| AssignmentScore | DECIMAL(4,2) |
| FinalScore      | DECIMAL(4,2) |
| GPA             | DECIMAL(3,2) |

---

# Project Structure

StudentManagementSystem

├── SMS.Data
│   ├── Entities
│   ├── Repositories
│   └── AppDbContext
│
├── SMS.Business
│   ├── Services
│   └── Interfaces
│
├── SMS.WPF
│   ├── Views
│   ├── ViewModels
│   ├── Commands
│   └── Resources
│
└── Database
└── StudentManagementDB.sql

---

# Main Screens

## Authentication

* Login Window
* Register Window

## Admin

* Dashboard
* Student Management
* Lecturer Management
* Class Management
* Subject Management
* User Management

## Lecturer

* Lecturer Dashboard
* Grade Management

## Student

* Student Dashboard
* Subject Registration
* Grade Viewer
* Profile Management

---

# Future Enhancements

* Attendance Management
* Course Scheduling
* Notification System
* Export Excel Reports
* Export PDF Reports
* Advanced Dashboard Charts

---

# License

This project is developed for educational purposes as a PRN212 course project.
