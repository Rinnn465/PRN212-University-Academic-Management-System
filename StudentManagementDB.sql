IF DB_ID(N'StudentManagementDB') IS NULL
BEGIN
    CREATE DATABASE StudentManagementDB;
END
GO

USE StudentManagementDB;
GO

-- Demo reset script: run this file when you want to recreate the sample database.
DROP TABLE IF EXISTS Grade;
DROP TABLE IF EXISTS Enrollment;
DROP TABLE IF EXISTS LecturerSubject;
DROP TABLE IF EXISTS [User];
DROP TABLE IF EXISTS Subject;
DROP TABLE IF EXISTS Student;
DROP TABLE IF EXISTS Lecturer;
DROP TABLE IF EXISTS [Class];
GO

CREATE TABLE [Class] (
    ClassId INT IDENTITY(1,1) PRIMARY KEY,
    ClassCode NVARCHAR(20) NOT NULL UNIQUE,
    ClassName NVARCHAR(100) NOT NULL
);

CREATE TABLE Student (
    StudentId INT IDENTITY(1,1) PRIMARY KEY,
    StudentCode NVARCHAR(20) NOT NULL UNIQUE,
    FullName NVARCHAR(100) NOT NULL,
    Gender NVARCHAR(10),
    DateOfBirth DATE,
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    GPA DECIMAL(3,2),
    ClassId INT,

    FOREIGN KEY (ClassId) REFERENCES [Class](ClassId)
);

CREATE TABLE Lecturer (
    LecturerId INT IDENTITY(1,1) PRIMARY KEY,
    LecturerCode NVARCHAR(20) NOT NULL UNIQUE,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Phone NVARCHAR(20)
);

CREATE TABLE [User] (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    [Password] NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    [Role] NVARCHAR(20) NOT NULL,
    [Status] NVARCHAR(20) NOT NULL,
    StudentId INT,
    LecturerId INT,

    FOREIGN KEY (StudentId) REFERENCES Student(StudentId),
    FOREIGN KEY (LecturerId) REFERENCES Lecturer(LecturerId)
);

CREATE TABLE Subject (
    SubjectId INT IDENTITY(1,1) PRIMARY KEY,
    SubjectCode NVARCHAR(20) NOT NULL UNIQUE,
    SubjectName NVARCHAR(100) NOT NULL,
    Credit INT NOT NULL
);

-- Assign lecturers to subjects/classes for a semester.
CREATE TABLE LecturerSubject (
    LecturerSubjectId INT IDENTITY(1,1) PRIMARY KEY,
    LecturerId INT NOT NULL,
    SubjectId INT NOT NULL,
    ClassId INT,
    Semester NVARCHAR(20) NOT NULL,

    FOREIGN KEY (LecturerId) REFERENCES Lecturer(LecturerId),
    FOREIGN KEY (SubjectId) REFERENCES Subject(SubjectId),
    FOREIGN KEY (ClassId) REFERENCES [Class](ClassId)
);

CREATE TABLE Enrollment (
    EnrollmentId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT NOT NULL,
    SubjectId INT NOT NULL,
    Semester NVARCHAR(20) NOT NULL,

    FOREIGN KEY (StudentId) REFERENCES Student(StudentId),
    FOREIGN KEY (SubjectId) REFERENCES Subject(SubjectId)
);

CREATE TABLE Grade (
    GradeId INT IDENTITY(1,1) PRIMARY KEY,
    EnrollmentId INT NOT NULL,
    AssignmentScore DECIMAL(4,2),
    FinalScore DECIMAL(4,2),
    GPA DECIMAL(3,2),

    FOREIGN KEY (EnrollmentId) REFERENCES Enrollment(EnrollmentId)
);

GO

-- Sample data for demo flows
INSERT INTO [Class] (ClassCode, ClassName)
VALUES
    ('SE1950', 'SE1950'),
    ('SE1916', 'SE1916'),
    ('SE1901', 'SE1901');

INSERT INTO Student (StudentCode, FullName, Gender, DateOfBirth, Email, Phone, GPA, ClassId)
VALUES
    ('SE195001', N'Nguyễn Văn An', N'Nam', '2005-01-15', 'anvnse195001@fpt.edu.vn', '0901000001', 3.20, 1),
    ('SE195002', N'Trần Thị Bình', N'Nữ', '2005-03-20', 'binhttse195002@fpt.edu.vn', '0901000002', 3.55, 1),
    ('SE191601', N'Lê Minh Cường', N'Nam', '2004-11-05', 'cuonglmse191601@fpt.edu.vn', '0901000003', 2.95, 2),
    ('SE191602', N'Phạm Ngọc Dung', N'Nữ', '2004-07-12', 'dungpnse191602@fpt.edu.vn', '0901000004', 3.75, 2),
    ('SE190101', N'Hoàng Gia Huy', N'Nam', '2004-09-25', 'huyhgse190101@fpt.edu.vn', '0901000005', 3.10, 3);

INSERT INTO Lecturer (LecturerCode, FullName, Email, Phone)
VALUES
    ('ThinhDP2', N'Đỗ Phúc Thịnh', 'thinhdp2@fe.edu.vn', '0912000001'),
    ('MinhNV3', N'Nguyễn Văn Minh', 'minhnv3@fe.edu.vn', '0912000002'),
    ('HoaPT1', N'Phạm Thị Hoa', 'hoapt1@fe.edu.vn', '0912000003');

INSERT INTO [User] (Username, [Password], FullName, [Role], [Status], StudentId, LecturerId)
VALUES
    ('admin', '12345', N'Quản trị hệ thống', 'Admin', 'Active', NULL, NULL),
    ('SE195001', '12345', N'Nguyễn Văn An', 'Student', 'Active', 1, NULL),
    ('SE195002', '12345', N'Trần Thị Bình', 'Student', 'Active', 2, NULL),
    ('SE191601', '12345', N'Lê Minh Cường', 'Student', 'Active', 3, NULL),
    ('SE191602', '12345', N'Phạm Ngọc Dung', 'Student', 'Active', 4, NULL),
    ('SE190101', '12345', N'Hoàng Gia Huy', 'Student', 'Locked', 5, NULL),
    ('ThinhDP2', '12345', N'Đỗ Phúc Thịnh', 'Lecturer', 'Active', NULL, 1),
    ('HungLD', '12345', N'Lại Đức Hùng', 'Lecturer', 'Active', NULL, 2),
    ('AnNDH', '12345', N'Ngô Đăng Hà An', 'Lecturer', 'Active', NULL, 3);

INSERT INTO Subject (SubjectCode, SubjectName, Credit)
VALUES
    ('PRN212', 'Basic Cross-Platform Application Programming With .NET', 3),
    ('PRO192', 'Object-Oriented Programming', 3),
    ('DBI202', 'Introduction to Databases', 3),
    ('SWT301', 'Software Testing', 3),
    ('SWP391', 'Software Development Project', 3);

INSERT INTO LecturerSubject (LecturerId, SubjectId, ClassId, Semester)
VALUES
    (1, 1, 1, 'SP26'),
    (1, 1, 2, 'SP26'),
    (2, 2, 1, 'SP26'),
    (2, 3, 2, 'SP26'),
    (3, 4, 3, 'SP26'),
    (3, 5, 1, 'SP26');

INSERT INTO Enrollment (StudentId, SubjectId, Semester)
VALUES
    (1, 1, 'SP26'),
    (1, 2, 'SP26'),
    (1, 5, 'SP26'),
    (2, 1, 'SP26'),
    (2, 2, 'SP26'),
    (3, 1, 'SP26'),
    (3, 3, 'SP26'),
    (4, 1, 'SP26'),
    (4, 3, 'SP26'),
    (5, 4, 'SP26');

INSERT INTO Grade (EnrollmentId, AssignmentScore, FinalScore, GPA)
VALUES
    (1, 8.00, 8.50, 3.50),
    (2, 7.50, 8.00, 3.20),
    (3, NULL, NULL, NULL),
    (4, 9.00, 8.75, 3.80),
    (5, 8.25, 8.00, 3.40),
    (6, 6.50, 7.00, 2.80),
    (7, 7.00, 7.50, 3.00),
    (8, 9.00, 9.25, 4.00),
    (9, 8.50, 8.25, 3.60),
    (10, 6.00, 6.50, 2.50);

GO
