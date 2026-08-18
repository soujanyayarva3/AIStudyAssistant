namespace AIStudyAssistant.API.Models;

public class Note
{
    public int NoteId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public int SubjectId { get; set; }

    public int UserId { get; set; }

    // Navigation
    public Subject? Subject { get; set; }
}