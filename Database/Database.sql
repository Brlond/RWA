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
    Description NVARCHAR(MAX),
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
INSERT INTO [dbo].[User] (Username, PasswordHash, PasswordSalt, FirstName, LastName, Email, IsAdmin)
VALUES 
('admin', N'e/guJOH62Pv5WPE2T4/Qb38bkMoxx7xTXfs7p6GFb2w=', N'mHLo5lwiTwQspVDoIdvbxQ==', 'admnin', 'admnin', 'admin@example.com', 1);
GO

-- Insert Tags
INSERT INTO Tag (Name)
VALUES ('2D'), ('3D'), ('Tips'), ('Mobile'), ('Desktop'), ('Unity'), ('Unreal'), ('Godot');
GO

-- Insert Topics
INSERT INTO Topic (Title, Description, CategoryId) VALUES
('Optimizing performance for mobile devices', 'Best practices to enhance performance on smartphones.', 2),
('Effective storyboarding for 2D adventure games', 'How to create a compelling story flow.', 1),
('Memory management in open-world games', 'Avoid crashes and improve load times with memory pools.', 2),
('Designing intuitive game UI', 'Make interfaces accessible and user-friendly.', 1),
('Using particle systems efficiently', 'Add visual effects without killing performance.', 2),
('Narrative branching with decision trees', 'How to build a story that reacts to players.', 1),
('Multiplayer lobby design', 'Create reliable matchmaking and room systems.', 3),
('Balancing game difficulty', 'Keep your players challenged but not frustrated.', 1),
('Procedural generation in indie games', 'Techniques to create content automatically.', 3),
('Audio layering for immersive soundscapes', 'Combine ambient, SFX, and music like a pro.', 2),
('Handling input across platforms', 'Support keyboard, controller, and touch input.', 2),
('Reward systems that retain players', 'Gamification techniques that actually work.', 3),
('Boss fight mechanics design', 'Make epic, memorable and fair boss encounters.', 1);
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
(1, 1, 'Try using GPU instancing to reduce draw calls.', 1),
(1, 2, 'Sketch each major scene before animating to save time.', 1);
GO
INSERT INTO [dbo].[Logs] ([DateOf], [Severity], [Message], [ErrorText]) VALUES
(GETDATE(), 1, 'System started.', 'Usererror'),
(GETDATE(), 2, 'User login failed.', 'Usererror'),
(GETDATE(), 3, 'Database timeout.', 'Error'),
(GETDATE(), 1, 'Health check passed.', 'Usererror'),
(GETDATE(), 4, 'Unhandled exception.', 'Error'),
(GETDATE(), 2, 'User logout.', 'Usererror'),
(GETDATE(), 3, 'Service unavailable.', 'Error'),
(GETDATE(), 1, 'Cache cleared.', 'Usererror'),
(GETDATE(), 4, 'Disk space low.', 'Error'),
(GETDATE(), 2, 'Email sent.', 'Usererror'),
(GETDATE(), 1, 'Task completed.', 'Usererror'),
(GETDATE(), 3, 'Deadlock detected.', 'Error'),
(GETDATE(), 2, 'Permission denied.', 'Usererror'),
(GETDATE(), 1, 'Session started.', 'Usererror'),
(GETDATE(), 4, 'Data corruption detected.', 'Error'),
(GETDATE(), 3, 'Transaction aborted.', 'Error'),
(GETDATE(), 2, 'Invalid input.', 'Usererror'),
(GETDATE(), 1, 'Configuration loaded.', 'Usererror'),
(GETDATE(), 4, 'Out of memory.', 'Error'),
(GETDATE(), 3, 'Timeout contacting server.', 'Error'),
(GETDATE(), 2, 'Password reset requested.', 'Usererror'),
(GETDATE(), 1, 'New user registered.', 'Usererror'),
(GETDATE(), 3, 'Thread limit exceeded.', 'Error'),
(GETDATE(), 4, 'Stack overflow.', 'Error'),
(GETDATE(), 2, 'Token expired.', 'Usererror'),
(GETDATE(), 1, 'Language set to EN.', 'Usererror'),
(GETDATE(), 3, 'Connection pool exhausted.', 'Error'),
(GETDATE(), 2, 'Access revoked.', 'Usererror'),
(GETDATE(), 4, 'Kernel panic.', 'Error'),
(GETDATE(), 3, 'File lock timeout.', 'Error'),
(GETDATE(), 2, 'Too many requests.', 'Usererror'),
(GETDATE(), 1, 'Daily report generated.', 'Usererror'),
(GETDATE(), 4, 'Corrupted metadata.', 'Error'),
(GETDATE(), 3, 'SSL handshake failed.', 'Error'),
(GETDATE(), 2, 'Captcha failed.', 'Usererror'),
(GETDATE(), 1, 'System idle.', 'Usererror'),
(GETDATE(), 3, 'Replication lag detected.', 'Error'),
(GETDATE(), 4, 'Memory leak.', 'Error'),
(GETDATE(), 2, 'Form submission error.', 'Usererror'),
(GETDATE(), 1, 'Locale updated.', 'Usererror'),
(GETDATE(), 3, 'Queue overflow.', 'Error'),
(GETDATE(), 4, 'Segmentation fault.', 'Error'),
(GETDATE(), 2, 'Invalid email format.', 'Usererror'),
(GETDATE(), 1, 'Widget loaded.', 'Usererror'),
(GETDATE(), 3, 'File not found.', 'Error'),
(GETDATE(), 2, 'Missing fields.', 'Usererror'),
(GETDATE(), 4, 'Infinite loop detected.', 'Error'),
(GETDATE(), 3, 'Unexpected shutdown.', 'Error'),
(GETDATE(), 2, 'Unverified email.', 'Usererror'),
(GETDATE(), 1, 'Restart completed.', 'Usererror');