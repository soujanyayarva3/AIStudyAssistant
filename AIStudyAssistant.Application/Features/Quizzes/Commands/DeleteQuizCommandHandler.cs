using AIStudyAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace AIStudyAssistant.Application.Features.Quizzes.Commands;

public class DeleteQuizCommandHandler : IRequestHandler<DeleteQuizCommand>
{
    private readonly IQuizRepository _repository;

    public DeleteQuizCommandHandler(IQuizRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _repository.GetQuizByIdAsync(request.QuizId, request.UserId);

        if (quiz == null)
            return Unit.Value;

        await _repository.DeleteQuizAsync(quiz);

        return Unit.Value;
    }
}