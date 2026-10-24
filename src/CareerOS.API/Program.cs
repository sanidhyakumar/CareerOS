using CareerOS.API.Middleware;
using CareerOS.Application;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting CareerOS API...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Serilog configuration
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/careeros-.log", rollingInterval: RollingInterval.Day));

    // Application + Infrastructure layers
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // API Services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "CareerOS API", Version = "v1", Description = "Local-First Career Management OS" });
    });

    // CORS for local development and UI container
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    });

    var app = builder.Build();

    // Auto-create database & tables if not yet created
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            var folder = Path.GetDirectoryName(db.Database.GetConnectionString()?.Replace("Data Source=", ""));
            if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            db.Database.EnsureCreated();
            db.Database.ExecuteSqlRaw(@"
                PRAGMA journal_mode = WAL;
                PRAGMA synchronous = NORMAL;
                PRAGMA cache_size = -64000;
                PRAGMA temp_store = MEMORY;
                PRAGMA mmap_size = 268435456;

                CREATE TABLE IF NOT EXISTS ""WikiArticles"" (
                    ""Id"" TEXT NOT NULL CONSTRAINT ""PK_WikiArticles"" PRIMARY KEY,
                    ""Title"" TEXT NOT NULL,
                    ""Slug"" TEXT NOT NULL,
                    ""Category"" INTEGER NOT NULL,
                    ""Content"" TEXT NOT NULL,
                    ""Summary"" TEXT NULL,
                    ""Tags"" TEXT NULL,
                    ""CompanyId"" TEXT NULL,
                    ""JobId"" TEXT NULL,
                    ""IsStarred"" INTEGER NOT NULL,
                    ""IsPinned"" INTEGER NOT NULL,
                    ""ViewCount"" INTEGER NOT NULL,
                    ""CreatedAt"" TEXT NOT NULL,
                    ""UpdatedAt"" TEXT NOT NULL,
                    CONSTRAINT ""FK_WikiArticles_Companies_CompanyId"" FOREIGN KEY (""CompanyId"") REFERENCES ""Companies"" (""Id"") ON DELETE SET NULL,
                    CONSTRAINT ""FK_WikiArticles_Jobs_JobId"" FOREIGN KEY (""JobId"") REFERENCES ""Jobs"" (""Id"") ON DELETE SET NULL
                );
                CREATE UNIQUE INDEX IF NOT EXISTS ""IX_WikiArticles_Slug"" ON ""WikiArticles"" (""Slug"");
                CREATE TABLE IF NOT EXISTS ""WikiArticleLinks"" (
                    ""Id"" TEXT NOT NULL CONSTRAINT ""PK_WikiArticleLinks"" PRIMARY KEY,
                    ""SourceArticleId"" TEXT NOT NULL,
                    ""TargetArticleId"" TEXT NULL,
                    ""TargetSlug"" TEXT NOT NULL,
                    ""LinkText"" TEXT NULL,
                    ""CreatedAt"" TEXT NOT NULL,
                    ""UpdatedAt"" TEXT NOT NULL,
                    CONSTRAINT ""FK_WikiArticleLinks_WikiArticles_SourceArticleId"" FOREIGN KEY (""SourceArticleId"") REFERENCES ""WikiArticles"" (""Id"") ON DELETE CASCADE,
                    CONSTRAINT ""FK_WikiArticleLinks_WikiArticles_TargetArticleId"" FOREIGN KEY (""TargetArticleId"") REFERENCES ""WikiArticles"" (""Id"") ON DELETE SET NULL
                );
                CREATE INDEX IF NOT EXISTS ""IX_WikiArticleLinks_SourceArticleId_TargetSlug"" ON ""WikiArticleLinks"" (""SourceArticleId"", ""TargetSlug"");

                CREATE TABLE IF NOT EXISTS ""SponsorshipCases"" (
                    ""Id"" TEXT NOT NULL CONSTRAINT ""PK_SponsorshipCases"" PRIMARY KEY,
                    ""CountryId"" TEXT NOT NULL,
                    ""ProgramName"" TEXT NOT NULL,
                    ""ProgramType"" INTEGER NOT NULL,
                    ""CompanyId"" TEXT NULL,
                    ""JobId"" TEXT NULL,
                    ""Eligibility"" INTEGER NOT NULL,
                    ""Availability"" INTEGER NOT NULL,
                    ""MinimumSalaryRequirement"" TEXT NULL,
                    ""Currency"" TEXT NULL,
                    ""NextFiscalCycleDate"" TEXT NULL,
                    ""FilingDeadline"" TEXT NULL,
                    ""EstimatedProcessingMonths"" INTEGER NULL,
                    ""SupportsDualIntent"" INTEGER NOT NULL,
                    ""LeadsToPermanentResidency"" INTEGER NOT NULL,
                    ""Notes"" TEXT NULL,
                    ""CreatedAt"" TEXT NOT NULL,
                    ""UpdatedAt"" TEXT NOT NULL,
                    CONSTRAINT ""FK_SponsorshipCases_Countries_CountryId"" FOREIGN KEY (""CountryId"") REFERENCES ""Countries"" (""Id"") ON DELETE RESTRICT,
                    CONSTRAINT ""FK_SponsorshipCases_Companies_CompanyId"" FOREIGN KEY (""CompanyId"") REFERENCES ""Companies"" (""Id"") ON DELETE SET NULL,
                    CONSTRAINT ""FK_SponsorshipCases_Jobs_JobId"" FOREIGN KEY (""JobId"") REFERENCES ""Jobs"" (""Id"") ON DELETE SET NULL
                );

                CREATE TABLE IF NOT EXISTS ""SponsorshipEvidences"" (
                    ""Id"" TEXT NOT NULL CONSTRAINT ""PK_SponsorshipEvidences"" PRIMARY KEY,
                    ""SponsorshipCaseId"" TEXT NOT NULL,
                    ""DocumentId"" TEXT NULL,
                    ""EvidenceType"" TEXT NOT NULL,
                    ""Description"" TEXT NOT NULL,
                    ""IsVerified"" INTEGER NOT NULL,
                    ""ExpiryDate"" TEXT NULL,
                    ""CreatedAt"" TEXT NOT NULL,
                    ""UpdatedAt"" TEXT NOT NULL,
                    CONSTRAINT ""FK_SponsorshipEvidences_SponsorshipCases_SponsorshipCaseId"" FOREIGN KEY (""SponsorshipCaseId"") REFERENCES ""SponsorshipCases"" (""Id"") ON DELETE CASCADE,
                    CONSTRAINT ""FK_SponsorshipEvidences_Documents_DocumentId"" FOREIGN KEY (""DocumentId"") REFERENCES ""Documents"" (""Id"") ON DELETE SET NULL
                );

                CREATE TABLE IF NOT EXISTS ""KanbanCards"" (
                    ""Id"" TEXT NOT NULL CONSTRAINT ""PK_KanbanCards"" PRIMARY KEY,
                    ""Column"" INTEGER NOT NULL,
                    ""Position"" INTEGER NOT NULL,
                    ""Title"" TEXT NOT NULL,
                    ""CompanyName"" TEXT NOT NULL,
                    ""CompanyId"" TEXT NULL,
                    ""JobId"" TEXT NULL,
                    ""ApplicationId"" TEXT NULL,
                    ""Priority"" INTEGER NOT NULL,
                    ""Salary"" TEXT NULL,
                    ""Location"" TEXT NULL,
                    ""Notes"" TEXT NULL,
                    ""Tags"" TEXT NULL,
                    ""DueDate"" TEXT NULL,
                    ""CreatedAt"" TEXT NOT NULL,
                    ""UpdatedAt"" TEXT NOT NULL,
                    CONSTRAINT ""FK_KanbanCards_Companies_CompanyId"" FOREIGN KEY (""CompanyId"") REFERENCES ""Companies"" (""Id"") ON DELETE SET NULL,
                    CONSTRAINT ""FK_KanbanCards_Jobs_JobId"" FOREIGN KEY (""JobId"") REFERENCES ""Jobs"" (""Id"") ON DELETE SET NULL,
                    CONSTRAINT ""FK_KanbanCards_JobApplications_ApplicationId"" FOREIGN KEY (""ApplicationId"") REFERENCES ""JobApplications"" (""Id"") ON DELETE SET NULL
                );
                CREATE INDEX IF NOT EXISTS ""IX_KanbanCards_Column_Position"" ON ""KanbanCards"" (""Column"", ""Position"");

                CREATE TABLE IF NOT EXISTS ""ReminderItems"" (
                    ""Id"" TEXT NOT NULL CONSTRAINT ""PK_ReminderItems"" PRIMARY KEY,
                    ""Category"" INTEGER NOT NULL,
                    ""Severity"" INTEGER NOT NULL,
                    ""Title"" TEXT NOT NULL,
                    ""Message"" TEXT NOT NULL,
                    ""EntityType"" TEXT NULL,
                    ""EntityId"" TEXT NULL,
                    ""ActionUrl"" TEXT NULL,
                    ""DueDate"" TEXT NULL,
                    ""IsDismissed"" INTEGER NOT NULL,
                    ""DismissedAt"" TEXT NULL,
                    ""IsSnoozed"" INTEGER NOT NULL,
                    ""SnoozedUntil"" TEXT NULL,
                    ""IsAutoGenerated"" INTEGER NOT NULL,
                    ""DeduplicationKey"" TEXT NULL,
                    ""CreatedAt"" TEXT NOT NULL,
                    ""UpdatedAt"" TEXT NOT NULL
                );
                CREATE INDEX IF NOT EXISTS ""IX_ReminderItems_DeduplicationKey"" ON ""ReminderItems"" (""DeduplicationKey"");
                CREATE INDEX IF NOT EXISTS ""IX_ReminderItems_IsDismissed_IsSnoozed"" ON ""ReminderItems"" (""IsDismissed"", ""IsSnoozed"");

                CREATE VIRTUAL TABLE IF NOT EXISTS GlobalSearchEntries USING fts5(
                    EntityId UNINDEXED,
                    EntityType UNINDEXED,
                    Title,
                    Subtitle,
                    Content,
                    Tags,
                    ActionUrl UNINDEXED,
                    tokenize = 'porter unicode61'
                );

                CREATE TABLE IF NOT EXISTS ""Credentials"" (
                    ""Id"" TEXT NOT NULL CONSTRAINT ""PK_Credentials"" PRIMARY KEY,
                    ""ServiceName"" TEXT NOT NULL,
                    ""PortalUrl"" TEXT NULL,
                    ""Username"" TEXT NOT NULL,
                    ""EncryptedPassword"" TEXT NOT NULL,
                    ""EncryptedNotes"" TEXT NULL,
                    ""CompanyId"" TEXT NULL,
                    ""JobApplicationId"" TEXT NULL,
                    ""LastUsedAt"" TEXT NULL,
                    ""PasswordLastChangedAt"" TEXT NOT NULL,
                    ""CreatedAt"" TEXT NOT NULL,
                    ""UpdatedAt"" TEXT NOT NULL,
                    CONSTRAINT ""FK_Credentials_Companies_CompanyId"" FOREIGN KEY (""CompanyId"") REFERENCES ""Companies"" (""Id"") ON DELETE SET NULL,
                    CONSTRAINT ""FK_Credentials_JobApplications_JobApplicationId"" FOREIGN KEY (""JobApplicationId"") REFERENCES ""JobApplications"" (""Id"") ON DELETE SET NULL
                );
                CREATE INDEX IF NOT EXISTS ""IX_Credentials_ServiceName"" ON ""Credentials"" (""ServiceName"");
                CREATE INDEX IF NOT EXISTS ""IX_Credentials_CompanyId"" ON ""Credentials"" (""CompanyId"");

                CREATE TABLE IF NOT EXISTS ""SecuritySettings"" (
                    ""Key"" TEXT NOT NULL CONSTRAINT ""PK_SecuritySettings"" PRIMARY KEY,
                    ""Value"" TEXT NOT NULL,
                    ""UpdatedAt"" TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS ""CareerProfiles"" (
                    ""Id"" TEXT NOT NULL CONSTRAINT ""PK_CareerProfiles"" PRIMARY KEY,
                    ""Name"" TEXT NOT NULL,
                    ""Slug"" TEXT NOT NULL,
                    ""Relationship"" TEXT NOT NULL,
                    ""Email"" TEXT NOT NULL,
                    ""PhoneNumber"" TEXT NULL,
                    ""AvatarUrl"" TEXT NULL,
                    ""ColorCode"" TEXT NOT NULL,
                    ""IsPrimary"" INTEGER NOT NULL,
                    ""IsActive"" INTEGER NOT NULL,
                    ""Headline"" TEXT NULL,
                    ""Bio"" TEXT NULL,
                    ""CreatedAt"" TEXT NOT NULL,
                    ""UpdatedAt"" TEXT NOT NULL
                );
                CREATE UNIQUE INDEX IF NOT EXISTS ""IX_CareerProfiles_Slug"" ON ""CareerProfiles"" (""Slug"");

                CREATE TABLE IF NOT EXISTS ""CareerProfilePreferences"" (
                    ""Id"" TEXT NOT NULL CONSTRAINT ""PK_CareerProfilePreferences"" PRIMARY KEY,
                    ""CareerProfileId"" TEXT NOT NULL,
                    ""TargetRoles"" TEXT NOT NULL,
                    ""TargetLocations"" TEXT NOT NULL,
                    ""TargetSalaryMin"" TEXT NULL,
                    ""TargetSalaryMax"" TEXT NULL,
                    ""Currency"" TEXT NOT NULL,
                    ""OpenToRelocation"" INTEGER NOT NULL,
                    ""RemotePreference"" TEXT NOT NULL,
                    ""Notes"" TEXT NULL,
                    ""CreatedAt"" TEXT NOT NULL,
                    ""UpdatedAt"" TEXT NOT NULL,
                    CONSTRAINT ""FK_CareerProfilePreferences_CareerProfiles_CareerProfileId"" FOREIGN KEY (""CareerProfileId"") REFERENCES ""CareerProfiles"" (""Id"") ON DELETE CASCADE
                );

                CREATE TABLE IF NOT EXISTS ""CareerProfileAddresses"" (
                    ""Id"" TEXT NOT NULL CONSTRAINT ""PK_CareerProfileAddresses"" PRIMARY KEY,
                    ""CareerProfileId"" TEXT NOT NULL,
                    ""Type"" TEXT NOT NULL,
                    ""Street"" TEXT NOT NULL,
                    ""City"" TEXT NOT NULL,
                    ""StateOrProvince"" TEXT NOT NULL,
                    ""PostalCode"" TEXT NOT NULL,
                    ""Country"" TEXT NOT NULL,
                    ""IsPrimary"" INTEGER NOT NULL,
                    ""CreatedAt"" TEXT NOT NULL,
                    ""UpdatedAt"" TEXT NOT NULL,
                    CONSTRAINT ""FK_CareerProfileAddresses_CareerProfiles_CareerProfileId"" FOREIGN KEY (""CareerProfileId"") REFERENCES ""CareerProfiles"" (""Id"") ON DELETE CASCADE
                );

                CREATE INDEX IF NOT EXISTS ""IX_JobApplications_Status_ApplicationDate"" ON ""JobApplications"" (""Status"", ""ApplicationDate"");
                CREATE INDEX IF NOT EXISTS ""IX_Companies_Industry_SponsorshipStatus"" ON ""Companies"" (""Industry"", ""SponsorshipStatus"");
                CREATE INDEX IF NOT EXISTS ""IX_Jobs_Status_Priority"" ON ""Jobs"" (""Status"", ""Priority"");
                CREATE INDEX IF NOT EXISTS ""IX_Contacts_CompanyId_Category"" ON ""Contacts"" (""CompanyId"", ""Category"");
                CREATE INDEX IF NOT EXISTS ""IX_Interviews_ScheduledAt_Status"" ON ""Interviews"" (""ScheduledAt"", ""Status"");
                CREATE INDEX IF NOT EXISTS ""IX_Documents_Category_ExpiryDate"" ON ""Documents"" (""Category"", ""ExpiryDate"");
            ");

            // Migration: Add CareerProfileId column to domain tables if missing (SQLite ALTER TABLE ADD COLUMN)
            var alterQueries = new[]
            {
                "ALTER TABLE \"Documents\" ADD COLUMN \"CareerProfileId\" TEXT NULL;",
                "ALTER TABLE \"JobApplications\" ADD COLUMN \"CareerProfileId\" TEXT NULL;",
                "ALTER TABLE \"Contacts\" ADD COLUMN \"CareerProfileId\" TEXT NULL;",
                "ALTER TABLE \"Interviews\" ADD COLUMN \"CareerProfileId\" TEXT NULL;",
                "ALTER TABLE \"SponsorshipCases\" ADD COLUMN \"CareerProfileId\" TEXT NULL;",
                "ALTER TABLE \"WikiArticles\" ADD COLUMN \"CareerProfileId\" TEXT NULL;",
                "ALTER TABLE \"ResumeProfiles\" ADD COLUMN \"CareerProfileId\" TEXT NULL;",
                "ALTER TABLE \"Credentials\" ADD COLUMN \"CareerProfileId\" TEXT NULL;",
                "ALTER TABLE \"ReminderItems\" ADD COLUMN \"CareerProfileId\" TEXT NULL;",
                "ALTER TABLE \"KanbanCards\" ADD COLUMN \"CareerProfileId\" TEXT NULL;",
                "ALTER TABLE \"Profiles\" ADD COLUMN \"CareerProfileId\" TEXT NULL;"
            };
            foreach (var sql in alterQueries)
            {
                try
                {
                    db.Database.ExecuteSqlRaw(sql);
                }
                catch { /* Column already exists */ }
            }

            Log.Information("Database initialization completed.");

            try
            {
                var profileService = scope.ServiceProvider.GetRequiredService<CareerOS.Application.Common.Interfaces.ICareerProfileService>();
                await profileService.EnsureDefaultProfilesAndDirectoriesAsync();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Career Profiles default provisioning warning, continuing.");
            }

            try
            {
                var searchService = scope.ServiceProvider.GetRequiredService<ISearchService>();
                var stats = await searchService.GetSearchStatsAsync();
                if (stats.Values.Sum() == 0)
                {
                    Log.Information("Initial search index is empty. Populating FTS5 index across all entities...");
                    await searchService.ReindexAllAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Search index initialization warning, continuing.");
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Database auto-init warning, continuing.");
        }
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseSerilogRequestLogging();

    // Enable Swagger
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CareerOS API v1"));

    app.UseCors("AllowAll");
    app.MapControllers();
    app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow, service = "CareerOS Local-First" }));

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "CareerOS API terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
