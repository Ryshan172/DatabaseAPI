--- Run this script with SSMS to create the database 
USE master;  
GO  
CREATE DATABASE BursaryDatabase 
GO


USE BursaryDatabase
GO

CREATE FUNCTION dbo.CalculateAge
(
    @DateOfBirth date
)
RETURNS INT
AS
BEGIN
    DECLARE @Result INT;

    SET @Result = DATEDIFF(day,@DateOfBirth,CAST (GETDATE() AS DATE ))

    RETURN @Result/365.25;
END;
GO

/*Updated Roles Table*/
-- Creating Roles Table 
CREATE TABLE [dbo].Roles (
RoleID int PRIMARY KEY IDENTITY(1,1) ,
RoleName varchar(10),
);
GO
INSERT INTO dbo.Roles(RoleName) VALUES ('BBDAdmin'),('University'),('Student')
GO

-- Creating Users Table 
CREATE TABLE [dbo].Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FirstName VARCHAR(255) NOT NULL,
    LastName VARCHAR(255) NOT NULL,
    RoleID INT NOT NULL,
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);
GO

/*Contact Details Table*/
CREATE TABLE ContactDetails (
    ContactID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    Email VARCHAR(255) ,
    PhoneNumber VARCHAR(20),
    CONSTRAINT FK_UserContact FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

/*Added Ethnicity, Dept and Genders tables*/
CREATE TABLE [dbo].Ethnicity(
EthnicityID int PRIMARY KEY IDENTITY(1,1) ,
Ethnicity VARCHAR(8)
);
GO
INSERT INTO dbo.Ethnicity(Ethnicity) VALUES ('African'),('Indian'),('Colored')
GO

CREATE TABLE [dbo].Genders(
GenderID int PRIMARY KEY IDENTITY(1,1) ,
Gender VARCHAR(6)
);
GO
INSERT INTO dbo.Genders(Gender) VALUES ('Female'),('Male')


CREATE TABLE [dbo].Departments(
DepartmentID int PRIMARY KEY IDENTITY(1,1) ,
Department varchar(7)
);
GO
INSERT INTO dbo.Departments(Department) VALUES ('ComSci'),('BEng'),('GameDev')
GO

-- Creating Universities Table 
CREATE TABLE [dbo].[Universities] (
    [UniversityID] INT PRIMARY KEY IDENTITY(1,1),
    [UniName] VARCHAR(200),
);
GO
/*Foreign Key Conflict when trying to Insert a University. This also Affects Adding Students
INSERT INTO dbo.Universities (UniName, DepartmentID, UserID) VALUES ('Wits', 1, 2);
*/

CREATE TABLE [dbo].[UniversityUser] (
    DepartmentID int REFERENCES [dbo].Departments(DepartmentID) ,
    UniversityID int REFERENCES [dbo].Universities(UniversityID) ,
    UserID int REFERENCES Users (UserID)
);
GO


--Create aplication status table
CREATE TABLE ApplicationStatuses (
    StatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName VARCHAR(50) NOT NULL
);
INSERT INTO dbo.ApplicationStatuses (StatusName) 
VALUES 
    ('Pending'),
    ('Accepted'),
    ('Rejected');

-- Creating University Application Table
CREATE TABLE [dbo].[UniversityApplication] (
    ApplicationID INT PRIMARY KEY IDENTITY(1,1),
    ApplicationStatusID INT,
    AmountRequested money,
    UniversityID INT REFERENCES Universities(UniversityID),
    CONSTRAINT FK_ApplicationStatus FOREIGN KEY (ApplicationStatusID) REFERENCES ApplicationStatuses(StatusID)
);

-- Creating Bursary Allocations Table
CREATE TABLE BursaryAllocations (
    AllocationID INT PRIMARY KEY IDENTITY,
    UniversityID INT,
    AmountAlloc MONEY,
    AllocationYear INT,
    FOREIGN KEY (UniversityID) REFERENCES Universities(UniversityID),
	-- Needs to be more than 0 
    CONSTRAINT CHK_AmountAlloc CHECK (AmountAlloc >= 0)
);
GO

-- Creating BBDAdminBalance Table
CREATE TABLE [dbo].BBDAdminBalance(
BalanceID int PRIMARY KEY IDENTITY(1,1),
Budget money,
AmountRemaining AS (Budget - AmountAllocated),
AmountAllocated money
);
GO





/* Updated Students Table with field changes */
-- Creating Students Table
CREATE TABLE [dbo].StudentsTable(
	 -- Remove IDENTITY property. Can not manually enter ID 
    StudentID int PRIMARY KEY,
    UserID int NOT NULL,
    DateOfBirth date NOT NULL,
    GenderID INT REFERENCES Genders(GenderID), -- Assuming Genders table exists
    EthnicityID int REFERENCES [dbo].Ethnicity(EthnicityID),
    DepartmentID int REFERENCES [dbo].Departments(DepartmentID) ,
    UniversityID int REFERENCES [dbo].Universities(UniversityID), -- New column
    CONSTRAINT FK_StudentUser FOREIGN KEY (UserID) REFERENCES Users(UserID)
);


-- Creating Student Allocations Table
CREATE TABLE [dbo].[StudentAllocations](
    AllocationID int PRIMARY KEY  identity(1,1),
    Amount money NOT NULL,
    AllocationYear INT,
    StudentID int REFERENCES [dbo].StudentsTable(StudentID)
);
GO

-- Creating Student Documents Table
CREATE TABLE [dbo].Documents (
    CV VARBINARY(1000),
    ID VARBINARY(1000),
    AcademicTranscript VARBINARY(1000),
    StudentID int REFERENCES [dbo].StudentsTable(StudentID)
);
GO

/* 
 SELECT * FROM dbo.Users
 SELECT * FROM dbo.Roles
 SELECT * FROM dbo.StudentsTable
 SELECT * FROM dbo.Universities
 SELECT * FROM dbo.UniversityUser
 SELECT * FROM dbo.BursaryAllocations
 SELECT * FROM dbo.UniversityApplication
 SELECT * FROM dbo.BBDAdminBalance
 SELECT * FROM dbo.Ethnicity
 SELECT * FROM dbo.Genders
 SELECT * FROM dbo.Departments
 SELECT * FROM dbo.ContactDetails
 SELECT * FROM dbo.Documents
 */
 

