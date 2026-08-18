using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.AIChats.Queries;

public class GetChatsByConversationQuery : IRequest<List<AIChat>>
{
    public int ConversationId { get; set; }

    public GetChatsByConversationQuery(int conversationId)
    {
        ConversationId = conversationId;
    }
}