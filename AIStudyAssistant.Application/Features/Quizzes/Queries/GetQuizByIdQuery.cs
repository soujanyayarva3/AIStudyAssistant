using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Quizzes.Queries;

public class GetQuizByIdQuery : IRequest<Quiz?>
{
    public int QuizId { get; set; }

    public int UserId { get; set; }
}