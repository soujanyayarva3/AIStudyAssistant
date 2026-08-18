using System.ComponentModel.DataAnnotations;
using AIStudyAssistant.Domain.Identity;

namespace AIStudyAssistant.Domain.Entities;

public class Summary
{
    [Key]
    public int SummaryId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string OriginalText { get; set; } = string.Empty;

    public string SummaryText { get; set; } = string.Empty;

    public string Keywords { get; set; } = string.Empty;

    public string Questions { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }

    public ApplicationUser? User { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string SummaryStyle { get; set; } = string.Empty;

    public int? SubjectId { get; set; }

    public Subject? Subject { get; set; }

    public bool IsGenerated { get; set; } = false;
}