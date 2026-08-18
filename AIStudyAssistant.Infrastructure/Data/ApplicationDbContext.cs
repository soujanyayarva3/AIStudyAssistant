using AIStudyAssistant.Domain.Entities;
using AIStudyAssistant.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AIStudyAssistant.Infrastructure.Data;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<StudyPlan> StudyPlans { get; set; }
    public DbSet<AIChat> AIChats { get; set; }
    public DbSet<Summary> Summaries { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Progress> Progresses { get; set; }
    public DbSet<Conversation> Conversations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Subject>()
            .HasKey(x => x.SubjectId);

        modelBuilder.Entity<Note>()
            .HasKey(x => x.NoteId);

        modelBuilder.Entity<StudyPlan>()
            .HasKey(x => x.PlanId);

        modelBuilder.Entity<AIChat>()
            .HasKey(x => x.ChatId);

        modelBuilder.Entity<Summary>()
            .HasKey(x => x.SummaryId);

        modelBuilder.Entity<Quiz>()
            .HasKey(x => x.QuizId);

        modelBuilder.Entity<Progress>()
            .HasKey(x => x.ProgressId);

        modelBuilder.Entity<Subject>()
            .HasMany(s => s.Notes)
            .WithOne(n => n.Subject)
            .HasForeignKey(n => n.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Conversation>()
            .HasMany(c => c.Chats)
            .WithOne(c => c.Conversation)
            .HasForeignKey(c => c.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedOn = DateTime.UtcNow;
                entry.Entity.CreatedOnUtc = DateTime.UtcNow;
                entry.Entity.ModifiedOn = DateTime.UtcNow;
                entry.Entity.ModifiedOnUtc = DateTime.UtcNow;
                entry.Entity.IsActive = true;
                entry.Entity.IsDeleted = false;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedOn = DateTime.UtcNow;
                entry.Entity.ModifiedOnUtc = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}