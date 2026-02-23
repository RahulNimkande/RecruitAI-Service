using Microsoft.EntityFrameworkCore;
using RecruitAI.Domain.Entities;

namespace RecruitAI.Infrastructure.Persistence;

public class RecruitAIDbContext : DbContext
{
    public RecruitAIDbContext(DbContextOptions<RecruitAIDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Recruiter> Recruiters { get; set; }
    public DbSet<RecruiterChatSession> RecruiterChatSessions { get; set; }
    public DbSet<RecruiterChatMessage> RecruiterChatMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // One User can have at most ONE Candidate profile
        modelBuilder.Entity<Candidate>()
            .HasOne(c => c.User)
            .WithOne()
            .HasForeignKey<Candidate>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // One User can have at most ONE Recruiter profile
        modelBuilder.Entity<Recruiter>()
            .HasOne(r => r.User)
            .WithOne()
            .HasForeignKey<Recruiter>(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecruiterChatMessage>()
            .HasOne(m => m.Session)
            .WithMany(s => s.Messages)
            .HasForeignKey(m => m.SessionId);
    }
}
