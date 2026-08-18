namespace AIStudyAssistant.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(
        string email,
        string resetLink);
}