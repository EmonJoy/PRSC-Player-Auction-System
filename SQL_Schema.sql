USE [master]
GO

-- Create the database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'PRSC_Auction_DB')
BEGIN
    CREATE DATABASE [PRSC_Auction_DB]
END
GO

USE [PRSC_Auction_DB]
GO

-- Create Players table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Players' and xtype='U')
BEGIN
    CREATE TABLE [dbo].[Players](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Name] [nvarchar](255) NOT NULL,
        [BasePrice] [decimal](18, 2) NOT NULL,
        [VideoPath] [nvarchar](MAX) NULL,
        [IsSold] [bit] NOT NULL,
     CONSTRAINT [PK_Players] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

-- Create Settings table for team funds
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Settings' and xtype='U')
BEGIN
    CREATE TABLE [dbo].[Settings](
        [SettingName] [nvarchar](50) NOT NULL,
        [SettingValue] [nvarchar](MAX) NULL,
     CONSTRAINT [PK_Settings] PRIMARY KEY CLUSTERED 
    (
        [SettingName] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

-- Insert initial funds if not present
IF NOT EXISTS (SELECT * FROM [dbo].[Settings] WHERE SettingName = 'TeamAFund')
BEGIN
    INSERT INTO [dbo].[Settings] (SettingName, SettingValue) VALUES ('TeamAFund', '100000.00')
END

IF NOT EXISTS (SELECT * FROM [dbo].[Settings] WHERE SettingName = 'TeamBFund')
BEGIN
    INSERT INTO [dbo].[Settings] (SettingName, SettingValue) VALUES ('TeamBFund', '100000.00')
END
GO
