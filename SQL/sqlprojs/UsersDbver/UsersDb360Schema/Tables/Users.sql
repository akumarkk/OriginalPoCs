CREATE TABLE [dbo].[Users]
(
    [Id] INT PRIMARY KEY,
    [FullName] NVARCHAR(100), -- Updated from UserName
    -- Add your new column here:
    [Email] NVARCHAR(256) NULL
)