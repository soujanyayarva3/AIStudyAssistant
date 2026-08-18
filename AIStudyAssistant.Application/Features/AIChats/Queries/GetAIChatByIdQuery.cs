using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.AIChats.Queries;

public class GetAIChatByIdQuery : IRequest<AIChat?>
{
    public int ChatId { get; set; }

    public int UserId { get; set; }
}