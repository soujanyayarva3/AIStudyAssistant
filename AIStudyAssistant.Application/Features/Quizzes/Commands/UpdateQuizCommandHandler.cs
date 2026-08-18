using AIStudyAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace AIStudyAssistant.Application.Features.Quizzes.Commands;

public class UpdateQuizCommandHandler : IRequestHandler<UpdateQuizCommand>
{
    private readonly IQuizRepository _repository;

    public UpdateQuizCommandHandler(IQuizRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _repository.GetQuizByIdAsync(request.QuizId, request.UserId);

        if (quiz == null)
            return Unit.Value;

        quiz.Title = request.Title;
        quiz.Question = request.Question;
        quiz.OptionA = request.OptionA;
        quiz.OptionB = request.OptionB;
        quiz.OptionC = request.OptionC;
        quiz.OptionD = request.OptionD;
        quiz.CorrectAnswer = request.CorrectAnswer;
        quiz.Score = request.Score;

        await _repository.UpdateQuizAsync(quiz);

        return Unit.Value;
    }
}