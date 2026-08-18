namespace AIStudyAssistant.Application.Models;

public class SummaryResultDto
{
    public string Summary { get; set; } = string.Empty;

    public List<string> Keywords { get; set; } = new();

    public List<string> Questions { get; set; } = new();
}