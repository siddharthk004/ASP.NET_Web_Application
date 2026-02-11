-- LifeOS Sample Data Script
-- Populate database with realistic sample data for testing

USE [LifeOSDB]
GO

-- Get the test user ID
DECLARE @UserId INT
SELECT @UserId = UserId FROM Users WHERE Email = 'admin@lifeos.com'

IF @UserId IS NULL
BEGIN
    PRINT 'Error: User admin@lifeos.com not found. Please run InsertTestUsers.sql first.'
    RETURN
END

PRINT 'Adding sample data for User ID: ' + CAST(@UserId AS VARCHAR)

-- Clear existing data for test user
DELETE FROM MoodLogs WHERE UserId = @UserId
DELETE FROM FocusLogs WHERE UserId = @UserId
DELETE FROM Incomes WHERE UserId = @UserId
DELETE FROM Expenses WHERE UserId = @UserId
DELETE FROM Tasks WHERE UserId = @UserId
DELETE FROM DailyHabits WHERE UserId = @UserId
GO

DECLARE @UserId INT
SELECT @UserId = UserId FROM Users WHERE Email = 'admin@lifeos.com'

-- ============================================
-- 1. DAILY HABITS (Today's habits)
-- ============================================
PRINT 'Inserting daily habits...'

INSERT INTO DailyHabits (UserId, HabitName, TargetValue, HabitDate, IsCompleted, Streak)
VALUES 
    (@UserId, 'Drink Water', '3 Liters', CAST(GETDATE() AS DATE), 1, 15),
    (@UserId, 'Morning Exercise', '30 minutes', CAST(GETDATE() AS DATE), 1, 8),
    (@UserId, 'Read Books', '20 pages', CAST(GETDATE() AS DATE), 0, 5),
    (@UserId, 'Meditation', '10 minutes', CAST(GETDATE() AS DATE), 0, 12),
    (@UserId, 'Coding Practice', '1 hour', CAST(GETDATE() AS DATE), 1, 20)

-- Yesterday's habits (for streak calculation)
INSERT INTO DailyHabits (UserId, HabitName, TargetValue, HabitDate, IsCompleted, Streak)
VALUES 
    (@UserId, 'Drink Water', '3 Liters', DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), 1, 14),
    (@UserId, 'Morning Exercise', '30 minutes', DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), 1, 7),
    (@UserId, 'Read Books', '20 pages', DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), 1, 4),
    (@UserId, 'Meditation', '10 minutes', DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), 1, 11),
    (@UserId, 'Coding Practice', '1 hour', DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), 1, 19)

PRINT '  ? Added 10 habit records (5 today, 5 yesterday)'

-- ============================================
-- 2. TASKS
-- ============================================
PRINT 'Inserting tasks...'

INSERT INTO Tasks (UserId, Title, Description, DueDateTime, Priority, Status, CreatedAt)
VALUES 
    -- Pending tasks
    (@UserId, 'Complete Project Report', 'Finish the Q4 project report and send to manager', 
     DATEADD(DAY, 2, GETDATE()), 'High', 'Pending', GETDATE()),
    
    (@UserId, 'Team Meeting', 'Weekly sync with development team', 
     DATEADD(HOUR, 3, GETDATE()), 'Medium', 'Pending', GETDATE()),
    
    (@UserId, 'Code Review', 'Review pull requests from team members', 
     DATEADD(DAY, 1, GETDATE()), 'High', 'Pending', DATEADD(DAY, -1, GETDATE())),
    
    (@UserId, 'Buy Groceries', 'Weekly grocery shopping - check list on fridge', 
     DATEADD(DAY, 1, GETDATE()), 'Low', 'Pending', GETDATE()),
    
    (@UserId, 'Update Documentation', 'Update API documentation for new endpoints', 
     DATEADD(DAY, 3, GETDATE()), 'Medium', 'Pending', DATEADD(DAY, -2, GETDATE())),
    
    -- Completed tasks
    (@UserId, 'Morning Standup', 'Daily team standup meeting', 
     DATEADD(HOUR, -2, GETDATE()), 'Medium', 'Done', DATEADD(DAY, -1, GETDATE())),
    
    (@UserId, 'Fix Login Bug', 'Resolve authentication issue reported by QA', 
     DATEADD(DAY, -1, GETDATE()), 'High', 'Done', DATEADD(DAY, -3, GETDATE())),
    
    (@UserId, 'Database Backup', 'Weekly database backup and verification', 
     DATEADD(DAY, -2, GETDATE()), 'High', 'Done', DATEADD(DAY, -7, GETDATE()))

PRINT '  ? Added 8 tasks (5 pending, 3 completed)'

-- ============================================
-- 3. EXPENSES (This month)
-- ============================================
PRINT 'Inserting expenses...'

DECLARE @StartOfMonth DATE = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)

INSERT INTO Expenses (UserId, Reason, Category, Amount, ExpenseDate)
VALUES 
    -- This week
    (@UserId, 'Lunch at Restaurant', 'Food', 450.00, GETDATE()),
    (@UserId, 'Uber to Office', 'Transport', 180.00, GETDATE()),
    (@UserId, 'Coffee and Snacks', 'Food', 250.00, DATEADD(DAY, -1, GETDATE())),
    (@UserId, 'Movie Tickets', 'Entertainment', 600.00, DATEADD(DAY, -1, GETDATE())),
    (@UserId, 'Electricity Bill', 'Bills', 1850.00, DATEADD(DAY, -2, GETDATE())),
    (@UserId, 'Grocery Shopping', 'Food', 3200.00, DATEADD(DAY, -3, GETDATE())),
    (@UserId, 'Book Purchase', 'Shopping', 899.00, DATEADD(DAY, -4, GETDATE())),
    
    -- Last week
    (@UserId, 'Internet Bill', 'Bills', 999.00, DATEADD(DAY, -7, GETDATE())),
    (@UserId, 'Fuel', 'Transport', 2500.00, DATEADD(DAY, -8, GETDATE())),
    (@UserId, 'Restaurant Dinner', 'Food', 1200.00, DATEADD(DAY, -9, GETDATE())),
    (@UserId, 'Gym Membership', 'Shopping', 2000.00, DATEADD(DAY, -10, GETDATE())),
    (@UserId, 'Mobile Recharge', 'Bills', 399.00, DATEADD(DAY, -12, GETDATE())),
    
    -- Earlier this month
    (@UserId, 'Weekend Outing', 'Entertainment', 3500.00, DATEADD(DAY, -15, GETDATE())),
    (@UserId, 'Clothes Shopping', 'Shopping', 4500.00, DATEADD(DAY, -18, GETDATE())),
    (@UserId, 'Medical Checkup', 'Bills', 2500.00, DATEADD(DAY, -20, GETDATE()))

PRINT '  ? Added 15 expense records'

-- ============================================
-- 4. INCOME
-- ============================================
PRINT 'Inserting income...'

INSERT INTO Incomes (UserId, Month, Year, MonthlyIncome)
VALUES 
    (@UserId, MONTH(GETDATE()), YEAR(GETDATE()), 75000.00),
    (@UserId, MONTH(DATEADD(MONTH, -1, GETDATE())), YEAR(DATEADD(MONTH, -1, GETDATE())), 75000.00),
    (@UserId, MONTH(DATEADD(MONTH, -2, GETDATE())), YEAR(DATEADD(MONTH, -2, GETDATE())), 70000.00),
    (@UserId, MONTH(DATEADD(MONTH, -3, GETDATE())), YEAR(DATEADD(MONTH, -3, GETDATE())), 70000.00)

PRINT '  ? Added 4 income records'

-- ============================================
-- 5. FOCUS LOGS (Last 7 days)
-- ============================================
PRINT 'Inserting focus logs...'

INSERT INTO FocusLogs (UserId, Category, MinutesSpent, LogDate)
VALUES 
    -- Today
    (@UserId, 'Coding', 120, GETDATE()),
    (@UserId, 'Study', 45, GETDATE()),
    (@UserId, 'Reading', 30, DATEADD(HOUR, -3, GETDATE())),
    
    -- Yesterday
    (@UserId, 'Coding', 180, DATEADD(DAY, -1, GETDATE())),
    (@UserId, 'Work', 240, DATEADD(DAY, -1, GETDATE())),
    (@UserId, 'Exercise', 60, DATEADD(DAY, -1, GETDATE())),
    (@UserId, 'Meditation', 15, DATEADD(DAY, -1, GETDATE())),
    
    -- 2 days ago
    (@UserId, 'Coding', 150, DATEADD(DAY, -2, GETDATE())),
    (@UserId, 'Study', 90, DATEADD(DAY, -2, GETDATE())),
    (@UserId, 'Reading', 40, DATEADD(DAY, -2, GETDATE())),
    
    -- 3 days ago
    (@UserId, 'Work', 300, DATEADD(DAY, -3, GETDATE())),
    (@UserId, 'Coding', 120, DATEADD(DAY, -3, GETDATE())),
    
    -- 4 days ago
    (@UserId, 'Coding', 135, DATEADD(DAY, -4, GETDATE())),
    (@UserId, 'Writing', 60, DATEADD(DAY, -4, GETDATE())),
    (@UserId, 'Study', 75, DATEADD(DAY, -4, GETDATE())),
    
    -- 5 days ago
    (@UserId, 'Coding', 200, DATEADD(DAY, -5, GETDATE())),
    (@UserId, 'Meditation', 20, DATEADD(DAY, -5, GETDATE())),
    
    -- 6 days ago
    (@UserId, 'Work', 180, DATEADD(DAY, -6, GETDATE())),
    (@UserId, 'Exercise', 45, DATEADD(DAY, -6, GETDATE())),
    (@UserId, 'Reading', 35, DATEADD(DAY, -6, GETDATE()))

PRINT '  ? Added 20 focus log entries'

-- ============================================
-- 6. MOOD LOGS (Last 30 days)
-- ============================================
PRINT 'Inserting mood logs...'

DECLARE @Counter INT = 0
WHILE @Counter < 30
BEGIN
    INSERT INTO MoodLogs (UserId, MoodLevel, MoodDate)
    VALUES (
        @UserId, 
        -- Random mood level between 3-5 (mostly positive)
        CASE 
            WHEN @Counter % 7 = 0 THEN 5  -- Great on some days
            WHEN @Counter % 5 = 0 THEN 4  -- Good on some days
            WHEN @Counter % 3 = 0 THEN 3  -- Okay on some days
            ELSE 4                        -- Default good
        END,
        DATEADD(DAY, -@Counter, CAST(GETDATE() AS DATE))
    )
    SET @Counter = @Counter + 1
END

PRINT '  ? Added 30 mood log entries'

-- ============================================
-- Summary
-- ============================================
PRINT ''
PRINT '================================================'
PRINT '? SAMPLE DATA INSERTED SUCCESSFULLY!'
PRINT '================================================'
PRINT ''
PRINT 'Data Summary for user: admin@lifeos.com'
PRINT '----------------------------------------'
PRINT '• Daily Habits: 10 records'
PRINT '• Tasks: 8 records (5 pending, 3 done)'
PRINT '• Expenses: 15 records (?' + CAST((SELECT SUM(Amount) FROM Expenses WHERE UserId = @UserId) AS VARCHAR) + ')'
PRINT '• Income: 4 months'
PRINT '• Focus Logs: 20 entries'
PRINT '• Mood Logs: 30 days'
PRINT ''
PRINT 'You can now login and explore the application with realistic data!'
PRINT '================================================'
