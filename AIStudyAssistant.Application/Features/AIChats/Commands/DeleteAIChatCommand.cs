using MediatR;

namespace AIStudyAssistant.Application.Features.AIChats.Commands;

public class DeleteAIChatCommand : IRequest
{
    public int ChatId { get; set; }

    public int UserId { get; set; }
}