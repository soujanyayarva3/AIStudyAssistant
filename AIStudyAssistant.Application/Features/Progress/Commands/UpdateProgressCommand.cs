using MediatR;

namespace AIStudyAssistant.Application.Features.Progress.Commands;

public class UpdateProgressCommand : IRequest
{
    public int UserId { get; set; }

    public int TotalSubjects { get; set; }

    public int TotalNotes { get; set; }

    public int CompletedStudyPlans { get; set; }

    public int TotalStudyPlans { get; set; }

    public int TotalQuizzes { get; set; }

    public double AverageQuizScore { get; set; }

    public double ProgressPercentage { get; set; }
}