
using AIStudyAssistant.API.DTOs;
using AIStudyAssistant.Domain.Identity;
using AIStudyAssistant.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AIStudyAssistant.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _emailService = emailService;
    }

    // =====================================================
    // REGISTER
    // =====================================================

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var existingUser =
            await _userManager.FindByEmailAsync(dto.Email);

        if (existingUser != null)
            return BadRequest("Email already exists.");

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName
        };

        var result =
            await _userManager.CreateAsync(
                user,
                dto.Password
            );

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("User registered successfully.");
    }

    // =====================================================
    // LOGIN
    // =====================================================

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user =
            await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
            return Unauthorized(
                "Invalid email or password."
            );

        var result =
            await _signInManager.CheckPasswordSignInAsync(
                user,
                dto.Password,
                false
            );

        if (!result.Succeeded)
            return Unauthorized(
                "Invalid email or password."
            );

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()
            ),

            new Claim(
                ClaimTypes.Email,
                user.Email ?? ""
            ),

            new Claim(
                "FullName",
                user.FullName
            )
        };

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                )
            );

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

        var token =
            new JwtSecurityToken(
                issuer:
                    _configuration["Jwt:Issuer"],

                audience:
                    _configuration["Jwt:Audience"],

                claims: claims,

                expires:
                    DateTime.UtcNow.AddMinutes(
                        Convert.ToDouble(
                            _configuration[
                                "Jwt:ExpiryInMinutes"
                            ]
                        )
                    ),

                signingCredentials:
                    credentials
            );

        return Ok(new
        {
            Token =
                new JwtSecurityTokenHandler()
                    .WriteToken(token)
        });
    }

    // =====================================================
    // FORGOT PASSWORD
    // =====================================================

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordDto dto)
    {
        var user =
            await _userManager.FindByEmailAsync(dto.Email);

        // Do not reveal whether an email exists
        if (user == null)
        {
            return Ok(
                "If an account with this email exists, a password reset link will be sent."
            );
        }

        var token =
            await _userManager.GeneratePasswordResetTokenAsync(
                user
            );

        var encodedToken =
            Uri.EscapeDataString(token);

        var encodedEmail =
            Uri.EscapeDataString(dto.Email);

        var frontendUrl =
            _configuration["Frontend:Url"]
            ?? "http://localhost:4200";

        var resetLink =
            $"{frontendUrl}/forgot-password?email={encodedEmail}&token={encodedToken}";

        await _emailService.SendPasswordResetEmailAsync(
            dto.Email,
            resetLink
        );

        return Ok(
            "If an account with this email exists, a password reset link has been sent."
        );
    }

    // =====================================================
    // RESET PASSWORD
    // =====================================================

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordDto dto)
    {
        var user =
            await _userManager.FindByEmailAsync(
                dto.Email
            );

        if (user == null)
        {
            return BadRequest(
                "Invalid password reset request."
            );
        }

        var result =
            await _userManager.ResetPasswordAsync(
                user,
                dto.Token,
                dto.NewPassword
            );

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(
            "Password reset successfully."
        );
    }
}
