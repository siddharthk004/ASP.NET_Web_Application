-- Insert Test User for LifeOS
-- Note: In production, use proper password hashing (BCrypt, PBKDF2, etc.)

USE [LifeOSDB]
GO

-- Insert test user
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'admin@lifeos.com')
BEGIN
    INSERT INTO Users (Name, Email, PasswordHash, CreatedAt)
    VALUES ('Admin User', 'admin@lifeos.com', 'admin123', GETDATE())
    
    PRINT 'Test user created successfully!'
    PRINT 'Email: admin@lifeos.com'
    PRINT 'Password: admin123'
END
ELSE
BEGIN
    PRINT 'User admin@lifeos.com already exists'
END
GO

-- Insert another test user
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'test@lifeos.com')
BEGIN
    INSERT INTO Users (Name, Email, PasswordHash, CreatedAt)
    VALUES ('Test User', 'test@lifeos.com', 'test123', GETDATE())
    
    PRINT 'Test user created successfully!'
    PRINT 'Email: test@lifeos.com'
    PRINT 'Password: test123'
END
ELSE
BEGIN
    PRINT 'User test@lifeos.com already exists'
END
GO

PRINT '================================================'
PRINT 'Test users created. You can now login with:'
PRINT '1. Email: admin@lifeos.com | Password: admin123'
PRINT '2. Email: test@lifeos.com  | Password: test123'
PRINT '================================================'
