using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Quizzes.Queries;

public class GetQuizzesQueryHandler
    : IRequestHandler<GetQuizzesQuery, List<Quiz>>
{
    private readonly IQuizRepository _repository;

    public GetQuizzesQueryHandler(IQuizRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Quiz>> Handle(
        GetQuizzesQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetQuizzesAsync(request.UserId);
    }
}