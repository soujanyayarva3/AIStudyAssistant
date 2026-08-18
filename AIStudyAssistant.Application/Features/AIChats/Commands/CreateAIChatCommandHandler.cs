using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Application.Services;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.AIChats.Commands;

public class CreateAIChatCommandHandler
    : IRequestHandler<CreateAIChatCommand, AIChat>
{
    private readonly IAIChatRepository _repository;
    private readonly IConversationRepository _conversationRepository;
    private readonly OllamaService _ollamaService;

    public CreateAIChatCommandHandler(
        IAIChatRepository repository,
        IConversationRepository conversationRepository,
        OllamaService ollamaService)
    {
        _repository = repository;
        _conversationRepository = conversationRepository;
        _ollamaService = ollamaService;
    }

    public async Task<AIChat> Handle(
        CreateAIChatCommand request,
        CancellationToken cancellationToken)
    {
        int conversationId;

        if (!request.ConversationId.HasValue || request.ConversationId.Value == 0)
        {
            var conversation = new Conversation
            {
                Title = request.Question.Length > 40
                    ? request.Question.Substring(0, 40)
                    : request.Question,

                UserId = request.UserId,
                CreatedDate = DateTime.UtcNow
            };

            conversation = await _conversationRepository.CreateAsync(conversation);

            conversationId = conversation.ConversationId;
        }
        else
        {
            conversationId = request.ConversationId.Value;
        }

        var aiResponse = await _ollamaService.GenerateResponseAsync(request.Question);

        var chat = new AIChat
        {
            ConversationId = conversationId,
            Question = request.Question,
            Response = aiResponse,
            UserId = request.UserId,
            CreatedDate = DateTime.UtcNow
        };

        return await _repository.CreateChatAsync(chat);
    }
}