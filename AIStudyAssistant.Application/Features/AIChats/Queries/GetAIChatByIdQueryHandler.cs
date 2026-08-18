using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.AIChats.Queries;

public class GetAIChatByIdQueryHandler
    : IRequestHandler<GetAIChatByIdQuery, AIChat?>
{
    private readonly IAIChatRepository _repository;

    public GetAIChatByIdQueryHandler(IAIChatRepository repository)
    {
        _repository = repository;
    }

    public async Task<AIChat?> Handle(
        GetAIChatByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetChatByIdAsync(
            request.ChatId,
            request.UserId);
    }
}