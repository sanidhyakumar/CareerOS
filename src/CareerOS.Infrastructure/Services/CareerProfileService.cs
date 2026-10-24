using CareerOS.Application.Common.Interfaces;
using CareerOS.Application.Features.CareerProfiles.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareerOS.Infrastructure.Services;

public class CareerProfileService : ICareerProfileService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<CareerProfileService> _logger;

    public CareerProfileService(
        ApplicationDbContext context,
        IFileStorageService fileStorage,
        ILogger<CareerProfileService> logger)
    {
        _context = context;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    public async Task EnsureDefaultProfilesAndDirectoriesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var defaultProfiles = new List<(string Name, string Slug, string Rel, string Color, bool Primary, string Headline, string TargetRoles)>
            {
                ("Sanidhya Kumar", "sanidhya-kumar", "Self", "#2563eb", true, "Principal Staff Engineer & Architect", "Staff Software Engineer, Principal Architect, VP Engineering"),
                ("Komal Kumari", "komal-kumari", "Spouse", "#db2777", false, "Data & Tech Professional", "Senior Analyst, Technical Program Manager"),
                ("Pranay Singh", "pranay-singh", "Child", "#16a34a", false, "Student & Future Innovator", "Software Intern, Academic Scholar"),
                ("Nayansh Singh", "nayansh-singh", "Child", "#d97706", false, "Student & Junior Developer", "STEM Trainee, Junior Developer"),
                ("Friend", "friend", "Friend", "#7c3aed", false, "Peer Collaborator", "Various Roles"),
                ("Client", "client", "Client", "#0891b2", false, "Consulting Client", "Advisory"),
                ("Other", "other", "Other", "#64748b", false, "Alternate Profile", "General")
            };

            CareerProfile? primaryProfile = null;

            foreach (var def in defaultProfiles)
            {
                var existing = await _context.CareerProfiles
                    .Include(p => p.Preferences)
                    .FirstOrDefaultAsync(p => p.Slug == def.Slug, cancellationToken);

                if (existing == null)
                {
                    existing = new CareerProfile
                    {
                        Name = def.Name,
                        Slug = def.Slug,
                        Relationship = def.Rel,
                        ColorCode = def.Color,
                        IsPrimary = def.Primary,
                        IsActive = def.Primary, // Primary starts active
                        Email = $"{def.Slug}@local.careeros",
                        Headline = def.Headline
                    };

                    existing.Preferences = new CareerProfilePreference
                    {
                        CareerProfileId = existing.Id,
                        TargetRoles = def.TargetRoles,
                        TargetLocations = "United States, Canada, Remote",
                        TargetSalaryMin = def.Primary ? 180000 : null,
                        TargetSalaryMax = def.Primary ? 250000 : null,
                        Currency = "USD",
                        OpenToRelocation = true,
                        RemotePreference = "Remote"
                    };

                    await _context.CareerProfiles.AddAsync(existing, cancellationToken);
                    _logger.LogInformation("[CareerProfiles] Provisioned default profile: {Name} ({Slug})", def.Name, def.Slug);
                }

                if (def.Primary)
                {
                    primaryProfile = existing;
                }

                // Provision directory structure for each profile
                ProvisionProfileDirectories(def.Slug);
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Backfill unassigned domain entities to the primary profile (Sanidhya Kumar)
            if (primaryProfile != null)
            {
                await BackfillUnassignedEntitiesAsync(primaryProfile.Id, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CareerProfiles] Error during default profile provisioning");
        }
    }

    private void ProvisionProfileDirectories(string slug)
    {
        try
        {
            var categories = new[] { "identity", "academics", "certifications", "employment", "immigration", "resumes" };
            foreach (var cat in categories)
            {
                var profileCatDir = _fileStorage.GetFullPath(Path.Combine("profiles", slug, cat));
                if (!Directory.Exists(profileCatDir))
                {
                    Directory.CreateDirectory(profileCatDir);
                }
            }

            var appDir = _fileStorage.GetFullPath(Path.Combine("applications", slug));
            if (!Directory.Exists(appDir))
            {
                Directory.CreateDirectory(appDir);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[CareerProfiles] Warning provisioning directories for profile slug {Slug}", slug);
        }
    }

    private async Task BackfillUnassignedEntitiesAsync(Guid primaryProfileId, CancellationToken cancellationToken)
    {
        // 1. Documents
        var unassignedDocs = await _context.Documents
            .Where(d => d.CareerProfileId == null || d.CareerProfileId == Guid.Empty)
            .ToListAsync(cancellationToken);
        foreach (var doc in unassignedDocs)
        {
            doc.CareerProfileId = primaryProfileId;
        }

        // 2. JobApplications
        var unassignedApps = await _context.JobApplications
            .Where(a => a.CareerProfileId == null || a.CareerProfileId == Guid.Empty)
            .ToListAsync(cancellationToken);
        foreach (var app in unassignedApps)
        {
            app.CareerProfileId = primaryProfileId;
        }

        // 3. Contacts
        var unassignedContacts = await _context.Contacts
            .Where(c => c.CareerProfileId == null || c.CareerProfileId == Guid.Empty)
            .ToListAsync(cancellationToken);
        foreach (var contact in unassignedContacts)
        {
            contact.CareerProfileId = primaryProfileId;
        }

        // 4. Interviews
        var unassignedInterviews = await _context.Interviews
            .Where(i => i.CareerProfileId == null || i.CareerProfileId == Guid.Empty)
            .ToListAsync(cancellationToken);
        foreach (var interview in unassignedInterviews)
        {
            interview.CareerProfileId = primaryProfileId;
        }

        // 5. SponsorshipCases
        var unassignedSponsorships = await _context.SponsorshipCases
            .Where(s => s.CareerProfileId == null || s.CareerProfileId == Guid.Empty)
            .ToListAsync(cancellationToken);
        foreach (var sc in unassignedSponsorships)
        {
            sc.CareerProfileId = primaryProfileId;
        }

        // 6. WikiArticles
        var unassignedWiki = await _context.WikiArticles
            .Where(w => w.CareerProfileId == null || w.CareerProfileId == Guid.Empty)
            .ToListAsync(cancellationToken);
        foreach (var wiki in unassignedWiki)
        {
            wiki.CareerProfileId = primaryProfileId;
        }

        // 7. ResumeProfiles
        var unassignedResumes = await _context.ResumeProfiles
            .Where(r => r.CareerProfileId == null || r.CareerProfileId == Guid.Empty)
            .ToListAsync(cancellationToken);
        foreach (var res in unassignedResumes)
        {
            res.CareerProfileId = primaryProfileId;
        }

        // 8. Credentials
        var unassignedCredentials = await _context.Credentials
            .Where(c => c.CareerProfileId == null || c.CareerProfileId == Guid.Empty)
            .ToListAsync(cancellationToken);
        foreach (var cred in unassignedCredentials)
        {
            cred.CareerProfileId = primaryProfileId;
        }

        // 9. Reminders
        var unassignedReminders = await _context.ReminderItems
            .Where(r => r.CareerProfileId == null || r.CareerProfileId == Guid.Empty)
            .ToListAsync(cancellationToken);
        foreach (var rem in unassignedReminders)
        {
            rem.CareerProfileId = primaryProfileId;
        }

        // 10. KanbanCards
        var unassignedCards = await _context.KanbanCards
            .Where(k => k.CareerProfileId == null || k.CareerProfileId == Guid.Empty)
            .ToListAsync(cancellationToken);
        foreach (var card in unassignedCards)
        {
            card.CareerProfileId = primaryProfileId;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<CareerProfileDto>> GetAllProfilesAsync(CancellationToken cancellationToken = default)
    {
        var profiles = await _context.CareerProfiles
            .Include(cp => cp.Preferences)
            .Include(cp => cp.Addresses)
            .OrderByDescending(cp => cp.IsPrimary)
            .ThenBy(cp => cp.Name)
            .ToListAsync(cancellationToken);

        var result = new List<CareerProfileDto>();
        foreach (var p in profiles)
        {
            var dto = MapToDto(p);
            dto.DocumentCount = await _context.Documents.CountAsync(d => d.CareerProfileId == p.Id, cancellationToken);
            dto.ApplicationCount = await _context.JobApplications.CountAsync(a => a.CareerProfileId == p.Id, cancellationToken);
            dto.InterviewCount = await _context.Interviews.CountAsync(i => i.CareerProfileId == p.Id, cancellationToken);
            dto.ContactCount = await _context.Contacts.CountAsync(c => c.CareerProfileId == p.Id, cancellationToken);
            dto.SponsorshipCount = await _context.SponsorshipCases.CountAsync(s => s.CareerProfileId == p.Id, cancellationToken);
            dto.WikiCount = await _context.WikiArticles.CountAsync(w => w.CareerProfileId == p.Id, cancellationToken);
            dto.ResumeProfileCount = await _context.ResumeProfiles.CountAsync(r => r.CareerProfileId == p.Id, cancellationToken);
            result.Add(dto);
        }

        return result;
    }

    public async Task<CareerProfileDto?> GetProfileByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var profile = await _context.CareerProfiles
            .Include(cp => cp.Preferences)
            .Include(cp => cp.Addresses)
            .FirstOrDefaultAsync(cp => cp.Id == id, cancellationToken);

        if (profile == null) return null;

        var dto = MapToDto(profile);
        dto.DocumentCount = await _context.Documents.CountAsync(d => d.CareerProfileId == profile.Id, cancellationToken);
        dto.ApplicationCount = await _context.JobApplications.CountAsync(a => a.CareerProfileId == profile.Id, cancellationToken);
        dto.InterviewCount = await _context.Interviews.CountAsync(i => i.CareerProfileId == profile.Id, cancellationToken);
        dto.ContactCount = await _context.Contacts.CountAsync(c => c.CareerProfileId == profile.Id, cancellationToken);
        dto.SponsorshipCount = await _context.SponsorshipCases.CountAsync(s => s.CareerProfileId == profile.Id, cancellationToken);
        dto.WikiCount = await _context.WikiArticles.CountAsync(w => w.CareerProfileId == profile.Id, cancellationToken);
        dto.ResumeProfileCount = await _context.ResumeProfiles.CountAsync(r => r.CareerProfileId == profile.Id, cancellationToken);
        return dto;
    }

    public async Task<CareerProfileDto?> GetProfileBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var profile = await _context.CareerProfiles
            .Include(cp => cp.Preferences)
            .Include(cp => cp.Addresses)
            .FirstOrDefaultAsync(cp => cp.Slug.ToLower() == slug.ToLower(), cancellationToken);

        if (profile == null) return null;
        return await GetProfileByIdAsync(profile.Id, cancellationToken);
    }

    public async Task<CareerProfileDto> GetActiveProfileAsync(CancellationToken cancellationToken = default)
    {
        var profile = await _context.CareerProfiles
            .Include(cp => cp.Preferences)
            .Include(cp => cp.Addresses)
            .FirstOrDefaultAsync(cp => cp.IsActive, cancellationToken)
            ?? await _context.CareerProfiles
                .Include(cp => cp.Preferences)
                .Include(cp => cp.Addresses)
                .FirstOrDefaultAsync(cp => cp.IsPrimary, cancellationToken);

        if (profile == null)
        {
            await EnsureDefaultProfilesAndDirectoriesAsync(cancellationToken);
            profile = await _context.CareerProfiles
                .Include(cp => cp.Preferences)
                .Include(cp => cp.Addresses)
                .FirstAsync(cp => cp.IsPrimary, cancellationToken);
        }

        return (await GetProfileByIdAsync(profile.Id, cancellationToken))!;
    }

    public async Task SetActiveProfileAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var profiles = await _context.CareerProfiles.ToListAsync(cancellationToken);
        var target = profiles.FirstOrDefault(p => p.Id == id);
        if (target == null) throw new KeyNotFoundException($"Profile with ID {id} was not found.");

        foreach (var p in profiles)
        {
            p.IsActive = (p.Id == id);
            p.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<CareerProfileDto> CreateProfileAsync(CreateCareerProfileDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Profile name is required.");

        var slug = CareerProfile.GenerateSlug(dto.Name);
        var existingSlug = await _context.CareerProfiles.AnyAsync(p => p.Slug == slug, cancellationToken);
        if (existingSlug)
        {
            slug = $"{slug}-{Guid.NewGuid().ToString("N")[..4]}";
        }

        var profile = CareerProfile.Create(
            dto.Name,
            dto.Relationship,
            dto.Email,
            dto.PhoneNumber,
            dto.Headline,
            dto.ColorCode,
            dto.IsPrimary);

        profile.Slug = slug;
        profile.Bio = dto.Bio;
        profile.IsActive = false;

        if (dto.Preferences != null)
        {
            profile.Preferences = new CareerProfilePreference
            {
                CareerProfileId = profile.Id,
                TargetRoles = dto.Preferences.TargetRoles,
                TargetLocations = dto.Preferences.TargetLocations,
                TargetSalaryMin = dto.Preferences.TargetSalaryMin,
                TargetSalaryMax = dto.Preferences.TargetSalaryMax,
                Currency = dto.Preferences.Currency,
                OpenToRelocation = dto.Preferences.OpenToRelocation,
                RemotePreference = dto.Preferences.RemotePreference,
                Notes = dto.Preferences.Notes
            };
        }

        if (dto.Addresses != null && dto.Addresses.Any())
        {
            foreach (var addr in dto.Addresses)
            {
                profile.Addresses.Add(new CareerProfileAddress
                {
                    CareerProfileId = profile.Id,
                    Street = addr.Street,
                    City = addr.City,
                    StateOrProvince = addr.StateOrProvince,
                    PostalCode = addr.PostalCode,
                    Country = addr.Country,
                    Type = addr.Type,
                    IsPrimary = addr.IsPrimary
                });
            }
        }

        await _context.CareerProfiles.AddAsync(profile, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        ProvisionProfileDirectories(profile.Slug);

        return (await GetProfileByIdAsync(profile.Id, cancellationToken))!;
    }

    public async Task<CareerProfileDto> UpdateProfileAsync(Guid id, UpdateCareerProfileDto dto, CancellationToken cancellationToken = default)
    {
        var profile = await _context.CareerProfiles
            .Include(cp => cp.Preferences)
            .Include(cp => cp.Addresses)
            .FirstOrDefaultAsync(cp => cp.Id == id, cancellationToken);

        if (profile == null) throw new KeyNotFoundException($"Profile with ID {id} was not found.");

        profile.Name = dto.Name.Trim();
        profile.Relationship = dto.Relationship.Trim();
        profile.Email = dto.Email?.Trim().ToLowerInvariant() ?? string.Empty;
        profile.PhoneNumber = dto.PhoneNumber?.Trim();
        profile.Headline = dto.Headline?.Trim();
        profile.Bio = dto.Bio?.Trim();
        profile.ColorCode = string.IsNullOrWhiteSpace(dto.ColorCode) ? profile.ColorCode : dto.ColorCode.Trim();
        profile.UpdatedAt = DateTime.UtcNow;

        if (dto.Preferences != null)
        {
            if (profile.Preferences == null)
            {
                profile.Preferences = new CareerProfilePreference { CareerProfileId = profile.Id };
            }
            profile.Preferences.TargetRoles = dto.Preferences.TargetRoles;
            profile.Preferences.TargetLocations = dto.Preferences.TargetLocations;
            profile.Preferences.TargetSalaryMin = dto.Preferences.TargetSalaryMin;
            profile.Preferences.TargetSalaryMax = dto.Preferences.TargetSalaryMax;
            profile.Preferences.Currency = dto.Preferences.Currency;
            profile.Preferences.OpenToRelocation = dto.Preferences.OpenToRelocation;
            profile.Preferences.RemotePreference = dto.Preferences.RemotePreference;
            profile.Preferences.Notes = dto.Preferences.Notes;
            profile.Preferences.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return (await GetProfileByIdAsync(profile.Id, cancellationToken))!;
    }

    public async Task DeleteProfileAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var profile = await _context.CareerProfiles.FirstOrDefaultAsync(cp => cp.Id == id, cancellationToken);
        if (profile == null) throw new KeyNotFoundException($"Profile with ID {id} was not found.");

        if (profile.IsPrimary)
        {
            throw new InvalidOperationException("The primary workspace profile cannot be deleted.");
        }

        _context.CareerProfiles.Remove(profile);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<FamilyDashboardDto> GetFamilyDashboardAsync(CancellationToken cancellationToken = default)
    {
        var profiles = await _context.CareerProfiles
            .Include(cp => cp.Preferences)
            .OrderByDescending(cp => cp.IsPrimary)
            .ThenBy(cp => cp.Name)
            .ToListAsync(cancellationToken);

        var totalApps = await _context.JobApplications.CountAsync(cancellationToken);
        var totalDocs = await _context.Documents.CountAsync(cancellationToken);
        var totalInterviews = await _context.Interviews.CountAsync(cancellationToken);
        var totalOffers = await _context.JobApplications.CountAsync(a => a.Status == ApplicationStatus.OfferReceived || a.Status == ApplicationStatus.Accepted, cancellationToken);

        var summaries = new List<ProfileSummaryCardDto>();
        foreach (var p in profiles)
        {
            var activeApps = await _context.JobApplications
                .CountAsync(a => a.CareerProfileId == p.Id && a.Status != ApplicationStatus.Rejected && a.Status != ApplicationStatus.Withdrawn, cancellationToken);

            var upcomingInterviews = await _context.Interviews
                .CountAsync(i => i.CareerProfileId == p.Id && i.ScheduledAt >= DateTime.UtcNow && i.Status != InterviewStatus.Cancelled, cancellationToken);

            var offers = await _context.JobApplications
                .CountAsync(a => a.CareerProfileId == p.Id && (a.Status == ApplicationStatus.OfferReceived || a.Status == ApplicationStatus.Accepted), cancellationToken);

            var expiringDocs = await _context.Documents
                .CountAsync(d => d.CareerProfileId == p.Id && d.ExpiryDate != null && d.ExpiryDate <= DateTime.UtcNow.AddDays(90), cancellationToken);

            summaries.Add(new ProfileSummaryCardDto
            {
                ProfileId = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Relationship = p.Relationship,
                ColorCode = p.ColorCode,
                Headline = p.Headline,
                IsPrimary = p.IsPrimary,
                IsActive = p.IsActive,
                ActiveApplications = activeApps,
                UpcomingInterviews = upcomingInterviews,
                TotalOffers = offers,
                ExpiringDocuments = expiringDocs,
                TargetRole = p.Preferences?.TargetRoles ?? ""
            });
        }

        // Recent family activities across all profiles
        var recentApps = await _context.JobApplications
            .Include(a => a.CareerProfile)
            .Include(a => a.Job)
            .OrderByDescending(a => a.ApplicationDate)
            .Take(5)
            .ToListAsync(cancellationToken);

        var recentInterviews = await _context.Interviews
            .Include(i => i.CareerProfile)
            .OrderByDescending(i => i.ScheduledAt)
            .Take(5)
            .ToListAsync(cancellationToken);

        var activities = new List<FamilyActivityDto>();
        foreach (var a in recentApps)
        {
            activities.Add(new FamilyActivityDto
            {
                ProfileId = a.CareerProfileId ?? Guid.Empty,
                ProfileName = a.CareerProfile?.Name ?? "Sanidhya Kumar",
                ColorCode = a.CareerProfile?.ColorCode ?? "#2563eb",
                Action = "Applied for Position",
                Title = $"{a.Job?.Title ?? "Application"} ({a.Status})",
                Timestamp = a.ApplicationDate,
                Type = (a.Status == ApplicationStatus.OfferReceived || a.Status == ApplicationStatus.Accepted) ? "Offer" : "Application"
            });
        }

        foreach (var i in recentInterviews)
        {
            activities.Add(new FamilyActivityDto
            {
                ProfileId = i.CareerProfileId ?? Guid.Empty,
                ProfileName = i.CareerProfile?.Name ?? "Sanidhya Kumar",
                ColorCode = i.CareerProfile?.ColorCode ?? "#2563eb",
                Action = $"Scheduled {i.InterviewType} Interview",
                Title = i.Title,
                Timestamp = i.ScheduledAt,
                Type = "Interview"
            });
        }

        activities = activities.OrderByDescending(act => act.Timestamp).Take(8).ToList();

        return new FamilyDashboardDto
        {
            TotalProfiles = profiles.Count,
            TotalApplications = totalApps,
            TotalDocuments = totalDocs,
            TotalInterviews = totalInterviews,
            TotalOffers = totalOffers,
            ProfileSummaries = summaries,
            RecentActivities = activities
        };
    }

    public async Task<ProfileAnalyticsDto> GetProfileAnalyticsAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        var profile = await _context.CareerProfiles.FirstOrDefaultAsync(p => p.Id == profileId, cancellationToken);
        var name = profile?.Name ?? "Profile";

        var totalApps = await _context.JobApplications.CountAsync(a => a.CareerProfileId == profileId, cancellationToken);
        var interviewing = await _context.JobApplications.CountAsync(a => a.CareerProfileId == profileId && 
            (a.Status == ApplicationStatus.PhoneScreen || a.Status == ApplicationStatus.TechnicalInterview || 
             a.Status == ApplicationStatus.BehavioralInterview || a.Status == ApplicationStatus.FinalRound), cancellationToken);
        var offers = await _context.JobApplications.CountAsync(a => a.CareerProfileId == profileId && 
            (a.Status == ApplicationStatus.OfferReceived || a.Status == ApplicationStatus.Accepted), cancellationToken);
        var rejections = await _context.JobApplications.CountAsync(a => a.CareerProfileId == profileId && a.Status == ApplicationStatus.Rejected, cancellationToken);
        var conversion = totalApps > 0 ? Math.Round((double)offers / totalApps * 100.0, 1) : 0;

        var totalDocs = await _context.Documents.CountAsync(d => d.CareerProfileId == profileId, cancellationToken);
        var expiringDocs = await _context.Documents.CountAsync(d => d.CareerProfileId == profileId && d.ExpiryDate != null && d.ExpiryDate <= DateTime.UtcNow.AddDays(90), cancellationToken);
        var totalContacts = await _context.Contacts.CountAsync(c => c.CareerProfileId == profileId, cancellationToken);

        return new ProfileAnalyticsDto
        {
            ProfileId = profileId,
            ProfileName = name,
            TotalApplications = totalApps,
            Interviewing = interviewing,
            Offers = offers,
            Rejections = rejections,
            ConversionRate = conversion,
            TotalDocuments = totalDocs,
            ExpiringDocuments = expiringDocs,
            TotalContacts = totalContacts
        };
    }

    private static CareerProfileDto MapToDto(CareerProfile p)
    {
        return new CareerProfileDto
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            Relationship = p.Relationship,
            Email = p.Email,
            PhoneNumber = p.PhoneNumber,
            AvatarUrl = p.AvatarUrl,
            ColorCode = p.ColorCode,
            IsPrimary = p.IsPrimary,
            IsActive = p.IsActive,
            Headline = p.Headline,
            Bio = p.Bio,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            Preferences = p.Preferences == null ? null : new CareerProfilePreferenceDto
            {
                Id = p.Preferences.Id,
                CareerProfileId = p.Preferences.CareerProfileId,
                TargetRoles = p.Preferences.TargetRoles,
                TargetLocations = p.Preferences.TargetLocations,
                TargetSalaryMin = p.Preferences.TargetSalaryMin,
                TargetSalaryMax = p.Preferences.TargetSalaryMax,
                Currency = p.Preferences.Currency,
                OpenToRelocation = p.Preferences.OpenToRelocation,
                RemotePreference = p.Preferences.RemotePreference,
                Notes = p.Preferences.Notes
            },
            Addresses = p.Addresses.Select(a => new CareerProfileAddressDto
            {
                Id = a.Id,
                CareerProfileId = a.CareerProfileId,
                Type = a.Type,
                Street = a.Street,
                City = a.City,
                StateOrProvince = a.StateOrProvince,
                PostalCode = a.PostalCode,
                Country = a.Country,
                IsPrimary = a.IsPrimary
            }).ToList()
        };
    }
}
