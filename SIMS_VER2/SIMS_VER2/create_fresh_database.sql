-- =============================================
-- Create Fresh Database - Complete Script
-- Run this to completely recreate the database
-- =============================================

USE master;
GO

-- Drop existing database
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'SIMS_DB2')
BEGIN
    ALTER DATABASE SIMS_DB2 SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SIMS_DB2;
    PRINT 'Existing database dropped.';
END
GO

-- Create new database
CREATE DATABASE SIMS_DB2;
GO

USE SIMS_DB2;
GO

-- =============================================
-- Create Tables
-- =============================================

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY(1,1),
    [Username] nvarchar(50) NOT NULL,
    [PasswordHash] nvarchar(255) NOT NULL,
    [Role] int NOT NULL,
    [Active] bit NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Students] (
    [Id] int NOT NULL IDENTITY(1,1),
    [StudentCode] nvarchar(10) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [DateOfBirth] datetime2 NULL,
    [Gender] nvarchar(10) NULL,
    [Email] nvarchar(100) NULL,
    [Phone] nvarchar(20) NULL,
    [Address] nvarchar(200) NULL,
    [Program] nvarchar(50) NULL,
    [Intake] nvarchar(20) NULL,
    [Status] nvarchar(20) NOT NULL DEFAULT 'ACTIVE',
    [UserId] int NULL,
    CONSTRAINT [PK_Students] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Students_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [Courses] (
    [Id] int NOT NULL IDENTITY(1,1),
    [CourseCode] nvarchar(20) NOT NULL,
    [CourseName] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Credits] int NOT NULL,
    [Program] nvarchar(50) NULL,
    [Type] nvarchar(20) NULL,
    CONSTRAINT [PK_Courses] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [CourseOfferings] (
    [Id] int NOT NULL IDENTITY(1,1),
    [CourseId] int NOT NULL,
    [Semester] nvarchar(20) NOT NULL,
    [Year] int NOT NULL,
    [ClassCode] nvarchar(20) NOT NULL,
    [InstructorUserId] int NULL,
    [MaxStudents] int NOT NULL,
    [Status] nvarchar(20) NOT NULL DEFAULT 'OPEN',
    CONSTRAINT [PK_CourseOfferings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CourseOfferings_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CourseOfferings_Users_InstructorUserId] FOREIGN KEY ([InstructorUserId]) REFERENCES [Users] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [Enrollments] (
    [Id] int NOT NULL IDENTITY(1,1),
    [StudentId] int NOT NULL,
    [CourseOfferingId] int NOT NULL,
    [Status] nvarchar(20) NOT NULL DEFAULT 'ENROLLED',
    [Grade] decimal(5,2) NULL,
    CONSTRAINT [PK_Enrollments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Enrollments_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Enrollments_CourseOfferings_CourseOfferingId] FOREIGN KEY ([CourseOfferingId]) REFERENCES [CourseOfferings] ([Id]) ON DELETE CASCADE
);
GO

-- =============================================
-- Create Indexes
-- =============================================

CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
GO

CREATE UNIQUE INDEX [IX_Students_StudentCode] ON [Students] ([StudentCode]);
GO

CREATE UNIQUE INDEX [IX_Courses_CourseCode] ON [Courses] ([CourseCode]);
GO

CREATE UNIQUE INDEX [IX_Enrollments_StudentId_CourseOfferingId] ON [Enrollments] ([StudentId], [CourseOfferingId]);
GO

-- =============================================
-- Insert Seed Data
-- =============================================

-- Insert Users
-- Password hash: SHA256(password) -> Base64
-- admin123 = JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=
-- faculty123 = JwQfWFbHOHqZclJpSvsEjRqpOSKP/NvWKFuXm42iDno=
-- student123 = cDsKPWrXW2SaKK3efYPGJR2kV1SSY7x/9F7HCbCoRIs=
INSERT INTO [Users] ([Username], [PasswordHash], [Role], [Active]) VALUES
('admin', 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=', 0, 1),
('faculty1', 'JwQfWFbHOHqZclJpSvsEjRqpOSKP/NvWKFuXm42iDno=', 2, 1),
('faculty2', 'JwQfWFbHOHqZclJpSvsEjRqpOSKP/NvWKFuXm42iDno=', 2, 1),
('student1', 'cDsKPWrXW2SaKK3efYPGJR2kV1SSY7x/9F7HCbCoRIs=', 1, 1);
GO

-- Insert Students
-- Note: BD00001 is linked to student1 user
DECLARE @Student1UserId int = (SELECT Id FROM Users WHERE Username = 'student1');

INSERT INTO [Students] ([StudentCode], [FullName], [DateOfBirth], [Gender], [Email], [Phone], [Address], [Program], [Intake], [Status], [UserId]) VALUES
('BD00001', 'John Doe', '2000-01-15', 'Male', 'john.doe@student.edu', '0123456789', '123 Main Street', 'Computer Science', '2020', 'ACTIVE', @Student1UserId),
('BD00002', 'Jane Smith', '2001-03-20', 'Female', 'jane.smith@student.edu', '0987654321', '456 Oak Avenue', 'Computer Science', '2020', 'ACTIVE', NULL),
('BD00003', 'Bob Johnson', '2000-07-10', 'Male', 'bob.johnson@student.edu', '0111222333', '789 Pine Road', 'Information Technology', '2021', 'ACTIVE', NULL),
('BD00004', 'Alice Williams', '2001-11-05', 'Female', 'alice.williams@student.edu', '0444555666', '321 Elm Street', 'Information Technology', '2021', 'ACTIVE', NULL),
('BD00005', 'Charlie Brown', '2000-05-25', 'Male', 'charlie.brown@student.edu', '0777888999', '654 Maple Drive', 'Computer Science', '2020', 'ACTIVE', NULL);
GO

-- Insert Courses
INSERT INTO [Courses] ([CourseCode], [CourseName], [Description], [Credits], [Program], [Type]) VALUES
('CS101', 'Introduction to Programming', 'Basic programming concepts and fundamentals', 3, 'Computer Science', 'CORE'),
('CS201', 'Data Structures', 'Study of data structures and algorithms', 4, 'Computer Science', 'CORE'),
('CS301', 'Database Systems', 'Introduction to database design and SQL', 3, 'Computer Science', 'CORE'),
('IT101', 'Web Development', 'Fundamentals of web development', 3, 'Information Technology', 'CORE'),
('CS401', 'Software Engineering', 'Software development methodologies and practices', 4, 'Computer Science', 'CORE');
GO

-- Insert Course Offerings
DECLARE @CS101Id int = (SELECT Id FROM Courses WHERE CourseCode = 'CS101');
DECLARE @CS201Id int = (SELECT Id FROM Courses WHERE CourseCode = 'CS201');
DECLARE @CS301Id int = (SELECT Id FROM Courses WHERE CourseCode = 'CS301');
DECLARE @IT101Id int = (SELECT Id FROM Courses WHERE CourseCode = 'IT101');
DECLARE @CS401Id int = (SELECT Id FROM Courses WHERE CourseCode = 'CS401');
DECLARE @Faculty1Id int = (SELECT Id FROM Users WHERE Username = 'faculty1');
DECLARE @Faculty2Id int = (SELECT Id FROM Users WHERE Username = 'faculty2');

INSERT INTO [CourseOfferings] ([CourseId], [Semester], [Year], [ClassCode], [InstructorUserId], [MaxStudents], [Status]) VALUES
(@CS101Id, 'Fall', 2024, 'CS101-F24-01', @Faculty1Id, 30, 'ONGOING'),
(@CS201Id, 'Fall', 2024, 'CS201-F24-01', @Faculty1Id, 25, 'ONGOING'),
(@CS301Id, 'Fall', 2024, 'CS301-F24-01', @Faculty2Id, 30, 'ONGOING'),
(@IT101Id, 'Fall', 2024, 'IT101-F24-01', @Faculty2Id, 35, 'OPEN'),
(@CS401Id, 'Spring', 2025, 'CS401-S25-01', @Faculty1Id, 20, 'OPEN');
GO

-- Insert Enrollments
DECLARE @JohnDoeId int = (SELECT Id FROM Students WHERE StudentCode = 'BD00001');
DECLARE @JaneSmithId int = (SELECT Id FROM Students WHERE StudentCode = 'BD00002');
DECLARE @BobJohnsonId int = (SELECT Id FROM Students WHERE StudentCode = 'BD00003');
DECLARE @AliceWilliamsId int = (SELECT Id FROM Students WHERE StudentCode = 'BD00004');
DECLARE @CS101OfferingId int = (SELECT Id FROM CourseOfferings WHERE ClassCode = 'CS101-F24-01');
DECLARE @CS201OfferingId int = (SELECT Id FROM CourseOfferings WHERE ClassCode = 'CS201-F24-01');
DECLARE @CS301OfferingId int = (SELECT Id FROM CourseOfferings WHERE ClassCode = 'CS301-F24-01');
DECLARE @IT101OfferingId int = (SELECT Id FROM CourseOfferings WHERE ClassCode = 'IT101-F24-01');

INSERT INTO [Enrollments] ([StudentId], [CourseOfferingId], [Status], [Grade]) VALUES
(@JohnDoeId, @CS101OfferingId, 'ENROLLED', NULL),
(@JohnDoeId, @CS201OfferingId, 'ENROLLED', NULL),
(@JaneSmithId, @CS101OfferingId, 'ENROLLED', NULL),
(@JaneSmithId, @CS301OfferingId, 'ENROLLED', NULL),
(@BobJohnsonId, @IT101OfferingId, 'ENROLLED', NULL),
(@AliceWilliamsId, @IT101OfferingId, 'ENROLLED', NULL);
GO

-- =============================================
-- Verification
-- =============================================

PRINT '';
PRINT '========================================';
PRINT 'Database Created Successfully!';
PRINT '========================================';
PRINT '';

SELECT 'Users' AS TableName, COUNT(*) AS Count FROM [Users]
UNION ALL SELECT 'Students', COUNT(*) FROM [Students]
UNION ALL SELECT 'Courses', COUNT(*) FROM [Courses]
UNION ALL SELECT 'CourseOfferings', COUNT(*) FROM [CourseOfferings]
UNION ALL SELECT 'Enrollments', COUNT(*) FROM [Enrollments];
GO

PRINT '';
PRINT 'Login Credentials:';
PRINT '  Admin:   admin / admin123';
PRINT '  Faculty: faculty1 / faculty123';
PRINT '  Faculty: faculty2 / faculty123';
PRINT '  Student: student1 / student123';
GO

