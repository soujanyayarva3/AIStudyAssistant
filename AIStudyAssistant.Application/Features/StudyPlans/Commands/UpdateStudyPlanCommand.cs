using MediatR;

namespace AIStudyAssistant.Application.Features.StudyPlans.Commands;

public class UpdateStudyPlanCommand : IRequest
{
    public int PlanId { get; set; }

    public string TaskName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime DueDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public int UserId { get; set; }
}