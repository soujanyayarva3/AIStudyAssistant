using MediatR;

namespace AIStudyAssistant.Application.Features.Quizzes.Commands;

public class DeleteQuizCommand : IRequest
{
    public int QuizId { get; set; }
    public int UserId { get; set; }
}