using CareerOS.Application.Common.Interfaces;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using CareerOS.Infrastructure.Repositories;
using CareerOS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CareerOS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=/app/data/careeros.db";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // Storage & Metadata Engine
        services.AddSingleton<IMetadataEngine, MetadataEngine>();
        services.AddSingleton<IFileStorageService, LocalFileStorageService>();

        // Repositories & Unit of Work
        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IResumeRepository, ResumeRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<IRegionRepository, RegionRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IResumeCenterRepository, ResumeCenterRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<IInterviewRepository, InterviewRepository>();
        services.AddScoped<IWikiRepository, WikiRepository>();
        services.AddScoped<ISponsorshipRepository, SponsorshipRepository>();
        services.AddScoped<IKanbanRepository, KanbanRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
