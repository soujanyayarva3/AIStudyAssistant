using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Quizzes.Queries;

public class GetQuizByIdQueryHandler
    : IRequestHandler<GetQuizByIdQuery, Quiz?>
{
    private readonly IQuizRepository _repository;

    public GetQuizByIdQueryHandler(IQuizRepository repository)
    {
        _repository = repository;
    }

    public async Task<Quiz?> Handle(
        GetQuizByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetQuizByIdAsync(
            request.QuizId,
            request.UserId);
    }
}