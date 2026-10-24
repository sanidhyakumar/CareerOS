using CareerOS.Application.Features.CareerProfiles.DTOs;

namespace CareerOS.Application.Common.Interfaces;

public interface ICareerProfileService
{
    Task<List<CareerProfileDto>> GetAllProfilesAsync(CancellationToken cancellationToken = default);
    Task<CareerProfileDto?> GetProfileByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CareerProfileDto?> GetProfileBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<CareerProfileDto> GetActiveProfileAsync(CancellationToken cancellationToken = default);
    Task SetActiveProfileAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CareerProfileDto> CreateProfileAsync(CreateCareerProfileDto dto, CancellationToken cancellationToken = default);
    Task<CareerProfileDto> UpdateProfileAsync(Guid id, UpdateCareerProfileDto dto, CancellationToken cancellationToken = default);
    Task DeleteProfileAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FamilyDashboardDto> GetFamilyDashboardAsync(CancellationToken cancellationToken = default);
    Task<ProfileAnalyticsDto> GetProfileAnalyticsAsync(Guid profileId, CancellationToken cancellationToken = default);
    Task EnsureDefaultProfilesAndDirectoriesAsync(CancellationToken cancellationToken = default);
}

