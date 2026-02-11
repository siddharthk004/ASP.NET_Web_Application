# LifeOS - Developer Quick Reference

## ?? 5-Minute Setup

```bash
1. Run: Database\FullSchema_LifeOS.sql
2. Run: Database\InsertTestUsers.sql
3. Press F5 in Visual Studio
4. Login: admin@lifeos.com / admin123
```

---

## ?? Project File Locations

### Controllers
```
Controllers\AuthController.cs         - Login/Logout
Controllers\DashboardController.cs    - Dashboard stats
Controllers\HabitsController.cs       - Habit tracking
Controllers\TaskController.cs         - Task management
Controllers\ExpensesController.cs     - Expense tracking
Controllers\IncomeController.cs       - Income tracking
Controllers\FocusController.cs        - Focus time logs
Controllers\MoodController.cs         - Mood tracking
```

### Models
```
Models\MD\User.cs          - User entity
Models\MD\DailyHabit.cs    - Habit entity (with Streak!)
Models\MD\TaskItem.cs      - Task entity
Models\MD\Expense.cs       - Expense entity
Models\MD\Income.cs        - Income entity
Models\MD\FocusLog.cs      - Focus log entity
Models\MD\MoodLog.cs       - Mood log entity
Models\MD\LifeOSContext.cs - EF DbContext
```

### Views
```
Views\Shared\_Layout.cshtml  - Master layout with sidebar
Views\Auth\Login.cshtml      - Login page (no layout)
Views\Dashboard\Index.cshtml - Dashboard
Views\Habits\Index.cshtml    - Habits list
Views\Task\Index.cshtml      - Tasks list
Views\Expenses\Index.cshtml  - Expenses list
Views\Income\Index.cshtml    - Income list
Views\Focus\Index.cshtml     - Focus logs
Views\Mood\Index.cshtml      - Mood tracker
```

### Styles
```
Content\Site.css           - Custom professional styles
Content\bootstrap.min.css  - Bootstrap 5
```

---

## ?? Common Development Tasks

### Add a New Feature Module

**1. Create Model**
```csharp
// Models\MD\YourModel.cs
public class YourModel
{
    [Key]
    public int YourModelId { get; set; }
    public int UserId { get; set; }
    public string Property { get; set; }
    // Add properties
}
```

**2. Add to DbContext**
```csharp
// Models\MD\LifeOSContext.cs
public DbSet<YourModel> YourModels { get; set; }
```

**3. Create Controller**
```csharp
// Controllers\YourController.cs
public class YourController : Controller
{
    LifeOSContext db = new LifeOSContext();
    
    public ActionResult Index()
    {
        if (Session["UserId"] == null)
            return RedirectToAction("Login", "Auth");
            
        int userId = (int)Session["UserId"];
        var data = db.YourModels.Where(x => x.UserId == userId).ToList();
        return View(data);
    }
    
    [HttpPost]
    public ActionResult Add(/* parameters */)
    {
        // Add logic
        db.SaveChanges();
        return RedirectToAction("Index");
    }
}
```

**4. Create View**
```razor
@model List<YourModel>
@{
    ViewBag.Title = "Your Feature";
}

<div class="content-card">
    <!-- Your UI here -->
</div>
```

**5. Add to Navigation**
```html
<!-- Views\Shared\_Layout.cshtml -->
<a href="@Url.Action("Index", "Your")" class="nav-item">
    <i class="fas fa-icon"></i>
    <span>Your Feature</span>
</a>
```

**6. Update Database**
```sql
CREATE TABLE YourModels (
    YourModelId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    Property NVARCHAR(100),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
)
```

---

## ?? UI Components Reference

### Stat Card
```html
<div class="stat-card">
    <div class="icon"><i class="fas fa-icon"></i></div>
    <h3>123</h3>
    <p>Description</p>
</div>
```

### Content Card
```html
<div class="content-card fade-in">
    <div class="card-header">
        <h5><i class="fas fa-icon"></i> Title</h5>
    </div>
    <!-- Content -->
</div>
```

### Form
```html
<form method="post" action="/Controller/Action" class="add-form">
    <div class="form-row">
        <input name="field" class="form-control" placeholder="..." required />
        <button type="submit" class="btn btn-primary">
            <i class="fas fa-plus"></i> Submit
        </button>
    </div>
</form>
```

### Data Table
```html
<table class="data-table">
    <thead>
        <tr>
            <th>Column 1</th>
            <th>Column 2</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in Model)
        {
            <tr>
                <td>@item.Property</td>
                <td>@item.Property2</td>
            </tr>
        }
    </tbody>
</table>
```

### Badges
```html
<span class="badge badge-success">Success</span>
<span class="badge badge-warning">Warning</span>
<span class="badge badge-danger">Danger</span>
<span class="badge badge-info">Info</span>
```

### Buttons
```html
<button class="btn btn-primary">Primary</button>
<button class="btn btn-success">Success</button>
<button class="btn btn-danger">Danger</button>
<button class="btn btn-sm">Small</button>
```

### Empty State
```html
<div class="empty-state">
    <i class="fas fa-icon"></i>
    <p>No data yet. Add something!</p>
</div>
```

---

## ?? CSS Variables

```css
:root {
    --primary: #667eea;        /* Purple */
    --primary-dark: #5568d3;
    --secondary: #764ba2;      /* Deep Purple */
    --success: #10b981;        /* Green */
    --danger: #ef4444;         /* Red */
    --warning: #f59e0b;        /* Orange */
    --info: #3b82f6;           /* Blue */
    --dark: #1e293b;           /* Dark Gray */
    --light: #f1f5f9;          /* Light Gray */
    --sidebar-width: 260px;
    --header-height: 70px;
}
```

---

## ?? Session Variables

```csharp
// Set in AuthController after login
Session["UserId"]   = user.UserId;    // int
Session["UserName"] = user.Name;       // string

// Use in controllers
int userId = (int)Session["UserId"];
string name = Session["UserName"]?.ToString();

// Check authentication
if (Session["UserId"] == null)
    return RedirectToAction("Login", "Auth");
```

---

## ?? Database Connection

```csharp
// In any controller
LifeOSContext db = new LifeOSContext();

// Query
var data = db.YourTable.Where(x => x.UserId == userId).ToList();

// Insert
db.YourTable.Add(newItem);
db.SaveChanges();

// Update
var item = db.YourTable.Find(id);
item.Property = newValue;
db.SaveChanges();

// Delete
var item = db.YourTable.Find(id);
db.YourTable.Remove(item);
db.SaveChanges();
```

---

## ?? Common Issues & Fixes

### Issue: "Invalid column name 'Streak'"
**Fix:** Run `Database\Migration_AddStreakColumn.sql`

### Issue: Cannot connect to database
**Fix:** 
1. Check SQL Server is running
2. Verify connection string in Web.config
3. Test connection in SSMS

### Issue: Login not working
**Fix:**
1. Verify user exists: `SELECT * FROM Users`
2. Check password matches exactly
3. Clear browser cookies/session

### Issue: Session lost on refresh
**Fix:**
```xml
<!-- Web.config -->
<system.web>
    <sessionState timeout="60" />
</system.web>
```

### Issue: CSS not loading
**Fix:**
1. Check file path: `~/Content/Site.css`
2. Rebuild solution
3. Clear browser cache (Ctrl + Shift + Delete)

### Issue: Views not found
**Fix:**
1. Check View folder structure: `Views\Controller\Action.cshtml`
2. Verify `_ViewStart.cshtml` exists
3. Check namespace in controller

---

## ?? Git Workflow

```bash
# Status
git status

# Stage changes
git add .

# Commit
git commit -m "Add feature: description"

# Push
git push origin main

# Pull latest
git pull origin main
```

---

## ?? NuGet Packages

### Current Dependencies
```
EntityFramework (6.4.4)
Microsoft.AspNet.Mvc (5.2.9)
Microsoft.AspNet.Razor (3.2.9)
Microsoft.AspNet.WebPages (3.2.9)
Bootstrap (5.x)
jQuery (3.7.0)
```

### Add Package
```powershell
# Package Manager Console
Install-Package PackageName

# Or use NuGet Package Manager GUI
Right-click project ? Manage NuGet Packages
```

---

## ?? Testing Queries

```sql
-- Check data exists
SELECT COUNT(*) FROM Users
SELECT COUNT(*) FROM DailyHabits
SELECT COUNT(*) FROM Tasks

-- View user's data
DECLARE @UserId INT = 1
SELECT * FROM DailyHabits WHERE UserId = @UserId
SELECT * FROM Tasks WHERE UserId = @UserId
SELECT * FROM Expenses WHERE UserId = @UserId

-- Reset test data
DELETE FROM DailyHabits WHERE UserId = 1
DELETE FROM Tasks WHERE UserId = 1
-- Then run InsertSampleData.sql
```

---

## ?? URLs & Routes

```
/                              ? Auth/Login
/Auth/Login                    ? Login page
/Dashboard/Index               ? Dashboard
/Habits/Index                  ? Daily Habits
/Habits/Add                    ? Add habit (POST)
/Habits/Complete/{id}          ? Mark complete
/Task/Index                    ? Tasks list
/Task/Add                      ? Add task (POST)
/Task/Complete/{id}            ? Complete task
/Task/Delete/{id}              ? Delete task
/Expenses/Index                ? Expenses list
/Expenses/Add                  ? Add expense (POST)
/Expenses/Delete/{id}          ? Delete expense
/Income/Index                  ? Income list
/Focus/Index                   ? Focus logs
/Mood/Index                    ? Mood tracker
```

---

## ?? Pro Tips

1. **Use `db.SaveChanges()` after any Insert/Update/Delete**
2. **Always check `Session["UserId"]` in controller actions**
3. **Use `fade-in` class for smooth page load animations**
4. **Return `RedirectToAction()` after POST to prevent duplicate submissions**
5. **Use `Find()` for primary key lookups (faster)**
6. **Use `FirstOrDefault()` when you expect 0 or 1 results**
7. **Use `ToList()` to execute queries immediately**
8. **Set `ViewBag` properties before returning View**

---

## ?? Useful Links

- [ASP.NET MVC Docs](https://docs.microsoft.com/en-us/aspnet/mvc/)
- [Entity Framework](https://docs.microsoft.com/en-us/ef/ef6/)
- [Bootstrap 5](https://getbootstrap.com/)
- [Font Awesome](https://fontawesome.com/icons)
- [C# Reference](https://docs.microsoft.com/en-us/dotnet/csharp/)

---

## ?? Keyboard Shortcuts (Visual Studio)

```
F5              - Start Debugging
Ctrl+F5         - Start Without Debugging
Shift+F5        - Stop Debugging
F9              - Toggle Breakpoint
F10             - Step Over
F11             - Step Into
Ctrl+Shift+B    - Build Solution
Ctrl+K, Ctrl+D  - Format Document
Ctrl+K, Ctrl+C  - Comment Selection
Ctrl+K, Ctrl+U  - Uncomment Selection
Ctrl+.          - Quick Actions
```

---

**Happy Coding! ??**
