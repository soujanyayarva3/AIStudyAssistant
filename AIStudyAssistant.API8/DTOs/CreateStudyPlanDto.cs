namespace AIStudyAssistant.API.DTOs;

public class CreateStudyPlanDto
{
  public string Title { get; set; } = string.Empty;

  public string Description { get; set; } = string.Empty;

  public DateTime DueDate { get; set; }

  public string Status { get; set; } = "Pending";
}
