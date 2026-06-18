CREATE DATABASE StudentManagementDB;
GO

USE StudentManagementDB;
GO

CREATE TABLE [User] (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50),
    Password NVARCHAR(255),
    FullName NVARCHAR(100),
    Role NVARCHAR(20),
    Status NVARCHAR(20)
);

CREATE TABLE Student (
    StudentId INT IDENTITY(1,1) PRIMARY KEY,
    StudentCode NVARCHAR(20),
    FullName NVARCHAR(100),
    Gender NVARCHAR(10),
    DateOfBirth DATE,
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    GPA DECIMAL(3,2)
);

CREATE TABLE Lecturer (
    LecturerId INT IDENTITY(1,1) PRIMARY KEY,
    LecturerCode NVARCHAR(20),
    FullName NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(20)
);

CREATE TABLE [Class] (
    ClassId INT IDENTITY(1,1) PRIMARY KEY,
    ClassCode NVARCHAR(20),
    ClassName NVARCHAR(100)
);

CREATE TABLE Subject (
    SubjectId INT IDENTITY(1,1) PRIMARY KEY,
    SubjectCode NVARCHAR(20),
    SubjectName NVARCHAR(100),
    Credit INT
);

CREATE TABLE Enrollment (
    EnrollmentId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT,
    SubjectId INT,
    Semester NVARCHAR(20),

    FOREIGN KEY (StudentId) REFERENCES Student(StudentId),
    FOREIGN KEY (SubjectId) REFERENCES Subject(SubjectId)
);

CREATE TABLE Grade (
    GradeId INT IDENTITY(1,1) PRIMARY KEY,
    EnrollmentId INT,
    AssignmentScore DECIMAL(4,2),
    FinalScore DECIMAL(4,2),
    GPA DECIMAL(3,2),

    FOREIGN KEY (EnrollmentId) REFERENCES Enrollment(EnrollmentId)
);
