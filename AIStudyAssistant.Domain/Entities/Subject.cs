namespace AIStudyAssistant.Domain.Entities;

public class Subject: BaseEntity
{
    public int SubjectId { get; set; }

    public string SubjectName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int UserId { get; set; }

    // Navigation
    public List<Note>? Notes { get; set; }
}