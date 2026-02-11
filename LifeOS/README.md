# LifeOS - Life Management System

A comprehensive ASP.NET MVC application for managing your daily life - habits, tasks, expenses, income, focus time, and mood tracking.

## ?? Features

- **Daily Habits Tracker** - Track daily habits with streak counting
- **Task Management** - Manage tasks with priorities and due dates
- **Expense Tracking** - Monitor your monthly expenses by category
- **Income Management** - Record monthly income
- **Focus Time Logging** - Track productive time across different categories
- **Mood Tracker** - Monitor your daily mood with visual tracking

## ?? Prerequisites

- Visual Studio 2017 or later
- .NET Framework 4.6.1 or later
- SQL Server Express (LocalDB or SQL Server)
- IIS Express (comes with Visual Studio)

## ??? Setup Instructions

### 1. Database Setup

**Option A: Run the full schema script (Recommended for new setup)**

1. Open SQL Server Management Studio (SSMS)
2. Connect to your SQL Server instance (.\SQLEXPRESS)
3. Open the file: `Database\FullSchema_LifeOS.sql`
4. Execute the script to create the database and all tables

**Option B: Add Streak column to existing database**

If you already have the LifeOS database but missing the Streak column:

1. Open SSMS and connect to your SQL Server
2. Open the file: `Database\Migration_AddStreakColumn.sql`
3. Execute the script

### 2. Connection String

The connection string is already configured in `Web.config`:

```xml
<connectionStrings>
  <add name="LifeOSConnection" 
       connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=LifeOSDB;Integrated Security=True;MultipleActiveResultSets=True;" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

If your SQL Server instance is different, update the `Data Source` value.

### 3. Create a Test User

Run this SQL script in SSMS to create a test user:

```sql
USE LifeOSDB
GO

INSERT INTO Users (Name, Email, PasswordHash, CreatedAt)
VALUES ('Test User', 'test@example.com', 'test123', GETDATE())
GO
```

**Login Credentials:**
- Email: `test@example.com`
- Password: `test123`

### 4. Build and Run

1. Open `LifeOS.sln` in Visual Studio
2. Restore NuGet packages (Right-click solution ? Restore NuGet Packages)
3. Build the solution (Ctrl + Shift + B)
4. Run the application (F5)
5. The login page should open automatically

## ?? Project Structure

```
LifeOS/
??? Controllers/          # MVC Controllers
?   ??? AuthController.cs       # Authentication
?   ??? DashboardController.cs  # Dashboard
?   ??? HabitsController.cs     # Daily Habits
?   ??? TaskController.cs       # Tasks
?   ??? ExpensesController.cs   # Expenses
?   ??? IncomeController.cs     # Income
?   ??? FocusController.cs      # Focus Time
?   ??? MoodController.cs       # Mood Tracking
??? Models/MD/           # Data Models
?   ??? User.cs
?   ??? DailyHabit.cs
?   ??? TaskItem.cs
?   ??? Expense.cs
?   ??? Income.cs
?   ??? FocusLog.cs
?   ??? MoodLog.cs
?   ??? LifeOSContext.cs  # Entity Framework DbContext
??? Views/               # Razor Views
?   ??? Shared/
?   ?   ??? _Layout.cshtml     # Main layout with sidebar
?   ??? Auth/Login.cshtml
?   ??? Dashboard/Index.cshtml
?   ??? Habits/Index.cshtml
?   ??? Task/Index.cshtml
?   ??? Expenses/Index.cshtml
?   ??? Income/Index.cshtml
?   ??? Focus/Index.cshtml
?   ??? Mood/Index.cshtml
??? Content/
?   ??? Site.css         # Custom professional UI styles
??? Database/            # SQL Scripts
?   ??? FullSchema_LifeOS.sql
?   ??? Migration_AddStreakColumn.sql
??? Web.config           # Application configuration
```

## ?? UI Features

- Modern gradient design with purple theme
- Responsive sidebar navigation
- Professional card-based layouts
- Interactive mood tracker with emojis
- Color-coded badges and status indicators
- Clean data tables with hover effects
- Font Awesome icons throughout

## ?? Troubleshooting

### Database Connection Issues

If you get a connection error:
1. Ensure SQL Server is running
2. Verify the connection string in `Web.config`
3. Check if the database exists: `SELECT name FROM sys.databases WHERE name = 'LifeOSDB'`

### "Invalid column name 'Streak'" Error

This was the original issue - run the migration script:
```bash
Database\Migration_AddStreakColumn.sql
```

### NuGet Package Errors

If you encounter missing package errors:
1. Right-click on the solution
2. Select "Restore NuGet Packages"
3. Rebuild the solution

## ?? Usage Guide

### Daily Workflow

1. **Login** - Use your credentials to access the system
2. **Dashboard** - View your daily overview and statistics
3. **Add Habits** - Set up daily habits you want to track
4. **Mark Habits Complete** - Check off habits as you complete them (builds streak!)
5. **Add Tasks** - Create tasks with priorities and due dates
6. **Log Expenses** - Record expenses as they occur
7. **Track Focus Time** - Log productive time in different categories
8. **Log Mood** - Record your daily mood (once per day)

### Tips

- Complete habits daily to build streaks ??
- Set realistic task priorities
- Review your expenses weekly
- Track at least 30 minutes of focus time daily
- Log your mood consistently for better insights

## ?? Security Note

?? **Important**: This is a development/learning project. For production use:

1. **Implement proper password hashing** (use BCrypt or similar)
2. Add input validation and sanitization
3. Implement HTTPS
4. Add authentication tokens
5. Enable CSRF protection
6. Add proper error handling and logging
7. Implement role-based authorization

## ?? Technologies Used

- **Backend**: ASP.NET MVC 5 (.NET Framework 4.6.1)
- **Database**: SQL Server with Entity Framework 6
- **Frontend**: Bootstrap 5, Font Awesome 6, Custom CSS
- **JavaScript**: jQuery 3.7.0

## ?? Contributing

Feel free to fork this project and submit pull requests for improvements!

## ?? License

This project is for educational purposes.

## ????? Author

Created as a comprehensive life management solution.

---

**Happy Life Management! ??**
