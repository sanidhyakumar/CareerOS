using CareerOS.Application.Features.Contacts.DTOs;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Contacts.Queries.GetContactStats;

public record GetContactStatsQuery : IRequest<ContactStatsDto>;

public class GetContactStatsQueryHandler : IRequestHandler<GetContactStatsQuery, ContactStatsDto>
{
    private readonly IUnitOfWork _uow;

    public GetContactStatsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ContactStatsDto> Handle(GetContactStatsQuery request, CancellationToken cancellationToken)
    {
        var contacts = (await _uow.Contacts.GetAllAsync(cancellationToken: cancellationToken)).ToList();

        int total = contacts.Count;
        int recruiters = contacts.Count(c => c.Category == ContactCategory.Recruiter);
        int hiringManagers = contacts.Count(c => c.Category == ContactCategory.HiringManager);
        int hr = contacts.Count(c => c.Category == ContactCategory.HumanResources);
        int referrals = contacts.Count(c => c.Category == ContactCategory.Referral);
        int immigration = contacts.Count(c => c.Category == ContactCategory.ImmigrationContact);
        int starred = contacts.Count(c => c.IsStarred);

        var nowPlus7 = DateTime.UtcNow.AddDays(7);
        int followUpsDue = contacts.Count(c => c.NextFollowUpDate.HasValue && c.NextFollowUpDate.Value <= nowPlus7);

        return new ContactStatsDto
        {
            TotalContacts = total,
            RecruitersCount = recruiters,
            HiringManagersCount = hiringManagers,
            HumanResourcesCount = hr,
            ReferralsCount = referrals,
            ImmigrationCount = immigration,
            FollowUpsDueCount = followUpsDue,
            StarredCount = starred
        };
    }
}
