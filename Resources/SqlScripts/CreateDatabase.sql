IF DB_ID('SyncCore') IS NULL
BEGIN
    CREATE DATABASE SyncCore;
END
GO

USE SyncCore;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SyncConfig]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SyncConfig](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        [SourceServer] [nvarchar](100) NOT NULL,
        [SourceDatabase] [nvarchar](100) NOT NULL,
        [TargetServer] [nvarchar](100) NOT NULL,
        [TargetDatabase] [nvarchar](100) NOT NULL,
        [Schedule] [nvarchar](100) NULL,
        [LastSync] [datetime] NULL,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_SyncConfig] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO 