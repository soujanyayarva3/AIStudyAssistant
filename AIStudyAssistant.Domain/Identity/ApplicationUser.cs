using Microsoft.AspNetCore.Identity;

namespace AIStudyAssistant.Domain.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public string FullName { get; set; } = string.Empty;
}