using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Conversations.Queries;

public class GetConversationsQuery : IRequest<List<Conversation>>
{
    public int UserId { get; set; }
}