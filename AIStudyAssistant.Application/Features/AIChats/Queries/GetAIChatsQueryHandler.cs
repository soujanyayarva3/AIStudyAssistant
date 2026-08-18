using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.AIChats.Queries;

public class GetAIChatsQueryHandler
    : IRequestHandler<GetAIChatsQuery, List<AIChat>>
{
    private readonly IAIChatRepository _repository;

    public GetAIChatsQueryHandler(IAIChatRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AIChat>> Handle(
        GetAIChatsQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetChatsAsync(request.UserId);
    }
}