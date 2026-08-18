using MediatR;

namespace AIStudyAssistant.Application.Features.AIChats.Commands;

public class UpdateAIChatCommand : IRequest
{
    public int ChatId { get; set; }

    public int UserId { get; set; }

    public string Question { get; set; } = string.Empty;

    public string Response { get; set; } = string.Empty;
}