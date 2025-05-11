CREATE TABLE [dbo].[AlertTable] (
    [AlertId] INT IDENTITY(1,1) PRIMARY KEY,
    [LocationId] INT NOT NULL,
    [Date_Time] DATETIME NOT NULL,
    [Parameter] NVARCHAR(50) NOT NULL,
    [Value] FLOAT NULL,
    [Deviation] FLOAT NULL,
    [Message] NVARCHAR(255) NOT NULL,
    [CreatedAt] DATETIME DEFAULT GETDATE(),
    [IsResolved] BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_AlertTable_Location FOREIGN KEY ([LocationId])
        REFERENCES [dbo].Locations
);
