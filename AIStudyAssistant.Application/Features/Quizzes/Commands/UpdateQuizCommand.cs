using MediatR;

namespace AIStudyAssistant.Application.Features.Quizzes.Commands;

public class UpdateQuizCommand : IRequest
{
    public int QuizId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public string OptionA { get; set; } = string.Empty;
    public string OptionB { get; set; } = string.Empty;
    public string OptionC { get; set; } = string.Empty;
    public string OptionD { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public int Score { get; set; }
}