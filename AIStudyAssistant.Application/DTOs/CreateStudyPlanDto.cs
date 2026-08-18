namespace AIStudyAssistant.Application.DTOs;

public class CreateStudyPlanDto
{
    public string Title { get; set; } = string.Empty;

    public DateTime TargetDate { get; set; }

    public string Status { get; set; } = "Pending";
}