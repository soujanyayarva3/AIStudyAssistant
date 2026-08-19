using AIStudyAssistant.Application.Interfaces;
using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Application.Interfaces.Services;
using AIStudyAssistant.Domain.Identity;
using AIStudyAssistant.Infrastructure.Data;
using AIStudyAssistant.Infrastructure.Repositories;
using AIStudyAssistant.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// CORS
// =====================================================

var frontendUrl = builder.Configuration["FRONTEND_URL"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins(
                frontendUrl ?? "http://localhost:4200"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// =====================================================
// DATABASE
// =====================================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(
            "DefaultConnection"
        )
    )
);

// =====================================================
// IDENTITY
// =====================================================

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// =====================================================
// JWT AUTHENTICATION
// =====================================================

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!
                        )
                    )
            };
    });

builder.Services.AddAuthorization();

// =====================================================
// MEDIATR
// =====================================================

builder.Services.AddMediatR(
    Assembly.Load("AIStudyAssistant.Application")
);

// =====================================================
// REPOSITORIES
// =====================================================

builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<IStudyPlanRepository, StudyPlanRepository>();
builder.Services.AddScoped<IAIChatRepository, AIChatRepository>();
builder.Services.AddScoped<ISummaryRepository, SummaryRepository>();
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IProgressRepository, ProgressRepository>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();

// =====================================================
// APPLICATION SERVICES
// =====================================================

builder.Services.AddScoped<
    IProgressCalculationService,
    ProgressCalculationService
>();

// =====================================================
// EMAIL SERVICE
// =====================================================

builder.Services.AddScoped<
    IEmailService,
    EmailService
>();

// =====================================================
// OLLAMA
// =====================================================

builder.Services.AddHttpClient<
    AIStudyAssistant.Application.Services.OllamaService
>();

// =====================================================
// CONTROLLERS
// =====================================================

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });

// =====================================================
// SWAGGER
// =====================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token"
        }
    );

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        }
    );
});

// =====================================================
// BUILD APPLICATION
// =====================================================

var app = builder.Build();

// =====================================================
// SWAGGER
// =====================================================

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Study Assistant API v1");
    options.RoutePrefix = "swagger";
});

// =====================================================
// MIDDLEWARE
// =====================================================

// HTTPS disabled because Docker uses HTTP
// app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();