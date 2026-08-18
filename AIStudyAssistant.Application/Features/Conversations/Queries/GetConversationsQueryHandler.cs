using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Conversations.Queries;

public class GetConversationsQueryHandler
    : IRequestHandler<GetConversationsQuery, List<Conversation>>
{
    private readonly IConversationRepository _repository;

    public GetConversationsQueryHandler(IConversationRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Conversation>> Handle(
        GetConversationsQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByUserIdAsync(request.UserId);
    }
}