using RecruitAI.Domain.Entities;

public class Recruiter
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; }

    public string CompanyName { get; set; }
}
