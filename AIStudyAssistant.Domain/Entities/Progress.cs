using System.ComponentModel.DataAnnotations;
using AIStudyAssistant.Domain.Identity;
namespace AIStudyAssistant.Domain.Entities;

public class Progress: BaseEntity
{
    [Key]
    public int ProgressId { get; set; }

    public int UserId { get; set; }

    public int TotalSubjects { get; set; }

    public int TotalNotes { get; set; }

    public int CompletedStudyPlans { get; set; }

    public int TotalStudyPlans { get; set; }

    public int TotalQuizzes { get; set; }

    public double AverageQuizScore { get; set; }

    public double ProgressPercentage { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    public ApplicationUser? User { get; set; }
}