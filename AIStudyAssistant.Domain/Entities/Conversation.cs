using System.ComponentModel.DataAnnotations;
using AIStudyAssistant.Domain.Identity;

namespace AIStudyAssistant.Domain.Entities;

public class Conversation
{
    [Key]
    public int ConversationId { get; set; }


    [Required]
    public string Title { get; set; } = string.Empty;


    public int UserId { get; set; }

    public ApplicationUser? User { get; set; }


    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;


    public ICollection<AIChat> Chats { get; set; }
        = new List<AIChat>();
}