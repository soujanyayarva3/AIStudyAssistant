using System.ComponentModel.DataAnnotations;
using AIStudyAssistant.Domain.Identity;
namespace AIStudyAssistant.Domain.Entities;

public class Quiz: BaseEntity
{
    [Key]
    public int QuizId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Question { get; set; } = string.Empty;

    public string OptionA { get; set; } = string.Empty;

    public string OptionB { get; set; } = string.Empty;

    public string OptionC { get; set; } = string.Empty;

    public string OptionD { get; set; } = string.Empty;

    public string CorrectAnswer { get; set; } = string.Empty;

    public int Score { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }

    public ApplicationUser? User { get; set; }
}