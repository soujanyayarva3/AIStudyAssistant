using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using AIStudyAssistant.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace AIStudyAssistant.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendPasswordResetEmailAsync(
        string email,
        string resetLink)
    {
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                "AI Study Assistant",
                _configuration["Email:From"]
            )
        );

        message.To.Add(
            MailboxAddress.Parse(email)
        );

        message.Subject = "Reset Your AI Study Assistant Password";

        message.Body = new BodyBuilder
        {
            HtmlBody = $"""
            <div style="font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 30px;">

                <h2 style="color: #4f46e5;">
                    AI Study Assistant
                </h2>

                <h3>
                    Reset Your Password
                </h3>

                <p>
                    We received a request to reset your password.
                </p>

                <p>
                    Click the button below to create a new password.
                </p>

                <div style="margin: 30px 0;">
                    <a href="{resetLink}"
                       style="
                       background:#4f46e5;
                       color:white;
                       padding:14px 25px;
                       text-decoration:none;
                       border-radius:8px;
                       display:inline-block;
                       font-weight:bold;">
                        Reset Password
                    </a>
                </div>

                <p>
                    If you did not request a password reset,
                    you can safely ignore this email.
                </p>

                <p style="color:#777;">
                    AI Study Assistant
                </p>

            </div>
            """
        }.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _configuration["Email:SmtpServer"],
            int.Parse(_configuration["Email:Port"]!),
            SecureSocketOptions.StartTls
        );

        await smtp.AuthenticateAsync(
            _configuration["Email:Username"],
            _configuration["Email:Password"]
        );

        await smtp.SendAsync(message);

        await smtp.DisconnectAsync(true);
    }
}