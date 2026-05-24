USE master;
GO

IF DB_ID(N'ApbdLecture9DbFirstTask') IS NOT NULL
BEGIN
    ALTER DATABASE ApbdLecture9DbFirstTask SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ApbdLecture9DbFirstTask;
END
GO

CREATE DATABASE ApbdLecture9DbFirstTask;
GO

USE ApbdLecture9DbFirstTask;
GO

CREATE TABLE dbo.Students
(
    StudentId INT IDENTITY(1,1) CONSTRAINT PK_Students PRIMARY KEY,
    IndexNumber NVARCHAR(20) NOT NULL,
    FirstName NVARCHAR(80) NOT NULL,
    LastName NVARCHAR(80) NOT NULL,
    Email NVARCHAR(160) NOT NULL,
    EnrollmentDate DATE NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Students_IsActive DEFAULT 1,
    CONSTRAINT UQ_Students_IndexNumber UNIQUE (IndexNumber),
    CONSTRAINT UQ_Students_Email UNIQUE (Email)
);

CREATE TABLE dbo.Courses
(
    CourseId INT IDENTITY(1,1) CONSTRAINT PK_Courses PRIMARY KEY,
    Code NVARCHAR(20) NOT NULL,
    Name NVARCHAR(160) NOT NULL,
    Credits INT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Courses_IsActive DEFAULT 1,
    CONSTRAINT UQ_Courses_Code UNIQUE (Code),
    CONSTRAINT CK_Courses_Credits CHECK (Credits BETWEEN 1 AND 10)
);

CREATE TABLE dbo.Enrollments
(
    EnrollmentId INT IDENTITY(1,1) CONSTRAINT PK_Enrollments PRIMARY KEY,
    StudentId INT NOT NULL,
    CourseId INT NOT NULL,
    EnrolledAt DATE NOT NULL,
    Status NVARCHAR(30) NOT NULL,
    CONSTRAINT FK_Enrollments_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId),
    CONSTRAINT FK_Enrollments_Courses FOREIGN KEY (CourseId) REFERENCES dbo.Courses(CourseId),
    CONSTRAINT UQ_Enrollments_Student_Course UNIQUE (StudentId, CourseId),
    CONSTRAINT CK_Enrollments_Status CHECK (Status IN (N'Active', N'Completed', N'Dropped'))
);

CREATE TABLE dbo.Assignments
(
    AssignmentId INT IDENTITY(1,1) CONSTRAINT PK_Assignments PRIMARY KEY,
    CourseId INT NOT NULL,
    Title NVARCHAR(160) NOT NULL,
    Description NVARCHAR(1000) NULL,
    DueDate DATETIME2 NOT NULL,
    MaxPoints INT NOT NULL,
    IsPublished BIT NOT NULL CONSTRAINT DF_Assignments_IsPublished DEFAULT 0,
    CONSTRAINT FK_Assignments_Courses FOREIGN KEY (CourseId) REFERENCES dbo.Courses(CourseId),
    CONSTRAINT CK_Assignments_MaxPoints CHECK (MaxPoints > 0)
);

CREATE TABLE dbo.Submissions
(
    SubmissionId INT IDENTITY(1,1) CONSTRAINT PK_Submissions PRIMARY KEY,
    AssignmentId INT NOT NULL,
    StudentId INT NOT NULL,
    RepositoryUrl NVARCHAR(300) NOT NULL,
    SubmittedAt DATETIME2 NOT NULL,
    Score INT NULL,
    Feedback NVARCHAR(1000) NULL,
    Status NVARCHAR(30) NOT NULL,
    CONSTRAINT FK_Submissions_Assignments FOREIGN KEY (AssignmentId) REFERENCES dbo.Assignments(AssignmentId),
    CONSTRAINT FK_Submissions_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId),
    CONSTRAINT UQ_Submissions_Assignment_Student UNIQUE (AssignmentId, StudentId),
    CONSTRAINT CK_Submissions_Status CHECK (Status IN (N'Submitted', N'Late', N'Graded')),
    CONSTRAINT CK_Submissions_Score CHECK (Score IS NULL OR Score >= 0)
);
GO

CREATE INDEX IX_Enrollments_CourseId ON dbo.Enrollments(CourseId);
CREATE INDEX IX_Assignments_CourseId ON dbo.Assignments(CourseId);
CREATE INDEX IX_Assignments_DueDate ON dbo.Assignments(DueDate);
CREATE INDEX IX_Submissions_StudentId ON dbo.Submissions(StudentId);
CREATE INDEX IX_Submissions_AssignmentId ON dbo.Submissions(AssignmentId);
CREATE INDEX IX_Submissions_Status ON dbo.Submissions(Status);
GO