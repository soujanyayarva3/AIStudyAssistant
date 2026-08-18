using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.AIChats.Queries;

public record GetAIChatsQuery(int UserId) : IRequest<List<AIChat>>;