using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.StudyPlans.Commands;

public class CreateStudyPlanCommand : IRequest<StudyPlan>
{
    public string TaskName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime DueDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public int UserId { get; set; }
}