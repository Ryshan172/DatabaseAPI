--- Run this script with SSMS to create the database 
-- For Azure DB, Take out Use master and Use BursaryDatabase, since you should already be in the DB 
USE master;  
GO  
CREATE DATABASE BursaryDatabase 
GO


USE BursaryDatabase
GO


/*Updated Roles Table*/
-- Creating Roles Table 
CREATE TABLE [dbo].Roles (
RoleID INT PRIMARY KEY CLUSTERED IDENTITY(1,1) ,
RoleName varchar(10),
);
GO
INSERT INTO dbo.Roles(RoleName) VALUES ('BBDAdmin'),('University'),('Student')
GO

-- Creating Users Table 
CREATE TABLE [dbo].Users (
    UserID INT PRIMARY KEY CLUSTERED IDENTITY(1,1),
    FirstName VARCHAR(255) NOT NULL,
    LastName VARCHAR(255) NOT NULL,
    RoleID INT NOT NULL,
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);
GO


/*Contact Details Table*/
CREATE TABLE [dbo].ContactDetails (
    ContactID INT PRIMARY KEY CLUSTERED IDENTITY(1,1),
    UserID INT NOT NULL,
    Email VARCHAR(255) ,
    PhoneNumber VARCHAR(20),
    CONSTRAINT FK_UserContact FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

/*Added Ethnicity, Dept and Genders tables*/
CREATE TABLE [dbo].Ethnicity(
EthnicityID INT PRIMARY KEY CLUSTERED IDENTITY(1,1) ,
Ethnicity VARCHAR(8)
);
GO
INSERT INTO [dbo].Ethnicity(Ethnicity) VALUES ('African'),('Indian'),('Colored')
GO

CREATE TABLE [dbo].Genders(
GenderID INT PRIMARY KEY CLUSTERED IDENTITY(1,1) ,
Gender VARCHAR(6)
);
GO
INSERT INTO dbo.Genders(Gender) VALUES ('Female'),('Male')


CREATE TABLE [dbo].Departments(
DepartmentID INT PRIMARY KEY CLUSTERED IDENTITY(1,1) ,
Department varchar(7)
);
GO
INSERT INTO dbo.Departments(Department) VALUES ('ComSci'),('BEng'),('GameDev')
GO

-- Creating Universities Table 
CREATE TABLE [dbo].[Universities] (
    [UniversityID] INT PRIMARY KEY CLUSTERED IDENTITY(1,1),
    [UniName] VARCHAR(200),
);
GO


CREATE TABLE [dbo].[UniversityUser] (
    DepartmentID INT REFERENCES [dbo].Departments(DepartmentID) ,
    UniversityID INT REFERENCES [dbo].Universities(UniversityID) ,
    UserID INT REFERENCES Users (UserID)
);
GO


--Create aplication status table
CREATE TABLE [dbo].ApplicationStatuses (
    StatusID INT PRIMARY KEY CLUSTERED IDENTITY(1,1),
    StatusName VARCHAR(50) NOT NULL
);
INSERT INTO dbo.ApplicationStatuses (StatusName) 
VALUES 
    ('Pending'),
    ('Accepted'),
    ('Rejected');


-- Creating University Application Table
CREATE TABLE [dbo].[UniversityApplication] (
    ApplicationID INT PRIMARY KEY CLUSTERED IDENTITY(1,1),
    ApplicationStatusID INT,
    AmountRequested money,
    UniversityID INT REFERENCES Universities(UniversityID),
    ApplicationYear INT,
    IsLocked BIT NOT NULL DEFAULT 0, -- Adding IsLocked field, Default 0 = false
    CONSTRAINT FK_ApplicationStatus FOREIGN KEY (ApplicationStatusID) REFERENCES ApplicationStatuses(StatusID),
    CONSTRAINT CHK_AmountAllocVal CHECK (AmountRequested >= 0), -- no negatives 
    CONSTRAINT CHK_AppYear CHECK (ApplicationYear >= 0)
);

-- Creating Bursary Allocations Table
CREATE TABLE BursaryAllocations (
    AllocationID INT PRIMARY KEY CLUSTERED IDENTITY,
    UniversityID INT,
    AmountAlloc MONEY,
    AllocationYear INT,
    UniversityApplicationID INT,
    FOREIGN KEY (UniversityID) REFERENCES Universities(UniversityID),
    CONSTRAINT FK_BursLinkID FOREIGN KEY (UniversityApplicationID) REFERENCES UniversityApplication(ApplicationID),
	-- Needs to be more than 0 
    CONSTRAINT CHK_AmountAlloc CHECK (AmountAlloc >= 0),
    CONSTRAINT CHK_BursAllocationYear CHECK (AllocationYear >= 0)
);
GO

-- Creating BBDAdminBalance Table
CREATE TABLE [dbo].BBDAdminBalance(
BalanceID INT PRIMARY KEY CLUSTERED IDENTITY(1,1),
Budget money,
AmountRemaining AS (Budget - AmountAllocated),
AmountAllocated money,
BudgetYear INT, -- Added year for budget 
CONSTRAINT CHK_NonNegativeMoney CHECK (Budget >= 0 AND AmountAllocated >= 0), -- Constraint to prevent negative money values
CONSTRAINT CHK_BudgetYear CHECK (BudgetYear >= 0)
);
GO



/* Updated Students Table with field changes */
-- Creating Students Table
CREATE TABLE [dbo].StudentsTable(
	 -- Remove IDENTITY property. Can not manually enter ID 
    StudentIDNum CHAR(13) PRIMARY KEY CLUSTERED, -- StudentID as char of size 13
    UserID INT NOT NULL,
    DateOfBirth DATE NOT NULL,
    GenderID INT REFERENCES Genders(GenderID), -- Assuming Genders table exists
    EthnicityID INT REFERENCES [dbo].Ethnicity(EthnicityID),
    DepartmentID INT REFERENCES [dbo].Departments(DepartmentID),
    UniversityID INT REFERENCES [dbo].Universities(UniversityID), -- New column
    CONSTRAINT FK_StudentUser FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CONSTRAINT CHK_DateOfBirth CHECK (DateOfBirth <= SYSDATETIME()) -- Constraint to prevent future DOB
);


-- Creating Student Allocations Table
CREATE TABLE [dbo].[StudentAllocations](
    AllocationID INT PRIMARY KEY CLUSTERED  identity(1,1),
    Amount money NOT NULL,
    AllocationYear INT,
    StudentIDNum CHAR(13) REFERENCES [dbo].StudentsTable(StudentIDNum),
    StudentMarks INT, 
    CourseYear INT,
    ApplicationStatusID INT
    CONSTRAINT FK_StudentAppStatus FOREIGN KEY (ApplicationStatusID) REFERENCES ApplicationStatuses(StatusID),
    CONSTRAINT CHK_AmountVal CHECK (Amount >= 0), -- no negatives 
    CONSTRAINT CHK_AllocationYear CHECK (AllocationYear >= 0),
    CONSTRAINT CHK_CourseYear CHECK (CourseYear >= 0)
);
GO

-- Creating Student Documents Table
CREATE TABLE [dbo].Documents (
    ID NVARCHAR(1000), -- URL for Document
    AcademicTranscript NVARCHAR(1000), 
    StudentIDNum CHAR(13) REFERENCES [dbo].StudentsTable(StudentIDNum)
);
GO

CREATE TABLE [dbo].TemporaryLinks (
    LinkID INT PRIMARY KEY CLUSTERED identity(1,1),
    StudentIDNum CHAR(13) REFERENCES [dbo].StudentsTable(StudentIDNum),
    LinkUrl NVARCHAR(1000), -- URL for link, 
    ExpiryDate DATE
    
)

-- Creating Reviewers Table 
CREATE TABLE [dbo].Reviewers (
    ReviewerID INT PRIMARY KEY CLUSTERED IDENTITY(1,1),
    UserID INT REFERENCES Users (UserID),
    StudentAllocationID INT NULL,
    UniversityApplicationID INT NULL,
    CONSTRAINT FK_StudnetAlloc FOREIGN KEY (StudentAllocationID) REFERENCES StudentAllocations(AllocationID),
    CONSTRAINT FK_UniversityApplicationID FOREIGN KEY (UniversityApplicationID) REFERENCES UniversityApplication(ApplicationID)
)


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
 

