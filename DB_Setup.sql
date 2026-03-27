-- ================================================================
--  PRSC_Auction_DB  ─  Database Setup Script
--  Run this once on: DESKTOP-BF5OMUT\SQLEXPRESS
-- ================================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'PRSC_Auction_DB')
    CREATE DATABASE PRSC_Auction_DB;
GO

USE PRSC_Auction_DB;
GO

-- ── Players table ─────────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Players')
BEGIN
    CREATE TABLE Players (
        Id           INT           PRIMARY KEY IDENTITY(1,1),
        Name         NVARCHAR(100) NOT NULL,
        Position     NVARCHAR(60)  NOT NULL DEFAULT '',
        SkillLevel   NVARCHAR(20)  NOT NULL DEFAULT 'Medium',
        BasePrice    DECIMAL(18,2) NOT NULL DEFAULT 0,
        SoldPrice    DECIMAL(18,2) NOT NULL DEFAULT 0,
        AssignedTeam NVARCHAR(60)  NOT NULL DEFAULT N'—',
        IsSold       BIT           NOT NULL DEFAULT 0,
        VideoPath    NVARCHAR(500)     NULL
    );
    PRINT 'Players table created.';
END
ELSE
BEGIN
    -- Add any missing columns if upgrading from old schema
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                   WHERE TABLE_NAME='Players' AND COLUMN_NAME='Position')
        ALTER TABLE Players ADD Position NVARCHAR(60) NOT NULL DEFAULT '';

    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                   WHERE TABLE_NAME='Players' AND COLUMN_NAME='SkillLevel')
        ALTER TABLE Players ADD SkillLevel NVARCHAR(20) NOT NULL DEFAULT 'Medium';

    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                   WHERE TABLE_NAME='Players' AND COLUMN_NAME='SoldPrice')
        ALTER TABLE Players ADD SoldPrice DECIMAL(18,2) NOT NULL DEFAULT 0;

    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                   WHERE TABLE_NAME='Players' AND COLUMN_NAME='AssignedTeam')
        ALTER TABLE Players ADD AssignedTeam NVARCHAR(60) NOT NULL DEFAULT N'—';

    PRINT 'Players table updated.';
END
GO

-- ── Settings table ────────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Settings')
BEGIN
    CREATE TABLE Settings (
        SettingName  NVARCHAR(100) PRIMARY KEY,
        SettingValue NVARCHAR(500) NOT NULL DEFAULT ''
    );
    PRINT 'Settings table created.';
END
GO

-- ── Seed default team funds ───────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM Settings WHERE SettingName = 'Team AlphaFund')
    INSERT INTO Settings VALUES ('Team AlphaFund', '100000');

IF NOT EXISTS (SELECT 1 FROM Settings WHERE SettingName = 'Team BetaFund')
    INSERT INTO Settings VALUES ('Team BetaFund', '100000');
GO

PRINT 'Setup complete. Database ready.';
