# TASKS - University Academic Management System Lite

## Current Repository State

- Existing repository originally had documentation, SQL script, and database bacpac.
- Created initial .NET solution:
  - `StudentManagementSystem.sln`
  - `DAL`
  - `BUS`
  - `GUI`
- Added EF Core SQL Server package to `DAL`.
- Renamed layers to the simple PRN-style naming convention: `GUI`, `BUS`, `DAL`.

## Proposed File Structure

```text
PRN212-University-Academic-Management-System/
|-- StudentManagementSystem.sln
|-- TASKS.md
|-- Readme.md
|-- StudentManagementDB.sql
|-- PRN212-Database.bacpac
|-- DAL/
|   |-- DAL.csproj
|   |-- AppDbContext.cs
|   |-- Entities/
|   |   |-- User.cs
|   |   |-- Student.cs
|   |   |-- Lecturer.cs
|   |   |-- Class.cs
|   |   |-- Subject.cs
|   |   |-- Enrollment.cs
|   |   `-- Grade.cs
|   |-- Enums/
|   |   |-- UserRole.cs
|   |   `-- UserStatus.cs
|   |-- Interfaces/
|   |   |-- IGenericRepository.cs
|   |   `-- IUnitOfWork.cs
|   `-- Repositories/
|       |-- GenericRepository.cs
|       `-- UnitOfWork.cs
|-- BUS/
|   |-- BUS.csproj
|   |-- DTOs/
|   |-- Interfaces/
|   |   |-- IAuthService.cs
|   |   |-- IStudentService.cs
|   |   |-- ILecturerService.cs
|   |   |-- ISubjectService.cs
|   |   |-- IClassService.cs
|   |   |-- IUserService.cs
|   |   `-- IDashboardService.cs
`-- GUI/
    |-- GUI.csproj
    |-- App.xaml
    |-- App.xaml.cs
    |-- Commands/
    |-- Resources/
    |-- ViewModels/
    `-- Views/
        |-- Authentication/
        |-- Admin/
        |-- Lecturer/
        `-- Student/
```

## Layer Meaning

- `GUI`: WPF screens, XAML, ViewModels, commands, app resources.
- `BUS`: business logic, validation, services, DTOs.
- `DAL`: EF Core, `AppDbContext`, entities, repositories, unit of work.

## Important Schema Notes

- Current schema does not link `User` to `Student` or `Lecturer`. For profile and role-specific screens, choose one approach:
  - Add nullable `StudentId` and `LecturerId` to `User`, or
  - Treat `Username` as `StudentCode`/`LecturerCode`.
- Current schema does not define lecturer assignments. Lecturer features need a relationship such as `LecturerSubject`, or add `LecturerId` to `Subject`.
- Current schema does not link students to classes. Class management can be CRUD-only until a `Student.ClassId` or class enrollment table is added.
- Password is currently plain text in the requirements. For PRN demo it can work, but the service layer should isolate password logic so hashing can be added later.

## TODO By Difficulty

### Phase 0 - Project Foundation

- [x] Create solution and 3 projects: `DAL`, `BUS`, `GUI`.
- [x] Add project references following 3-layer direction.
- [x] Add EF Core SQL Server package to `DAL`.
- [x] Create core entity classes from provided schema.
- [x] Create `AppDbContext` and base repository/unit of work contracts.
- [x] Add `appsettings.json` or a simple connection-string provider for WPF.
- [x] Configure default SQL Server Authentication connection string for `sa/12345`.
- [x] Add `AppDbContextFactory` for EF Core SQL Server connections.
- [x] Add `SetupSqlAuthentication.sql` helper script for enabling/updating `sa`.
- [x] Add `.gitignore` for build artifacts.
- [x] Verify build after foundation files are added.

### Phase 1 - Database and Data Access

- [x] Update SQL script with simple demo-friendly foreign keys and basic unique columns.
- [x] Decide and implement account-owner mapping for `User -> Student/Lecturer`.
- [x] Decide and implement lecturer assignment mapping with `LecturerSubject`.
- [x] Simplify `AppDbContext` mappings to match the demo SQL schema.
- [ ] Add EF Core migrations, or keep database-first SQL script only for PRN submission.
- [x] Seed admin account, sample students, lecturers, subjects, enrollments, and grades.

### Phase 2 - Business Layer Services

- [x] Implement `AuthService` for login/status checks.
- [x] Extend `AuthService` for student registration.
- [x] Implement `StudentService` for profile, subject list, enroll/cancel, grades, GPA.
- [x] Implement `LecturerService` for assigned subjects/classes, student list, grade entry/update, statistics.
- [x] Implement Admin CRUD services for students, lecturers, classes, subjects, and users.
- [x] Implement `DashboardService` for total counts and average GPA.
- [x] Add validation messages and duplicate checks in services.

### Phase 3 - WPF Base UI

- [x] Create shared `RelayCommand` and base `ViewModelBase`.
- [x] Create navigation/session state.
- [x] Create `LoginWindow`.
- [x] Create `RegisterWindow`.
- [x] Redirect user by role after login.
- [x] Add basic styles/resources for buttons, text boxes, tables, and dialogs.

### Phase 4 - Admin Screens

- [ ] `DashboardWindow`: total students, lecturers, classes, subjects, average GPA.
- [ ] `StudentManagementWindow`: list, add, update, delete, search.
- [ ] `LecturerManagementWindow`: list, add, update, delete, search.
- [ ] `SubjectManagementWindow`: list, add, update, delete.
- [ ] `ClassManagementWindow`: list, add, update, delete.
- [ ] `UserManagementWindow`: lock/unlock account, assign roles.

### Phase 5 - Student Screens

- [ ] `StudentHomeWindow`: summary and navigation.
- [ ] `ProfileWindow`: view/update personal information.
- [ ] `SubjectRegistrationWindow`: available subjects, register, cancel registration.
- [ ] `GradeViewWindow`: transcript and GPA.

### Phase 6 - Lecturer Screens

- [ ] `LecturerHomeWindow`: assigned subjects/classes and navigation.
- [ ] `StudentGradeWindow`: enrolled students, enter/update scores.
- [ ] Add class result statistics: count, pass/fail rate, average score/GPA.

### Phase 7 - Polish and Submission

- [ ] Add user-friendly Vietnamese labels/messages.
- [ ] Add exception handling and confirmation dialogs.
- [ ] Test role flows manually with seed accounts.
- [ ] Update `Readme.md` with setup, database import, demo accounts, and screenshots.
- [ ] Clean build artifacts before final submission.
