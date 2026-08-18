using MediatR;
using AIStudyAssistant.Domain.Entities;

namespace AIStudyAssistant.Application.Features.AIChats.Commands;

public class CreateAIChatCommand : IRequest<AIChat>
{
    public int UserId { get; set; }

    public int? ConversationId { get; set; }

    public string Question { get; set; } = string.Empty;

    public string? ResponseStyle { get; set; }

    public bool ShowExamples { get; set; }
}