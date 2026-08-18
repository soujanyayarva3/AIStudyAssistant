using AIStudyAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace AIStudyAssistant.Application.Features.AIChats.Commands;

public class UpdateAIChatCommandHandler
    : IRequestHandler<UpdateAIChatCommand>
{
    private readonly IAIChatRepository _repository;

    public UpdateAIChatCommandHandler(IAIChatRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdateAIChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await _repository.GetChatByIdAsync(request.ChatId, request.UserId);

        if (chat == null)
            return Unit.Value;

        chat.Question = request.Question;
        chat.Response = request.Response;

        await _repository.UpdateChatAsync(chat);

        return Unit.Value;
    }
}