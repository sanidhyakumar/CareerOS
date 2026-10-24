using CareerOS.Domain.Entities;

namespace CareerOS.Domain.Interfaces;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetProfileAsync(bool includeDetails = true, CancellationToken cancellationToken = default);
    Task<UserProfile?> GetByIdAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default);
    Task<UserProfile> AddAsync(UserProfile profile, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserProfile profile, CancellationToken cancellationToken = default);

    Task<ProfileCitizenship?> GetCitizenshipByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddCitizenshipAsync(ProfileCitizenship citizenship, CancellationToken cancellationToken = default);
    Task DeleteCitizenshipAsync(Guid id, CancellationToken cancellationToken = default);

    Task<WorkAuthorization?> GetWorkAuthorizationByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddWorkAuthorizationAsync(WorkAuthorization workAuth, CancellationToken cancellationToken = default);
    Task UpdateWorkAuthorizationAsync(WorkAuthorization workAuth, CancellationToken cancellationToken = default);
    Task DeleteWorkAuthorizationAsync(Guid id, CancellationToken cancellationToken = default);

    Task<SocialLink?> GetSocialLinkByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddSocialLinkAsync(SocialLink socialLink, CancellationToken cancellationToken = default);
    Task UpdateSocialLinkAsync(SocialLink socialLink, CancellationToken cancellationToken = default);
    Task DeleteSocialLinkAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Address?> GetAddressByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAddressAsync(Address address, CancellationToken cancellationToken = default);
    Task UpdateAddressAsync(Address address, CancellationToken cancellationToken = default);
    Task DeleteAddressAsync(Guid id, CancellationToken cancellationToken = default);
}

