using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CareerOS.Application.Common.Interfaces;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareerOS.Infrastructure.Services;

public class DataPortabilityService : IDataPortabilityService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<DataPortabilityService> _logger;

    public DataPortabilityService(
        ApplicationDbContext context,
        IFileStorageService fileStorage,
        ILogger<DataPortabilityService> logger)
    {
        _context = context;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    public async Task<byte[]> ExportFullBackupJsonAsync(CancellationToken cancellationToken = default)
    {
        var profiles = await _context.CareerProfiles
            .Include(p => p.Preferences)
            .Include(p => p.Addresses)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        var companies = await _context.Companies.AsNoTracking().ToListAsync(cancellationToken);
        var jobs = await _context.Jobs.AsNoTracking().ToListAsync(cancellationToken);
        var applications = await _context.JobApplications
            .Include(a => a.TimelineEvents)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        var contacts = await _context.Contacts.AsNoTracking().ToListAsync(cancellationToken);
        var wikiArticles = await _context.WikiArticles.AsNoTracking().ToListAsync(cancellationToken);
        var interviews = await _context.Interviews.AsNoTracking().ToListAsync(cancellationToken);
        var sponsorshipCases = await _context.SponsorshipCases.AsNoTracking().ToListAsync(cancellationToken);
        var kanbanCards = await _context.KanbanCards.AsNoTracking().ToListAsync(cancellationToken);
        var documents = await _context.Documents.AsNoTracking().ToListAsync(cancellationToken);
        var reminders = await _context.ReminderItems.AsNoTracking().ToListAsync(cancellationToken);

        var backupPackage = new
        {
            Manifest = new
            {
                Version = "1.0",
                ExportedAt = DateTime.UtcNow,
                Platform = "CareerOS Local-First",
                RecordCounts = new Dictionary<string, int>
                {
                    { "CareerProfiles", profiles.Count },
                    { "Companies", companies.Count },
                    { "Jobs", jobs.Count },
                    { "Applications", applications.Count },
                    { "Contacts", contacts.Count },
                    { "WikiArticles", wikiArticles.Count },
                    { "Interviews", interviews.Count },
                    { "SponsorshipCases", sponsorshipCases.Count },
                    { "KanbanCards", kanbanCards.Count },
                    { "Documents", documents.Count },
                    { "Reminders", reminders.Count }
                }
            },
            CareerProfiles = profiles,
            Companies = companies,
            Jobs = jobs,
            Applications = applications,
            Contacts = contacts,
            WikiArticles = wikiArticles,
            Interviews = interviews,
            SponsorshipCases = sponsorshipCases,
            KanbanCards = kanbanCards,
            Documents = documents,
            Reminders = reminders
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return JsonSerializer.SerializeToUtf8Bytes(backupPackage, options);
    }

    public async Task<byte[]> ExportCsvAsync(string entityType, CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();

        switch (entityType?.ToLower())
        {
            case "jobs":
                sb.AppendLine("Id,Title,Company,Location,Status,Priority,Seniority,EmploymentType,MinSalary,MaxSalary,Currency,TechStack,CreatedAt");
                var jobs = await _context.Jobs.AsNoTracking().ToListAsync(cancellationToken);
                foreach (var j in jobs)
                {
                    sb.AppendLine($"{CsvEscape(j.Id.ToString())},{CsvEscape(j.Title)},{CsvEscape(j.Company)},{CsvEscape(j.Location)},{j.Status},{j.Priority},{j.Seniority},{j.EmploymentType},{j.MinSalary},{j.MaxSalary},{j.Currency},{CsvEscape(j.TechStack)},{j.CreatedAt:O}");
                }
                break;

            case "applications":
                sb.AppendLine("Id,JobId,Company,Status,ApplicationDate,Channel,Notes,CreatedAt");
                var apps = await _context.JobApplications
                    .Include(a => a.Job)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
                foreach (var a in apps)
                {
                    string jobTitle = a.Job?.Title ?? "Position";
                    string comp = a.Job?.Company ?? "Company";
                    sb.AppendLine($"{CsvEscape(a.Id.ToString())},{CsvEscape(jobTitle)},{CsvEscape(comp)},{a.Status},{a.ApplicationDate:O},{CsvEscape(a.Channel)},{CsvEscape(a.Notes)},{a.CreatedAt:O}");
                }
                break;

            case "companies":
                sb.AppendLine("Id,Name,Industry,Headquarters,Size,Stage,WebsiteUrl,SponsorshipStatus,PersonalRating,CreatedAt");
                var companies = await _context.Companies.AsNoTracking().ToListAsync(cancellationToken);
                foreach (var c in companies)
                {
                    sb.AppendLine($"{CsvEscape(c.Id.ToString())},{CsvEscape(c.Name)},{CsvEscape(c.Industry)},{CsvEscape(c.Headquarters)},{CsvEscape(c.Size)},{CsvEscape(c.Stage)},{CsvEscape(c.WebsiteUrl)},{c.SponsorshipStatus},{c.PersonalRating},{c.CreatedAt:O}");
                }
                break;

            case "contacts":
                sb.AppendLine("Id,Name,Category,Email,Phone,Company,JobTitle,LinkedIn,CreatedAt");
                var contacts = await _context.Contacts.AsNoTracking().ToListAsync(cancellationToken);
                foreach (var ct in contacts)
                {
                    sb.AppendLine($"{CsvEscape(ct.Id.ToString())},{CsvEscape(ct.Name)},{ct.Category},{CsvEscape(ct.Email)},{CsvEscape(ct.Phone)},{CsvEscape(ct.Company)},{CsvEscape(ct.JobTitle)},{CsvEscape(ct.LinkedIn)},{ct.CreatedAt:O}");
                }
                break;

            default:
                throw new ArgumentException($"Unsupported entity type for CSV export: {entityType}");
        }

        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
    }

    public async Task<byte[]> ExportMarkdownWikiZipAsync(CancellationToken cancellationToken = default)
    {
        var articles = await _context.WikiArticles.AsNoTracking().ToListAsync(cancellationToken);

        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var a in articles)
            {
                string folder = a.Category.ToString();
                string filename = $"{folder}/{a.Slug}.md";

                var entry = archive.CreateEntry(filename, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                using var writer = new StreamWriter(entryStream, Encoding.UTF8);

                // Write YAML Frontmatter
                writer.WriteLine("---");
                writer.WriteLine($"title: \"{a.Title.Replace("\"", "\\\"")}\"");
                writer.WriteLine($"slug: \"{a.Slug}\"");
                writer.WriteLine($"category: \"{a.Category}\"");
                writer.WriteLine($"tags: \"{a.Tags}\"");
                writer.WriteLine($"starred: {a.IsStarred.ToString().ToLower()}");
                writer.WriteLine($"createdAt: \"{a.CreatedAt:O}\"");
                writer.WriteLine($"updatedAt: \"{a.UpdatedAt:O}\"");
                writer.WriteLine("---");
                writer.WriteLine();
                writer.WriteLine($"# {a.Title}");
                writer.WriteLine();
                if (!string.IsNullOrWhiteSpace(a.Summary))
                {
                    writer.WriteLine($"> {a.Summary}");
                    writer.WriteLine();
                }
                writer.WriteLine(a.Content);
            }
        }

        return memoryStream.ToArray();
    }

    public async Task<byte[]> ExportFullSystemZipAsync(CancellationToken cancellationToken = default)
    {
        var jsonBackup = await ExportFullBackupJsonAsync(cancellationToken);
        var jobsCsv = await ExportCsvAsync("jobs", cancellationToken);
        var appsCsv = await ExportCsvAsync("applications", cancellationToken);
        var compCsv = await ExportCsvAsync("companies", cancellationToken);
        var contactsCsv = await ExportCsvAsync("contacts", cancellationToken);
        var wikiArticles = await _context.WikiArticles.AsNoTracking().ToListAsync(cancellationToken);
        var documents = await _context.Documents.AsNoTracking().ToListAsync(cancellationToken);

        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            // 1. JSON Snapshot
            AddFileToZip(archive, "careeros-database.json", jsonBackup);

            // 2. CSV folder
            AddFileToZip(archive, "csv/jobs.csv", jobsCsv);
            AddFileToZip(archive, "csv/applications.csv", appsCsv);
            AddFileToZip(archive, "csv/companies.csv", compCsv);
            AddFileToZip(archive, "csv/contacts.csv", contactsCsv);

            // 3. Wiki Markdown
            foreach (var a in wikiArticles)
            {
                string mdContent = $"---\ntitle: \"{a.Title}\"\nslug: \"{a.Slug}\"\ncategory: \"{a.Category}\"\ntags: \"{a.Tags}\"\n---\n\n# {a.Title}\n\n{a.Content}";
                AddFileToZip(archive, $"wiki/{a.Slug}.md", Encoding.UTF8.GetBytes(mdContent));
            }

            // 4. Manifest
            string sha = Convert.ToHexString(SHA256.HashData(jsonBackup)).ToLowerInvariant();
            var manifest = new
            {
                Package = "CareerOS Full Export Archive",
                ExportedAt = DateTime.UtcNow,
                DatabaseChecksumSha256 = sha,
                DocumentCount = documents.Count,
                WikiArticleCount = wikiArticles.Count
            };
            AddFileToZip(archive, "manifest.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true })));
        }

        return memoryStream.ToArray();
    }

    public async Task<IDictionary<string, int>> ImportBackupJsonAsync(
        Stream jsonStream,
        bool overwriteExisting = false,
        CancellationToken cancellationToken = default)
    {
        var summary = new Dictionary<string, int>
        {
            { "CareerProfiles", 0 },
            { "Companies", 0 },
            { "Jobs", 0 },
            { "Applications", 0 },
            { "Contacts", 0 },
            { "WikiArticles", 0 }
        };

        using var doc = await JsonDocument.ParseAsync(jsonStream, cancellationToken: cancellationToken);
        var root = doc.RootElement;

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // 0. Import CareerProfiles (Root Aggregates)
            if (root.TryGetProperty("careerProfiles", out var profilesElement) && profilesElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in profilesElement.EnumerateArray())
                {
                    string slug = GetStringProp(item, "slug") ?? "";
                    string name = GetStringProp(item, "name") ?? "";
                    if (string.IsNullOrWhiteSpace(slug) || string.IsNullOrWhiteSpace(name)) continue;

                    var existing = await _context.CareerProfiles.FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
                    if (existing == null)
                    {
                        var cp = new CareerProfile
                        {
                            Name = name,
                            Slug = slug,
                            Relationship = GetStringProp(item, "relationship") ?? "Self",
                            ColorCode = GetStringProp(item, "colorCode") ?? "#2563eb",
                            Headline = GetStringProp(item, "headline"),
                            Email = GetStringProp(item, "email") ?? $"{slug}@local.careeros"
                        };
                        _context.CareerProfiles.Add(cp);
                        summary["CareerProfiles"]++;
                    }
                }
                await _context.SaveChangesAsync(cancellationToken);
            }

            // 1. Import Companies
            if (root.TryGetProperty("companies", out var compElement) && compElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in compElement.EnumerateArray())
                {
                    string name = item.GetProperty("name").GetString() ?? "";
                    if (string.IsNullOrWhiteSpace(name)) continue;

                    var existing = await _context.Companies.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
                    if (existing == null)
                    {
                        var c = new Company
                        {
                            Name = name,
                            Description = GetStringProp(item, "description"),
                            Headquarters = GetStringProp(item, "headquarters"),
                            Industry = GetStringProp(item, "industry"),
                            WebsiteUrl = GetStringProp(item, "websiteUrl"),
                            Notes = GetStringProp(item, "notes")
                        };
                        _context.Companies.Add(c);
                        summary["Companies"]++;
                    }
                }
                await _context.SaveChangesAsync(cancellationToken);
            }

            // 2. Import Jobs
            if (root.TryGetProperty("jobs", out var jobsElement) && jobsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in jobsElement.EnumerateArray())
                {
                    string title = item.GetProperty("title").GetString() ?? "";
                    string company = item.GetProperty("company").GetString() ?? "";
                    if (string.IsNullOrWhiteSpace(title)) continue;

                    var existing = await _context.Jobs.FirstOrDefaultAsync(j => j.Title == title && j.Company == company, cancellationToken);
                    if (existing == null)
                    {
                        var j = new Job
                        {
                            Title = title,
                            Company = company,
                            Location = GetStringProp(item, "location"),
                            Description = GetStringProp(item, "description"),
                            Notes = GetStringProp(item, "notes"),
                            TechStack = GetStringProp(item, "techStack")
                        };
                        _context.Jobs.Add(j);
                        summary["Jobs"]++;
                    }
                }
                await _context.SaveChangesAsync(cancellationToken);
            }

            // 3. Import Contacts
            if (root.TryGetProperty("contacts", out var contactsElement) && contactsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in contactsElement.EnumerateArray())
                {
                    string name = item.GetProperty("name").GetString() ?? "";
                    string? email = GetStringProp(item, "email");
                    if (string.IsNullOrWhiteSpace(name)) continue;

                    var existing = await _context.Contacts.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
                    if (existing == null)
                    {
                        var ct = new Contact
                        {
                            Name = name,
                            Email = email,
                            Phone = GetStringProp(item, "phone"),
                            Company = GetStringProp(item, "company"),
                            JobTitle = GetStringProp(item, "jobTitle"),
                            Notes = GetStringProp(item, "notes")
                        };
                        _context.Contacts.Add(ct);
                        summary["Contacts"]++;
                    }
                }
                await _context.SaveChangesAsync(cancellationToken);
            }

            // 4. Import Wiki Articles
            if (root.TryGetProperty("wikiArticles", out var wikiElement) && wikiElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in wikiElement.EnumerateArray())
                {
                    string title = item.GetProperty("title").GetString() ?? "";
                    string slug = item.GetProperty("slug").GetString() ?? "";
                    if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(slug)) continue;

                    var existing = await _context.WikiArticles.FirstOrDefaultAsync(w => w.Slug == slug, cancellationToken);
                    if (existing == null)
                    {
                        var wiki = new WikiArticle
                        {
                            Title = title,
                            Slug = slug,
                            Content = GetStringProp(item, "content") ?? "",
                            Summary = GetStringProp(item, "summary"),
                            Tags = GetStringProp(item, "tags")
                        };
                        _context.WikiArticles.Add(wiki);
                        summary["WikiArticles"]++;
                    }
                }
                await _context.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("Imported backup payload: {Summary}", JsonSerializer.Serialize(summary));
            return summary;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to import JSON backup package.");
            throw;
        }
    }

    private static void AddFileToZip(ZipArchive archive, string entryName, byte[] content)
    {
        var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
        using var stream = entry.Open();
        stream.Write(content, 0, content.Length);
    }

    private static string CsvEscape(string? input)
    {
        if (string.IsNullOrEmpty(input)) return "\"\"";
        return $"\"{input.Replace("\"", "\"\"")}\"";
    }

    private static string? GetStringProp(JsonElement element, string propName)
    {
        if (element.TryGetProperty(propName, out var prop) && prop.ValueKind == JsonValueKind.String)
        {
            return prop.GetString();
        }
        return null;
    }
}
