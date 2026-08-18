using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.AIChats.Queries;

public class GetChatsByConversationQueryHandler
    : IRequestHandler<GetChatsByConversationQuery, List<AIChat>>
{
    private readonly IAIChatRepository _repository;

    public GetChatsByConversationQueryHandler(IAIChatRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AIChat>> Handle(
        GetChatsByConversationQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetChatsByConversationAsync(request.ConversationId);
    }
}