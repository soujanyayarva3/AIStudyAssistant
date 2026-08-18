using AIStudyAssistant.Domain.Entities;

namespace AIStudyAssistant.Application.Interfaces.Repositories;

public interface IAIChatRepository
{
    Task<List<AIChat>> GetChatsAsync(int userId);

    Task<AIChat?> GetChatByIdAsync(int chatId, int userId);

    Task<AIChat> CreateChatAsync(AIChat chat);

    Task<List<AIChat>> GetChatsByConversationAsync(int conversationId);

    Task UpdateChatAsync(AIChat chat);

    Task DeleteChatAsync(AIChat chat);
    
}