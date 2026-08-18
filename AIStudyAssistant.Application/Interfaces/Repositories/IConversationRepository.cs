using AIStudyAssistant.Domain.Entities;

namespace AIStudyAssistant.Application.Interfaces.Repositories;

public interface IConversationRepository
{
    Task<Conversation> CreateAsync(Conversation conversation);

    Task<List<Conversation>> GetByUserIdAsync(int userId);
}