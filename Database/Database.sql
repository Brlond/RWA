-- Switch to master before creating the DB
--USE [master];
--GO

---- Create the database
--CREATE DATABASE RWA;
--GO

--USE RWA;
--GO

-- Category table
CREATE TABLE Category (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL
);
GO

-- User table
CREATE TABLE [dbo].[User] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
	PasswordSalt NVARCHAR(255) NOT NULL,
	FirstName NVARCHAR(256) NOT NULL,
	LastName NVARCHAR(256) NOT NULL,
    Email NVARCHAR(256),
    IsAdmin BIT DEFAULT 0
);

GO

-- Topic table
CREATE TABLE Topic (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Description TEXT,
    CategoryId INT,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CategoryId) REFERENCES Category(Id)
);
GO

-- Tag table
CREATE TABLE Tag (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(50) NOT NULL UNIQUE
);
GO

-- Topic-Tag join table
CREATE TABLE TopicTag (
    TopicId INT NOT NULL,
    TagId INT NOT NULL,
    PRIMARY KEY (TopicId, TagId),
    FOREIGN KEY (TopicId) REFERENCES Topic(Id) ON DELETE CASCADE,
    FOREIGN KEY (TagId) REFERENCES Tag(Id) ON DELETE CASCADE
);
GO

-- Post table
CREATE TABLE Post (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    TopicId INT,
    Content NVARCHAR(MAX) NOT NULL,
    PostedAt DATETIME DEFAULT GETDATE(),
    Approved BIT DEFAULT 0,
    FOREIGN KEY (UserId) REFERENCES [dbo].[User](Id),
    FOREIGN KEY (TopicId) REFERENCES Topic(Id)
);
GO
CREATE TABLE Logs (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	DateOf Datetime,
	Severity INT CHECK(Severity BETWEEN 1 AND 5),
	Message NVARCHAR(1024),
	ErrorText NVARCHAR(MAX)
);

GO


-- Rating table
CREATE TABLE Rating (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    PostId INT,
    Score INT CHECK (Score BETWEEN 1 AND 5),
    RatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT UQ_User_Post UNIQUE (UserId, PostId),
    FOREIGN KEY (UserId) REFERENCES [dbo].[User](Id),
    FOREIGN KEY (PostId) REFERENCES Post(Id)
);
GO
-- Sample data
-- Insert Categories
INSERT INTO Category (Name) VALUES ('Design'), ('Programming'), ('Modeling'), ('Animation'), ('Sound'), ('Interactivity');
GO

-- Insert Users
INSERT INTO [dbo].[User](Username, PasswordHash,PasswordSalt,FirstName,LastName, Email, IsAdmin)
VALUE
('admin', N'e/guJOH62Pv5WPE2T4/Qb38bkMoxx7xTXfs7p6GFb2w=', N'mHLo5lwiTwQspVDoIdvbxQ==' ,'admnin','admnin', 'admin@example.com', 1),
GO

-- Insert Tags
INSERT INTO Tag (Name)
VALUES ('2D'), ('3D'), ('Tips'), ('Mobile'), ('Desktop'), ('Unity'), ('Unreal'), ('Godot');
GO

-- Insert Topics
INSERT INTO Topic (Title, Description, CategoryId)
VALUES 
('Optimizing performance for mobile devices', 'Best practices to enhance performance on smartphones.', 2),
('Effective storyboarding for 2D adventure games', 'How to create a compelling story flow.', 1);
GO

-- Insert TopicTag links
INSERT INTO TopicTag (TopicId, TagId)
VALUES 
(1, 4), (1, 6), 
(2, 1), (2, 3);
GO

-- Insert Posts
INSERT INTO Post (UserId, TopicId, Content, Approved)
VALUES 
(2, 1, 'Try using GPU instancing to reduce draw calls.', 1),
(3, 2, 'Sketch each major scene before animating to save time.', 1);
GO

-- Insert Ratings
INSERT INTO Rating (UserId, PostId, Score)
VALUES 
(1, 1, 5),
(2, 2, 4);
GO