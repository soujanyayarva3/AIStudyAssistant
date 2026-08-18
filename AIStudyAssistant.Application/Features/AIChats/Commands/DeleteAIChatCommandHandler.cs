using AIStudyAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace AIStudyAssistant.Application.Features.AIChats.Commands;

public class DeleteAIChatCommandHandler
    : IRequestHandler<DeleteAIChatCommand>
{
    private readonly IAIChatRepository _repository;

    public DeleteAIChatCommandHandler(IAIChatRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteAIChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await _repository.GetChatByIdAsync(request.ChatId, request.UserId);

        if (chat == null)
            return Unit.Value;

        await _repository.DeleteChatAsync(chat);

        return Unit.Value;
    }
}