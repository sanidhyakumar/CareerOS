using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class InterviewInterviewer : BaseEntity
{
    public Guid InterviewId { get; set; }
    public Interview Interview { get; set; } = null!;

    public Guid ContactId { get; set; }
    public Contact Contact { get; set; } = null!;

    public string? Role { get; set; } // Lead Interviewer, Shadow, Bar Raiser, Hiring Manager

    public InterviewInterviewer() { }

    public static InterviewInterviewer Create(Guid interviewId, Guid contactId, string? role = null)
    {
        return new InterviewInterviewer
        {
            InterviewId = interviewId,
            ContactId = contactId,
            Role = role?.Trim()
        };
    }
}
