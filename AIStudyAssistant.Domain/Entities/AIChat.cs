using System.ComponentModel.DataAnnotations;
using AIStudyAssistant.Domain.Identity;

namespace AIStudyAssistant.Domain.Entities;

public class AIChat : BaseEntity
{
    [Key]
    public int ChatId { get; set; }


    // Groups messages into one conversation
    public int ConversationId { get; set; }

    public Conversation? Conversation { get; set; }


    [Required]
    public string Question { get; set; } = string.Empty;


    [Required]
    public string Response { get; set; } = string.Empty;


    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;


    public int UserId { get; set; }

    public ApplicationUser? User { get; set; }
}