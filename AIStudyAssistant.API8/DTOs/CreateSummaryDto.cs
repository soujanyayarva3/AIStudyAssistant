namespace AIStudyAssistant.Application.DTOs;

public class CreateSummaryDto
{
    public string Title { get; set; } = string.Empty;

    public string OriginalText { get; set; } = string.Empty;

    public string SummaryText { get; set; } = string.Empty;
}