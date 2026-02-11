-- LifeOS Database Migration Script
-- Purpose: Add missing Streak column to DailyHabits table
-- Date: Generated for schema sync

USE [LifeOSDB]
GO

-- Add Streak column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[DailyHabits]') AND name = 'Streak')
BEGIN
    ALTER TABLE [dbo].[DailyHabits]
    ADD [Streak] INT NOT NULL DEFAULT 0
    PRINT 'Streak column added successfully'
END
ELSE
BEGIN
    PRINT 'Streak column already exists'
END
GO

-- Ensure all required columns exist in DailyHabits table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[DailyHabits]') AND name = 'HabitId')
BEGIN
    PRINT 'ERROR: DailyHabits table structure is incomplete. Please run the full schema script.'
END
GO
