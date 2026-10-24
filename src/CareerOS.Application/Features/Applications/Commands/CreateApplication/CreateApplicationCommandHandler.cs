using AutoMapper;
using CareerOS.Application.Features.Applications.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Applications.Commands.CreateApplication;

public class CreateApplicationCommandHandler : IRequestHandler<CreateApplicationCommand, JobApplicationDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateApplicationCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<JobApplicationDto> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var job = await _uow.Jobs.GetByIdAsync(request.JobId, cancellationToken: cancellationToken);
        if (job == null)
            throw new NotFoundException(nameof(Job), request.JobId);

        Guid? companyId = request.CompanyId ?? job.CompanyId;
        string companyName = job.Company;
        if (companyId.HasValue)
        {
            var company = await _uow.Companies.GetByIdAsync(companyId.Value, cancellationToken: cancellationToken);
            if (company != null)
            {
                companyName = company.Name;
            }
        }

        // Create Application Entity
        var application = JobApplication.Create(
            jobId: request.JobId,
            companyId: companyId,
            resumeVersionId: request.ResumeVersionId,
            coverLetterId: request.CoverLetterId,
            portfolioVersionId: request.PortfolioVersionId,
            applicationDate: request.ApplicationDate,
            status: request.Status,
            channel: request.Channel,
            referralContactId: request.ReferralContactId,
            notes: request.Notes);

        await _uow.Applications.AddAsync(application, cancellationToken);

        // Gather frozen details for immutable snapshot
        string? resumeProfileName = null;
        string? resumeVersionName = null;
        int? resumeVersionNumber = null;
        string? resumeDocSha256 = null;
        string? resumeDocFileName = null;
        string? resumeContent = null;

        if (request.ResumeVersionId.HasValue)
        {
            var resumeVersion = await _uow.ResumeCenter.GetVersionByIdAsync(request.ResumeVersionId.Value, cancellationToken);
            if (resumeVersion != null)
            {
                resumeVersionName = resumeVersion.VersionName;
                resumeVersionNumber = resumeVersion.VersionNumber;
                resumeProfileName = resumeVersion.ResumeProfile?.Name;

                if (resumeVersion.DocumentId.HasValue)
                {
                    var doc = await _uow.Documents.GetByIdAsync(resumeVersion.DocumentId.Value, cancellationToken: cancellationToken);
                    if (doc != null)
                    {
                        resumeDocSha256 = doc.Sha256Hash;
                        resumeDocFileName = doc.OriginalFileName;
                        resumeContent = doc.Description;
                    }
                }
            }
        }

        string? coverLetterTitle = null;
        string? coverLetterBody = null;
        if (request.CoverLetterId.HasValue)
        {
            var cl = await _uow.ResumeCenter.GetCoverLetterByIdAsync(request.CoverLetterId.Value, cancellationToken);
            if (cl != null)
            {
                coverLetterTitle = cl.Title;
                coverLetterBody = cl.Body;
            }
        }

        string? portfolioName = null;
        string? portfolioUrl = null;
        if (request.PortfolioVersionId.HasValue)
        {
            var pf = await _uow.ResumeCenter.GetPortfolioVersionByIdAsync(request.PortfolioVersionId.Value, cancellationToken);
            if (pf != null)
            {
                portfolioName = pf.Name;
                portfolioUrl = pf.Url;
            }
        }

        string? referralName = null;
        if (request.ReferralContactId.HasValue)
        {
            var contact = await _uow.Contacts.GetByIdAsync(request.ReferralContactId.Value, cancellationToken: cancellationToken);
            if (contact != null)
            {
                referralName = contact.Name;
            }
        }

        // Create Immutable Snapshot
        var snapshot = ApplicationSnapshot.Create(
            jobApplicationId: application.Id,
            jobTitle: job.Title,
            companyName: companyName,
            department: job.Department,
            location: job.Location,
            workplaceType: job.WorkplaceType.ToString(),
            seniority: job.Seniority.ToString(),
            salaryRange: job.Salary ?? (job.MinSalary.HasValue ? $"{job.Currency} {job.MinSalary}-{job.MaxSalary}" : null),
            techStack: job.TechStack,
            resumeProfileName: resumeProfileName,
            resumeVersionName: resumeVersionName,
            resumeVersionNumber: resumeVersionNumber,
            resumeDocumentSha256: resumeDocSha256,
            resumeDocumentFileName: resumeDocFileName,
            resumeContentSnapshot: resumeContent,
            coverLetterTitle: coverLetterTitle,
            coverLetterBodySnapshot: coverLetterBody,
            portfolioName: portfolioName,
            portfolioUrlSnapshot: portfolioUrl,
            channel: request.Channel,
            referralContactName: referralName,
            submissionNotes: request.Notes);

        await _uow.Applications.AddSnapshotAsync(snapshot, cancellationToken);
        application.SnapshotId = snapshot.Id;

        // Add Initial Timeline Event
        string eventTitle = request.Status switch
        {
            ApplicationStatus.Draft => "Application Draft Created",
            ApplicationStatus.Applied => "Application Submitted",
            _ => $"Application Created with status {request.Status}"
        };

        var initialEvent = ApplicationTimelineEvent.Create(
            jobApplicationId: application.Id,
            status: request.Status,
            eventDate: request.ApplicationDate,
            title: eventTitle,
            notes: request.Notes);

        await _uow.Applications.AddTimelineEventAsync(initialEvent, cancellationToken);

        // Also sync Job status if applicable
        if (job.Status == JobStatus.Saved && request.Status == ApplicationStatus.Applied)
        {
            job.Status = JobStatus.Applied;
            job.AppliedAt = request.ApplicationDate;
            await _uow.Jobs.UpdateAsync(job, cancellationToken);
        }

        await _uow.SaveChangesAsync(cancellationToken);

        // Reload to return full representation
        var full = await _uow.Applications.GetByIdAsync(application.Id, includeDetails: true, cancellationToken: cancellationToken);
        return _mapper.Map<JobApplicationDto>(full ?? application);
    }
}
