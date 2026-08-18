using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Quizzes.Commands;

public class CreateQuizCommand : IRequest<List<Quiz>>
{
    public string Topic { get; set; } = string.Empty;

    public int Score { get; set; }

    public string Difficulty { get; set; } = "Medium";

    public int UserId { get; set; }
}