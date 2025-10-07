# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

GameSpace (branded as "GamiPort") is an ASP.NET Core 8.0 MVC web application built with Areas architecture. It's a gaming community platform combining forums, e-commerce, mini-games, social features, and user management.

**Technology Stack**: .NET 8.0, Entity Framework Core 8.0, SQL Server, ASP.NET Core Identity, SignalR, Bootstrap 5

## Build & Run Commands

```bash
# Navigate to solution directory (from repository root)
cd GameSpace

# Restore packages
dotnet restore GameSpace.sln

# Build project
dotnet build GameSpace.sln

# Build without restore (faster if packages already restored)
dotnet build GameSpace.sln --no-restore

# Clean build artifacts
dotnet clean GameSpace.sln

# Navigate to project directory and run
cd GameSpace
dotnet run

# Run with hot reload (development)
dotnet watch run

# Run with specific profile
dotnet run --launch-profile https

# Restore client-side libraries (Bootstrap, Font Awesome, etc.)
# Run from project directory: GameSpace/GameSpace/
libman restore

# Format code (enforce C# conventions)
dotnet format GameSpace.sln

# Apply Identity migrations
dotnet ef database update --context ApplicationDbContext
```

**Default URLs**:
- HTTPS: https://localhost:7042
- HTTP: http://localhost:5211

**Connection Strings** (in appsettings.json):
- `DefaultConnection` - ASP.NET Core Identity database
- `GameSpace` - Main application database (aliases: `GameSpacedatabaseContext`)

**Note**: Main database is database-first. Schema scripts are in `GameSpace/schema/`.

## Architecture Overview

### Areas-Based Organization

The application uses **ASP.NET Core Areas** to organize features into modules:

1. **MiniGame** - Largest area with wallet system, sign-in rewards, pet management, and mini-games
2. **social_hub** - SignalR-based real-time chat, notifications, content filtering
3. **OnlineStore** - E-commerce with products, orders, suppliers
4. **Forum** - Gaming forums, threads, posts, reactions
5. **MemberManagement** - User and manager administration
6. **Identity** - ASP.NET Core Identity scaffolded pages

### Dual Authentication System

**Important**: This application has TWO separate authentication systems:

1. **ASP.NET Core Identity** - For regular users
   - Uses `ApplicationDbContext`
   - Standard cookie authentication
   - Email confirmation required

2. **Custom Admin Authentication** - For managers/staff
   - Scheme: `"AdminCookie"`
   - Cookie name: `GameSpace.Admin`
   - Login path: `/Login`
   - 4-hour sliding expiration
   - Uses `GameSpacedatabaseContext`

**Authorization Policies**:
- `AdminOnly` - Requires AdminCookie
- `CanManageShopping`, `CanAdmin`, `CanMessage`, `CanUserStatus`, `CanPet`, `CanCS` - Fine-grained permissions via claims

### Dual Database Context

**Critical**: Two separate DbContexts exist:

1. **ApplicationDbContext** (`Data/ApplicationDbContext.cs`)
   - Handles ASP.NET Core Identity (users, roles, claims)
   - Connection string: `DefaultConnection`
   - Code-first with migrations

2. **GameSpacedatabaseContext** (`Models/GameSpacedatabaseContext.cs`)
   - Main application database (84+ entity models)
   - Connection string: `GameSpacedatabaseContext`
   - Database-first approach (scaffolded from existing DB)
   - 100,000+ line auto-generated file
   - Use `GameSpacedatabaseContext.Partial.cs` for custom extensions

**Always verify which context a service/controller should use.**

### Directory Structure

**Important**: The repository has a nested structure. From the repository root:

```
C:\Users\n2029\Desktop\GameSpace\          (Repository root - contains docs)
├── CLAUDE.md                               - This file
├── AGENTS.md                               - General AI agent guidelines
├── BUILD_ERROR_SUMMARY.md                  - Build error documentation
├── GameSpace\                              (Solution directory)
│   ├── GameSpace.sln                       - Main solution file
│   └── GameSpace\                          (Project directory)
│       ├── GameSpace.csproj                - Project file
│       ├── Program.cs                      - DI configuration, middleware
│       ├── Areas/
│       │   ├── MiniGame/                   - Controllers (24), Services (30+)
│       │   ├── social_hub/                 - ChatHub (SignalR), MuteFilter
│       │   ├── OnlineStore/                - ProductInfoes, OrderInfoes
│       │   ├── Forum/                      - Posts, Threads, Metrics
│       │   ├── MemberManagement/           - User/Manager admin
│       │   └── Identity/                   - ASP.NET Identity pages
│       ├── Controllers/                    - Root controllers (Home, Login)
│       ├── Data/                           - ApplicationDbContext & migrations
│       ├── Infrastructure/
│       │   ├── Login/                      - ILoginIdentity abstraction
│       │   └── Time/                       - IAppClock (Taipei timezone)
│       ├── Models/                         - 84+ entity models
│       ├── Partials/                       - DbContext partial extensions
│       ├── Views/                          - 270+ Razor views
│       └── wwwroot/                        - Static assets
```

**Build Commands Context**: When documentation refers to `GameSpace/GameSpace`, it means:
- From repo root: `cd GameSpace/GameSpace` (the project directory)
- Absolute path: `C:\Users\n2029\Desktop\GameSpace\GameSpace\GameSpace\`

## Key Patterns & Conventions

### Base Controller Pattern

Areas use base controllers for shared functionality:

**MiniGame Example** (`Areas/MiniGame/Controllers/MiniGameBaseController.cs`):
```csharp
[Area("MiniGame")]
[Authorize(AuthenticationSchemes = "AdminCookie", Policy = "AdminOnly")]
public class MiniGameBaseController : Controller
{
    protected readonly GameSpacedatabaseContext _context;
    protected readonly IMiniGameAdminService _adminService;
    protected readonly IManagerPermissionService _permissionService;

    protected async Task<int> GetCurrentManagerId();
    protected async Task<ManagerDatum?> GetCurrentManagerAsync();
    protected async Task<bool> HasPermissionAsync(string gate);
}
```

**When creating controllers**: Inherit from area base controller and call `base(context, adminService, permissionService)` constructor.

**Router Entry Point Controllers**: MiniGame area includes `GameAdminController` and `PetAdminController` that serve as unified routing entry points for tab navigation (`_MiniGameAdminTabs.cshtml`). These controllers redirect to the actual implementation controllers (`AdminMiniGame`, `AdminPet`) while maintaining clean URL routes.

### Service Layer Pattern

Services follow interface-based dependency injection:

```csharp
// Interface
public interface IUserWalletService
{
    Task<UserWallet?> GetWalletAsync(int userId);
    Task<bool> AddPointsAsync(int userId, int points, string reason);
}

// Implementation
public class UserWalletService : IUserWalletService
{
    private readonly GameSpacedatabaseContext _context;

    public UserWalletService(GameSpacedatabaseContext context)
    {
        _context = context;
    }
}
```

**Registration**: Services registered in `Program.cs` or area-specific `ServiceExtensions.cs` (MiniGame area).

### Permission System

**MiniGame Area** uses fine-grained permissions:
- Stored in `ManagerRolePermission` table
- Loaded as claims during admin login
- Checked via `IManagerPermissionService.HasPermissionAsync(managerId, gate)`
- Available "gates": `Shopping`, `Admin`, `Message`, `UserStat`, `Pet`, `CS`

**Usage in controllers**:
```csharp
if (!await HasPermissionAsync("Pet"))
{
    return Forbid();
}
```

### Idempotency Filter

**MiniGame Area** has `IdempotencyFilter` for POST/PUT/PATCH/DELETE:
- Checks `X-Idempotency-Key` header
- 60-second deduplication window
- Memory cache-based
- Applied globally to MiniGame controllers

### SignalR Chat (social_hub)

**ChatHub** (`/social_hub/chatHub`):
- Real-time direct messaging
- Methods: `SendDirect`, `MarkAsRead`, `GetUnreadCount`
- Client events: `ReceiveDirect`, `ReadReceipt`, `UnreadUpdate`, `Error`
- **MuteFilter**: Automatically masks profanity in messages

**Authentication**: Supports both regular users and admin cookies via `UserContextReader`.

### Time Zone Handling

**Critical**: Application uses **Taipei timezone** (UTC+8):
- `IAppClock` service provides timezone-aware DateTime
- Use `_appClock.Now` instead of `DateTime.Now`
- View helpers: `@Html.TaipeiTime(datetime)`, `@Html.TaipeiTimeShort(datetime)`

## Database Schema

### Key Entity Models (in `/Models/`)

**Users**: `User`, `UserWallet`, `UserToken`, `UserIntroduce`, `UserRight`
**Managers**: `ManagerDatum`, `ManagerRole`, `ManagerRolePermission`
**MiniGame**: `MiniGame`, `Pet`, `DailyGameLimit`, `SignInRecord`, `SignInRule`
**Wallet**: `Coupon`, `CouponType`, `Evoucher`, `EvoucherType`, `WalletHistory`
**Social**: `DmConversation`, `DmMessage`, `Group`, `GroupChat`, `Notification`
**Forum**: `Forum`, `Thread`, `ThreadPost`, `Post`, `Reaction`
**Store**: `ProductInfo`, `OrderInfo`, `OrderItem`, `PaymentTransaction`
**Support**: `SupportTicket`, `SupportTicketMessage`, `CsAgent`

### Migration Management

**Important**: While the main application database uses a database-first approach, the MiniGame area has its own migrations for specific features:

- **MiniGame Migrations**: Located in `Areas/MiniGame/Migrations/`
  - Manages MiniGame-specific tables like `SignInRule`
  - Uses `GameSpacedatabaseContext` (the main database context)
  - Apply migrations: `dotnet ef database update --context GameSpacedatabaseContext`
  - Create new migration: `dotnet ef migrations add MigrationName --context GameSpacedatabaseContext --output-dir Areas/MiniGame/Migrations`

- **Identity Migrations**: Located in `Data/Migrations/`
  - Manages ASP.NET Core Identity tables
  - Uses `ApplicationDbContext`
  - Apply migrations: `dotnet ef database update --context ApplicationDbContext`

### DbSet Naming Convention

**Inconsistent**: Some tables use singular (`User`), others plural (`ProductInfoes`, `OrderInfoes`).
**Important**: Check `GameSpacedatabaseContext.cs` for exact DbSet names before querying.

### Seed Data

Manager test accounts (ID 30000001-30000010) with various permissions in `/schema/` SQL scripts.

**Primary test account**:
- Username: `zhang_zhiming_01`
- Password: `AdminPass001@`
- Role: Full permissions

## Common Coding Patterns

### Controller Method Pattern

```csharp
[HttpGet]
public async Task<IActionResult> Index()
{
    if (!await HasPermissionAsync("RequiredGate"))
        return Forbid();

    var data = await _service.GetDataAsync();
    return View(data);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(CreateViewModel model)
{
    if (!ModelState.IsValid)
        return View(model);

    await _service.CreateAsync(model);
    return RedirectToAction(nameof(Index));
}
```

### Service Method Pattern

```csharp
public async Task<Result> DoSomethingAsync(int id)
{
    var entity = await _context.Entities
        .FirstOrDefaultAsync(e => e.Id == id);

    if (entity == null)
        return Result.NotFound();

    // Business logic
    entity.UpdatedAt = _appClock.Now;
    await _context.SaveChangesAsync();

    return Result.Success();
}
```

### ViewModel Separation

**Always** use ViewModels for:
- Form input (`CreateViewModel`, `EditViewModel`)
- Display data (`IndexRowViewModel`, `DetailViewModel`)
- API responses (`ApiResultViewModel`)

**Never** bind entities directly to views or accept entities as form input.

## Known Issues & Technical Debt

### Current Build Status

**As of October 8, 2025**: The project builds successfully with **0 errors**. All major build issues have been resolved.

**Recently Fixed**:
- ✅ All Razor View syntax errors resolved
- ✅ All ViewModel property mismatches corrected
- ✅ All type conversion errors fixed
- ✅ All navigation property issues resolved
- ✅ Major property name corrections (User_Point → UserPoint, etc.)
- ✅ DbSet naming issues (WalletHistory → WalletHistories)
- ✅ Service interface method implementations
- ✅ Critical controller dependency issues

**Current Focus**: Feature development and enhancement

### Common Anti-Patterns to Avoid

- **Don't**: Use `ApplicationDbContext` for business entities (use `GameSpacedatabaseContext`)
- **Don't**: Forget to call base class constructors in controllers
- **Don't**: Use `DateTime.Now` (use `IAppClock.Now`)
- **Don't**: Skip permission checks in admin controllers
- **Don't**: Bind entities directly to forms
- **Don't**: Assume property names without checking the actual entity definition in `Models/`
- **Don't**: Use navigation properties without verifying they exist in `GameSpacedatabaseContext.cs`

## MiniGame Area Deep Dive

The most complex area with 4 major subsystems:

### 1. Wallet System
- User points balance tracking
- Exchange points for coupons/vouchers
- Transaction history
- Admin: Issue points, manage coupon/voucher types

### 2. Sign-In System
- Daily check-in calendar
- Rewards: points, pet XP, coupons
- Admin: Configure reward rules

### 3. Pet System
- Pet naming, interactions (feed, bath, play, sleep)
- Skin color changes (costs points)
- Background customization
- Admin: Pet rules, level-up rules, individual pet management

### 4. Mini-Game System
- Adventure game (3 plays/day default)
- Win/lose/abort outcomes with rewards
- Admin: Game rules configuration

**Service Registration**: All MiniGame services registered via `Areas/MiniGame/ServiceExtensions.cs` (30+ services).

## Important Configuration Files

- **appsettings.json / appsettings.Development.json**: Connection strings, CORS, session config
- **libman.json**: Client-side libraries (Font Awesome, Bootstrap)
- **Program.cs**: DI container setup, middleware pipeline, authentication schemes
- **schema/**: Database documentation, seed data scripts

## Documentation Locations

- `/Areas/MiniGame/Views/AdminPet/README_NewFeatures.md` - Pet feature docs
- Root directory error reports:
  - `BUILD_ERROR_SUMMARY.md` - Current build error summary (start here)
  - `INDEX.md` - Guide to all build error documentation
  - `build_analysis_report.md` - Detailed error analysis
  - `quick_fix_guide.md` - Step-by-step fix instructions
  - `AGENTS.md` - General repository guidelines for AI agents

## Development Workflow

1. **Starting Development**: Ensure SQL Server is running and databases exist
2. **Before Making Changes**:
   - Run `dotnet build GameSpace.sln --no-restore` to check current build status
   - Review error reports in root directory if build fails
3. **Adding Features**:
   - Create service interface, implementation, register in DI
   - Create controller/views
   - **Always verify entity property names** by checking `/Models/` before writing service code
4. **Database Changes**:
   - Main DB uses database-first approach (scaffolded from existing DB)
   - For Identity changes: Create EF migration for `ApplicationDbContext`
5. **Testing**: Use provided test manager accounts, verify permissions work correctly
6. **Verifying Property Names**:
   - Check entity definitions in `GameSpacedatabaseContext.cs` (100,000+ line file)
   - Use custom extensions in `GameSpacedatabaseContext.Partial.cs`
   - DbSet names may differ from entity names (e.g., `WalletHistories` not `WalletHistory`)
7. **Committing**: Follow existing patterns, keep ViewModels separate from entities

## Security Considerations

- **Anti-Forgery**: Global filter validates tokens on all POST/PUT/PATCH/DELETE
- **Authorization**: Always check permissions before admin actions
- **Authentication**: Respect dual auth system, don't mix Identity and Admin cookies
- **Idempotency**: Mutation endpoints in MiniGame area use idempotency keys
- **Content Filtering**: social_hub uses MuteFilter for user-generated content

## Troubleshooting Build Errors

### When Entity Property Errors Occur

If you encounter errors like `'EntityName' does not contain a definition for 'PropertyName'`:

1. **Locate the entity definition**: Open `GameSpace/GameSpace/Models/EntityName.cs`
2. **Find the actual property name**: Property names are case-sensitive and may differ from expectations
3. **Common naming patterns**:
   - Some properties use underscores: `User_email`, `User_Point`
   - Some use PascalCase: `UserId`, `UserPoint`
   - ID properties may be `Id` or `ID` or variations like `CouponId`
4. **Check DbSet names**: In `GameSpacedatabaseContext.cs`, the DbSet name may be different:
   - `WalletHistories` (not `WalletHistory`)
   - `Evouchers` (not `EVouchers`)
   - `ManagerData` (not `Managers`)

### When Service Interface Errors Occur

If you see `'IServiceName' does not contain a definition for 'MethodName'`:

1. Add the method signature to the interface in `Areas/{Area}/Services/IServiceName.cs`
2. Implement the method in the concrete service class
3. Verify the method is registered in DI (check `Program.cs` or `ServiceExtensions.cs`)

### When ViewModel Property Errors Occur

If Razor views show errors about missing ViewModel properties:

1. Open the ViewModel class (usually in `Areas/{Area}/ViewModels/`)
2. Add the missing property with appropriate type and attributes
3. Ensure the property is populated in the controller action

## Quick Reference

**File Counts**: 58 controllers, 66 services, 84+ models, 270+ views
**Main Solution**: `GameSpace/GameSpace.sln`
**Main Project**: `GameSpace/GameSpace/GameSpace.csproj`
**Framework**: .NET 8.0
**Database**: SQL Server (LocalDB/Express)
**Server**: `DESKTOP-8HQIS1S\SQLEXPRESS` (development)
**Current Build Status**: 0 errors (October 8, 2025) - Successfully building

**Key Files for Context**:
- `BUILD_ERROR_SUMMARY.md` - Historical build error documentation (October 3, 2025)
- `AGENTS.md` - General repository guidelines for AI agents
- `INDEX.md` - Guide to all build error documentation
- `schema/` - Database schema scripts and seed data

**Code Style** (from AGENTS.md):
- C# with .NET 8, 4 spaces indent (no tabs)
- Allman brace style (braces on new lines)
- PascalCase for types/methods, camelCase for locals, `_camelCase` for private fields
- One class per file
- Run `dotnet format GameSpace.sln` before committing
