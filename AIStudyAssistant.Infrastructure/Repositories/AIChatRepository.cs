using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using AIStudyAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AIStudyAssistant.Infrastructure.Repositories;

public class AIChatRepository : IAIChatRepository
{
    private readonly ApplicationDbContext _context;

    public AIChatRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AIChat>> GetChatsAsync(int userId)
    {
        return await _context.AIChats
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task<AIChat?> GetChatByIdAsync(int chatId, int userId)
    {
        return await _context.AIChats
            .FirstOrDefaultAsync(c => c.ChatId == chatId && c.UserId == userId);
    }

    public async Task<AIChat> CreateChatAsync(AIChat chat)
    {
        var sw = Stopwatch.StartNew();

        Console.WriteLine("DB Start");

        _context.AIChats.Add(chat);

        Console.WriteLine("Before SaveChanges");

        await _context.SaveChangesAsync();

        Console.WriteLine($"After SaveChanges : {sw.ElapsedMilliseconds} ms");

        return chat;
    }

    public async Task UpdateChatAsync(AIChat chat)
    {
        _context.AIChats.Update(chat);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteChatAsync(AIChat chat)
    {
        _context.AIChats.Remove(chat);
        await _context.SaveChangesAsync();
    }
    public async Task<List<AIChat>> GetChatsByConversationAsync(int conversationId)
    {
        return await _context.AIChats
            .Where(x => x.ConversationId == conversationId)
            .OrderBy(x => x.CreatedDate)
            .ToListAsync();
    }
}