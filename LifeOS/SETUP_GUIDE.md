# LifeOS - Complete Setup Guide

## ?? Quick Start (5 Minutes)

Follow these steps to get LifeOS up and running:

### Step 1: Database Setup (2 minutes)

1. Open **SQL Server Management Studio (SSMS)**
2. Connect to your SQL Server instance: `.\SQLEXPRESS`
3. Click **File ? Open ? File**
4. Navigate to `Database\FullSchema_LifeOS.sql`
5. Click **Execute** (or press F5)
6. You should see: "LifeOS Database Schema Created Successfully!"

### Step 2: Create Test User (1 minute)

1. In SSMS, click **File ? Open ? File**
2. Navigate to `Database\InsertTestUsers.sql`
3. Click **Execute** (or press F5)
4. Note the login credentials shown in the output

**Default Login:**
- Email: `admin@lifeos.com`
- Password: `admin123`

### Step 3: Run the Application (2 minutes)

1. Open `LifeOS.sln` in Visual Studio
2. Right-click the solution ? **Restore NuGet Packages**
3. Press **F5** to build and run
4. Login with the credentials from Step 2
5. Explore the features!

---

## ?? Detailed Setup Instructions

### Prerequisites Checklist

- [ ] Windows 10/11
- [ ] Visual Studio 2017 or later
- [ ] .NET Framework 4.6.1 SDK
- [ ] SQL Server 2016 Express or later
- [ ] SQL Server Management Studio (SSMS)

### SQL Server Installation

If you don't have SQL Server installed:

1. Download **SQL Server Express** from Microsoft
2. During installation, select:
   - Database Engine Services
   - SQL Server Management Tools (or install SSMS separately)
3. Note your instance name (usually `SQLEXPRESS`)
4. Enable SQL Server Browser service

### Connection String Configuration

The default connection string in `Web.config`:

```xml
<add name="LifeOSConnection" 
     connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=LifeOSDB;Integrated Security=True;MultipleActiveResultSets=True;" 
     providerName="System.Data.SqlClient" />
```

**If your SQL Server instance has a different name**, update `Data Source`:
- Local instance: `.\SQLEXPRESS`
- Named instance: `.\YourInstanceName`
- Full server: `localhost\SQLEXPRESS`

### NuGet Packages

The project requires these packages (automatically restored):
- EntityFramework 6.x
- Microsoft.AspNet.Mvc 5.x
- Bootstrap 5.x
- jQuery 3.7.x

---

## ?? Troubleshooting

### "Cannot connect to SQL Server"

**Solution 1**: Enable TCP/IP
1. Open **SQL Server Configuration Manager**
2. Expand **SQL Server Network Configuration**
3. Click **Protocols for SQLEXPRESS**
4. Right-click **TCP/IP** ? Enable
5. Restart SQL Server service

**Solution 2**: Check Windows Authentication
1. Open SSMS
2. Connect using Windows Authentication
3. Expand **Security ? Logins**
4. Verify your Windows user has access

### "Invalid column name 'Streak'"

This means the Streak column is missing from DailyHabits table.

**Fix:**
1. Open SSMS
2. Run `Database\Migration_AddStreakColumn.sql`
3. Restart your application

### "Could not load file or assembly EntityFramework"

**Fix:**
1. In Visual Studio, go to **Tools ? NuGet Package Manager ? Package Manager Console**
2. Run: `Update-Package -reinstall`
3. Rebuild solution

### Database exists but tables are empty

Run the test user script:
```sql
Database\InsertTestUsers.sql
```

---

## ?? Database Schema Overview

```
Users
??? UserId (PK, Identity)
??? Name
??? Email (Unique)
??? PasswordHash
??? CreatedAt

DailyHabits
??? HabitId (PK, Identity)
??? UserId (FK ? Users)
??? HabitName
??? TargetValue
??? HabitDate
??? IsCompleted
??? Streak

Tasks
??? TaskId (PK, Identity)
??? UserId (FK ? Users)
??? Title
??? Description
??? DueDateTime
??? Priority
??? Status
??? CreatedAt

Expenses
??? ExpenseId (PK, Identity)
??? UserId (FK ? Users)
??? Reason
??? Category
??? Amount
??? ExpenseDate

Incomes
??? IncomeId (PK, Identity)
??? UserId (FK ? Users)
??? Month
??? Year
??? MonthlyIncome

FocusLogs
??? FocusId (PK, Identity)
??? UserId (FK ? Users)
??? Category
??? MinutesSpent
??? LogDate

MoodLogs
??? MoodId (PK, Identity)
??? UserId (FK ? Users)
??? MoodLevel (1-5)
??? MoodDate
```

---

## ?? Features Overview

### 1. Dashboard
- Daily habits completion rate
- Pending tasks count
- Monthly expenses total
- Today's focus time
- Quick navigation to all features

### 2. Daily Habits
- Add daily habits with targets
- Track completion status
- Build streaks (consecutive days)
- Visual status indicators

### 3. Tasks
- Create tasks with descriptions
- Set priorities (Low/Medium/High)
- Due date tracking
- Status management (Pending/Done)
- Overdue indicators

### 4. Expenses
- Record expenses by category
- Monthly expense totals
- Category-wise tracking
- Date-based filtering

### 5. Income
- Monthly income tracking
- Historical income records
- Update existing records

### 6. Focus Time
- Log productive time
- Category-based tracking
- Daily and weekly totals
- Hour conversion

### 7. Mood Tracker
- Daily mood logging (1-5 scale)
- Visual emoji interface
- 30-day history
- Average mood calculation

---

## ?? Advanced Configuration

### Enable HTTPS (Recommended for Production)

1. In Visual Studio, right-click project ? Properties
2. Go to **Web** tab
3. Check **Enable SSL**
4. Note the SSL URL
5. Update the URL in your browser

### Add More Users

```sql
USE LifeOSDB
GO

INSERT INTO Users (Name, Email, PasswordHash, CreatedAt)
VALUES ('John Doe', 'john@example.com', 'password123', GETDATE())
GO
```

### Customize UI Colors

Edit `Content\Site.css` and modify CSS variables:

```css
:root {
    --primary: #667eea;        /* Main purple */
    --secondary: #764ba2;      /* Secondary purple */
    --success: #10b981;        /* Green */
    --danger: #ef4444;         /* Red */
    --warning: #f59e0b;        /* Orange */
    --info: #3b82f6;           /* Blue */
}
```

---

## ?? Mobile Responsive

The UI is fully responsive and works on:
- Desktop (1920x1080 and above)
- Tablets (768px - 1024px)
- Mobile phones (320px - 767px)

On mobile, the sidebar can be toggled.

---

## ?? Security Recommendations for Production

1. **Password Hashing**
   - Replace SHA256 with BCrypt or Argon2
   - Install: `Install-Package BCrypt.Net-Next`
   
2. **Add HTTPS**
   - Enable SSL in IIS
   - Add SSL certificate
   - Redirect HTTP to HTTPS

3. **Input Validation**
   - Add data annotations to models
   - Use AntiForgeryToken on all forms
   - Sanitize user inputs

4. **Authentication**
   - Implement ASP.NET Identity
   - Add Remember Me functionality
   - Session timeout handling

5. **Error Handling**
   - Add global error handler
   - Log errors to database/file
   - Show friendly error pages

---

## ?? Support & Help

### Common Questions

**Q: Can I use SQL Server (not Express)?**
A: Yes! Just update the connection string accordingly.

**Q: How do I deploy to IIS?**
A: 
1. Build in Release mode
2. Right-click project ? Publish
3. Choose IIS, FTP, or Folder
4. Follow the wizard

**Q: Can I add more features?**
A: Absolutely! The architecture is modular. Add new controllers, models, and views as needed.

**Q: Is there a mobile app?**
A: Currently web-only, but the responsive design works great on mobile browsers.

---

## ?? Learning Resources

- [ASP.NET MVC Documentation](https://docs.microsoft.com/en-us/aspnet/mvc/)
- [Entity Framework 6](https://docs.microsoft.com/en-us/ef/ef6/)
- [Bootstrap 5 Docs](https://getbootstrap.com/docs/5.0/)
- [Font Awesome Icons](https://fontawesome.com/icons)

---

## ? Post-Setup Checklist

- [ ] Database created successfully
- [ ] Test user can login
- [ ] All pages load without errors
- [ ] Can add habits, tasks, expenses
- [ ] Sidebar navigation works
- [ ] Data persists after refresh
- [ ] Logout works correctly

---

**?? Congratulations! Your LifeOS is ready to use!**

Start managing your life efficiently with LifeOS. Track habits, complete tasks, monitor expenses, and maintain your well-being all in one place!
