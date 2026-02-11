-- LifeOS Complete Database Schema
-- ASP.NET MVC Life Management System

USE master
GO

-- Create database if not exists
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'LifeOSDB')
BEGIN
    CREATE DATABASE [LifeOSDB]
    PRINT 'Database LifeOSDB created successfully'
END
GO

USE [LifeOSDB]
GO

-- Users Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [UserId] INT PRIMARY KEY IDENTITY(1,1),
        [Name] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(100) NOT NULL UNIQUE,
        [PasswordHash] NVARCHAR(256) NOT NULL,
        [CreatedAt] DATETIME DEFAULT GETDATE()
    )
    PRINT 'Users table created'
END
GO

-- DailyHabits Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DailyHabits]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DailyHabits] (
        [HabitId] INT PRIMARY KEY IDENTITY(1,1),
        [UserId] INT NOT NULL,
        [HabitName] NVARCHAR(200) NOT NULL,
        [TargetValue] NVARCHAR(100),
        [HabitDate] DATE NOT NULL,
        [IsCompleted] BIT DEFAULT 0,
        [Streak] INT DEFAULT 0,
        FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE
    )
    PRINT 'DailyHabits table created'
END
GO

-- Tasks Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tasks]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Tasks] (
        [TaskId] INT PRIMARY KEY IDENTITY(1,1),
        [UserId] INT NOT NULL,
        [Title] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(MAX),
        [DueDateTime] DATETIME NOT NULL,
        [Priority] NVARCHAR(20) DEFAULT 'Medium',
        [Status] NVARCHAR(20) DEFAULT 'Pending',
        [CreatedAt] DATETIME DEFAULT GETDATE(),
        FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE
    )
    PRINT 'Tasks table created'
END
GO

-- Expenses Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Expenses]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Expenses] (
        [ExpenseId] INT PRIMARY KEY IDENTITY(1,1),
        [UserId] INT NOT NULL,
        [Reason] NVARCHAR(200) NOT NULL,
        [Category] NVARCHAR(100),
        [Amount] DECIMAL(18,2) NOT NULL,
        [ExpenseDate] DATETIME DEFAULT GETDATE(),
        FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE
    )
    PRINT 'Expenses table created'
END
GO

-- Incomes Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Incomes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Incomes] (
        [IncomeId] INT PRIMARY KEY IDENTITY(1,1),
        [UserId] INT NOT NULL,
        [Month] INT NOT NULL,
        [Year] INT NOT NULL,
        [MonthlyIncome] DECIMAL(18,2) NOT NULL,
        FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE
    )
    PRINT 'Incomes table created'
END
GO

-- FocusLogs Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FocusLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FocusLogs] (
        [FocusId] INT PRIMARY KEY IDENTITY(1,1),
        [UserId] INT NOT NULL,
        [Category] NVARCHAR(100) NOT NULL,
        [MinutesSpent] INT NOT NULL,
        [LogDate] DATETIME DEFAULT GETDATE(),
        FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE
    )
    PRINT 'FocusLogs table created'
END
GO

-- MoodLogs Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MoodLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[MoodLogs] (
        [MoodId] INT PRIMARY KEY IDENTITY(1,1),
        [UserId] INT NOT NULL,
        [MoodLevel] INT NOT NULL CHECK ([MoodLevel] BETWEEN 1 AND 5),
        [MoodDate] DATE NOT NULL,
        FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE
    )
    PRINT 'MoodLogs table created'
END
GO

-- Create indexes for better performance
CREATE NONCLUSTERED INDEX [IX_DailyHabits_UserId_Date] ON [dbo].[DailyHabits]([UserId], [HabitDate])
CREATE NONCLUSTERED INDEX [IX_Tasks_UserId_Status] ON [dbo].[Tasks]([UserId], [Status])
CREATE NONCLUSTERED INDEX [IX_Expenses_UserId_Date] ON [dbo].[Expenses]([UserId], [ExpenseDate])
GO

PRINT 'LifeOS Database Schema Created Successfully!'
